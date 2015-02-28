using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using OpenPop.Mime;
using Cursor = System.Windows.Forms.Cursor;
using Message = OpenPop.Mime.Message;
using Timer = System.Threading.Timer;


namespace VodopriborUchet
{
    public partial class Main : Form
    {
        private DataTable dtTable;
        private static int placeID;
        private int _treeNodeId;
        private string _treeNodeName;
        private List<int> doneNotes = new List<int>();
        private HttpServer httpServer;
        private Thread thread;
        //private TreeNode _lastSelectedNode;

        private delegate void addWarningService();
        private addWarningService myWarningDelegate;

        public Main()
        {
            InitializeComponent();
            myWarningDelegate = new addWarningService(warningDelegate);

        }

        

        private void ресурсыToolStripMenuItem_Click(object sender, EventArgs e)
        {
           var edtImpulse = new EditImpulse();
           edtImpulse.ShowDialog(this);
        }

        private void объектыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var edtObjects = new EditObjects();
            edtObjects.ShowDialog(this);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {


            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
                TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);
               
                if (tn == null)
                {
                    treeView1.SelectedNode = treeView1.TopNode;
                    contextMenuStrip1.Items[1].Enabled = false;
                    contextMenuStrip1.Items[2].Enabled = false;
                    contextMenuStrip1.Items[4].Enabled = false;
                }
                else
                {
                    treeView1.SelectedNode = tn;
                    _treeNodeId = Convert.ToInt32(treeView1.SelectedNode.Name);
                    _treeNodeName = treeView1.SelectedNode.Text;
                    contextMenuStrip1.Items[1].Enabled = true;
                    contextMenuStrip1.Items[2].Enabled = true;
                    contextMenuStrip1.Items[4].Enabled = true;
                }
                

               // MessageBox.Show(treeView1.SelectedNode.FullPath);
            }

                if (e.Button == MouseButtons.Left)
                {
                    TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);


                    if (tn != null)
                    {
                       // MessageBox.Show(tn.Name+ " " + tn.Text + " " + tn.Tag);
                        treeView1.SelectedNode = tn;
                        var nodeID = Convert.ToInt32(treeView1.SelectedNode.Name);
                        _treeNodeId = Convert.ToInt32(treeView1.SelectedNode.Name);
                        _treeNodeName = treeView1.SelectedNode.Text;
                        using (var context = new db_sqlceEntities())
                        {
                            var query1 = from c in context.objects_place
                                /*join o in context.owners on c.owner_id equals o.id
                                join cou in context.counters on c.id equals cou.objects_place_id
                                where c.id == nodeID
                                select new
                                       {
                                           fio = o.surname + " " + o.name + " " + o.patronymic,
                                           o.tel, counter = cou.serial_amspi, sn1 = cou.serial_r1, sn2 = cou.serial_r2
                                       };*/

                                from cou in context.counters.Where(cou => cou.objects_place_id == c.id).DefaultIfEmpty()
                                from o in context.owners.Where(o => o.id == c.owner_id)
                                         where c.id == nodeID
                                         select new
                                         {
                                             fio = o.surname + " " + o.name + " " + o.patronymic,
                                             o.tel,
                                             counter = cou.serial_amspi,
                                             sn1 = cou.serial_r1,
                                             sn2 = cou.serial_r2
                                         };
                            var rr = query1.ToList();
                            if (rr.LongCount() > 0)
                            {
                                this.listView1.Items[0].SubItems[1].Text = rr.First().fio;
                                this.listView1.Items[1].SubItems[1].Text = rr.First().tel;
                                this.listView1.Items[2].SubItems[1].Text = FindRootNode(treeView1.SelectedNode).Text;
                                this.listView1.Items[3].SubItems[1].Text = rr.First().counter;
                                this.listView1.Items[4].SubItems[1].Text = rr.First().sn1;
                                this.listView1.Items[5].SubItems[1].Text = rr.First().sn2;
                            }
                            else
                            {
                                this.listView1.Items[0].SubItems[1].Text = "";
                                this.listView1.Items[1].SubItems[1].Text = "";
                                this.listView1.Items[2].SubItems[1].Text = "";
                                this.listView1.Items[3].SubItems[1].Text = "";
                                this.listView1.Items[4].SubItems[1].Text = "";
                                this.listView1.Items[5].SubItems[1].Text = "";
                            }
                        }


                    }
                    else
                    {
                        if (treeView1.TopNode != null) treeView1.SelectedNode = treeView1.TopNode;
                    }
                }
                this.groupBox2.Text = treeView1.SelectedNode.Text;

            updateDataPackagesGrid();
            updateAlarmPackageGrid();
        }

        private TreeNode FindRootNode(TreeNode treeNode)
        {
            while (treeNode.Parent != null)
            {
                treeNode = treeNode.Parent;
            }
            return treeNode;
        }
        private void Main_Load(object sender, EventArgs e)
        {
            treeView1.MouseDown += new MouseEventHandler(treeView1_MouseDown);
           
            loadTree();
            

            startHttpService();
            MailService();

            var timer = new System.Timers.Timer { Interval = 10000 }; //Interval = 30 * 60000
            timer.Elapsed += startWarningService;
            
            timer.Start();
           

            updateDataPackagesGrid();
            updateAlarmPackageGrid();
          
        }

       
        private void startWarningService(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            this.Invoke(myWarningDelegate);
            
            
        }

        private void warningDelegate()
        {
            var context = new db_sqlceEntities();

            try
            {
                var w_query = context.warning_conditions.FirstOrDefault();

                var tr = DateTime.Now.AddMonths(-1);
                var ttr = DateTime.Now;
                var query = from c in context.package_iz
                            where ((c.date_inc >= tr && c.date_inc <= ttr)
                    && (c.vg < w_query.gprs_mod_min || c.vg > w_query.gprs_mod_max || c.gp < w_query.gprs_sig_min || c.gp > w_query.gprs_sig_max || c.vc < w_query.controller_voltage_min || c.vc > w_query.controller_voltage_max))

                            select new
                            {
                                na = c.na,
                                vc = c.vc,
                                vg = c.vg,
                                gp = c.gp
                            };

                var results = query.ToList();
                List<string> lst = new List<string>();
                foreach (var item in results)
                {
                    string txt_gp_max;
                    string txt_gp_min;
                    string txt_vg_max;
                    string txt_vg_min;
                    string txt_ct_min;
                    string txt_ct_max;
                    if (item.vg > w_query.gprs_mod_max) { txt_vg_max = "ППУ-РМ № "+ item.na + " Напряжение GPRS модуля превышено!"; lst.Add(txt_vg_max); }
                    if (item.vg < w_query.gprs_mod_min) { txt_vg_min = "ППУ-РМ № " + item.na + " Напряжение GPRS модуля ниже нормы!"; lst.Add(txt_vg_min); }
                    if (item.gp < w_query.gprs_sig_min) { txt_gp_min = "ППУ-РМ № " + item.na + " Уровень GPRS сиграла ниже нормы!"; lst.Add(txt_gp_min); }
                    if (item.gp > w_query.gprs_sig_max) { txt_gp_max = "ППУ-РМ № " + item.na + " Уровень GPRS сиграла превышен!"; lst.Add(txt_gp_max); }
                    if (item.vc < w_query.controller_voltage_min) { txt_ct_min = "ППУ-РМ № " + item.na + " Напряжение контроллера ниже нормы!"; lst.Add(txt_ct_min); }
                    if (item.vc > w_query.controller_voltage_max) { txt_ct_max = "ППУ-РМ № " + item.na + " Напряжение контроллера превышено!"; lst.Add(txt_ct_max); }

                }

                listBox1.Items.Clear();
                listBox1.Items.AddRange(lst.ToArray());
                listBox1.Update();
                lst.Clear();
            }
            catch (SqlCeException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updateAlarmPackageGrid()
        {
            var context = new db_sqlceEntities();
            var cn = listView1.Items[3].SubItems[1].Text;
            try
            {
                var query = from c in context.package_al
                            where (c.date_inc >= dateTimePicker1.Value && c.date_inc <= dateTimePicker2.Value) &&
                             (c.na.Equals(cn))

                            select new
                            {
                                da = c.date_inc,
                                sn = c.na,
                                wr = c.wr_txt

                            };

                var results = query.ToList();
                this.bindingWarningSource.DataSource = results;


            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        public class PackageC
        {
            public DateTime date { get; set; }
            public float r1 { get; set; }
            public float r2 { get; set; }
            public float r1_c { get; set; }
            public float r2_c { get; set; }
        }

        private void updateDataPackagesGrid()
        {
            var context = new db_sqlceEntities();
            var cn = listView1.Items[3].SubItems[1].Text;
            try
            {
                var query = from c in context.package_iz
                            from p1 in context.impulse.Where(p => p.code == c.pw1).DefaultIfEmpty()
                            from p2 in context.impulse.Where(p => p.code == c.pw2).DefaultIfEmpty()
                            where (c.da >= dateTimePicker1.Value && c.da <= dateTimePicker2.Value) &&
                             (c.na.Equals(cn))

                    select  new PackageC()
                            {
                                date = c.date_inc,
                                r1 = (float)(c.r1 * p1.weight) / 1000,// * c.pw1 / 1000,
                                r2 = (float)(c.r2 * p2.weight) / 1000, //* c.pw2 / 1000
                                r1_c = 0,
                                r2_c = 0

                            };
                

             

                var results = query.ToList();


                for (var i = 1; i < results.Count; i++)
                {
                    results[i].r1_c = results[i].r1 - results[i - 1].r1;
                    results[i].r2_c = results[i].r2 - results[i - 1].r2;
                }

                this.bindingPackageSource.DataSource = results;

             /*   DataTable dat = new DataTable();


                using (SqlCeDataAdapter adap =
                          new SqlCeDataAdapter("SELECT * from package_iz unpivot (Cons for  r1_0, r1_1,r1_2,r1_3,r1_4,r1_5,r1_6,r1_7,r1_8,r1_9,r1_10,r1_11 FROM package_iz p" +
                                              
                                               " ", @"Data Source=C:\Users\User\Documents\Visual Studio 2013\Projects\VodopriborUchet\VodopriborUchet\db\db_sqlce.sdf"))
                {
                    //the adapter will open and close the connection for you.
                    adap.Fill(dat);
                }
*/
            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void добавитьОбъектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var addPlc = new AddPlace(this);
            addPlc.ShowDialog(this);
        }

        private void единицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var edtUnits = new EditUnits();
            edtUnits.ShowDialog(this);
            
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void startHttpService()
        {
            int ip_port = 8080;
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    if (opt != null && opt.is_ip)
                    {
                            ip_port = (int) opt.ip_port;
                            httpServer = new MyHttpServer(ip_port);
                            thread = new Thread(new ThreadStart(httpServer.listen));
                            Debug.WriteLine("стартуем хттп");
                            thread.IsBackground = true;
                            thread.Start();
                    }

                }
                catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
            

        }

        private void MailService()
        {
            MailService mailService = VodopriborUchet.MailService.GetInstance();
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    if (opt != null)
                    {
                        if (opt.is_mail == true)
                        {
                            mailService.StartMailService();
                        }
                        if (opt.is_mail == false)
                        {
                            mailService.StopMailService();
                        }
                       
                    }
                   
                }
                catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
            
        }

       

        public void loadTree()
        {
           
            doneNotes.Clear();
            dtTable = new DataTable("TreeView");
            dtTable.Columns.Add("PlaceID", typeof(string));
            dtTable.Columns.Add("PlaceName", typeof(string));
            DataColumn dc = new DataColumn("PlaceParentID", typeof(string)) { AllowDBNull = true };
            dtTable.Columns.Add(dc);

            using (var context = new db_sqlceEntities())
            {
                foreach (var db in context.objects_place.ToList())
                {
                    dtTable.Rows.Add(db.id, db.name, db.objects_place_id);
                }
            }


            CreateNodes();
           
           /* byte[] myBytes = {0,0,0,0,0,88,112,64}; //assume you pad your array with enough zeros to make it 8 bytes.
            var myDouble = BitConverter.ToDouble(myBytes,0);
            byte[] byt = BitConverter.GetBytes(261.5);
            Console.WriteLine(byt.ToString());
            Console.WriteLine(myDouble.ToString("F", CultureInfo.InvariantCulture));*/

        }

        private void CreateNodes()
        {
            DataRow[] dRow = new DataRow[dtTable.Rows.Count];
            dtTable.Rows.CopyTo(dRow,0);
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            TreeNode[] tNode = RecurseRows(dRow);

            treeView1.Nodes.AddRange(tNode);
            treeView1.EndUpdate();
        }

        private TreeNode[] RecurseRows(DataRow[] dRows)
        {
            var nodeList = new List<TreeNode>();
            foreach (var dRow in dRows)
            {
                TreeNode node = new TreeNode(dRow["PlaceName"].ToString());
                placeID = Convert.ToInt32(dRow["PlaceID"]);

                node.Name = placeID.ToString();
                node.ToolTipText = placeID.ToString();
                

               // if (nodeList.Find(FindNode) == null)
                if (doneNotes.Contains(placeID)) continue;
                var childRows = dtTable.Select("PlaceParentID = " + placeID);
                if (childRows.Length > 0)
                {
                    TreeNode[] childNodes = RecurseRows(childRows);
                    node.Nodes.AddRange(childNodes);
                }
                var nID = Convert.ToInt32(node.Name);
                doneNotes.Add(nID);
                nodeList.Add(node);
            }

            TreeNode[] nodeArr = nodeList.ToArray();
            return nodeArr;
        }

      /*  private static bool FindNode(TreeNode n)
        {
            if (n.Nodes.Count == 0)
                return n.Name == placeID.ToString();
            else
            {
                while (n.Nodes.Count > 0)
                {
                    foreach (TreeNode tn in n.Nodes)
                    {
                        if (tn.Name == placeID.ToString())
                            return true;
                        else
                            n = tn;
                    }
                }
                return false;
            }
        }*/



        private void вложитьОбъектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var attachObject = new AttachObject(_treeNodeId, this);
            attachObject.ShowDialog(this);
        }

        private void владельцыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var edtOwners = new EditOwners();
            edtOwners.ShowDialog(this);
        }

        private void установитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var installRec = new InstallRecorder(_treeNodeId, _treeNodeName);
            installRec.ShowDialog(this);
        }

        private void удалитьОбъектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var ttt = context.objects_place.LongCount(p => p.objects_place_id == _treeNodeId);
                    if (ttt > 0)
                    {
                        MessageBox.Show("У объекта есть вложенные объекты!");
                    }
                    else
                    {
                    var  obPlc = context.objects_place.FirstOrDefault(p => p.id == _treeNodeId);
                    if (obPlc != null)
                    {
                        context.objects_place.Remove(obPlc);
                        context.SaveChanges();
                        loadTree();
                    }

                    }

                }
                catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }
                
            }
        }


        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var abt = new About();
            abt.ShowDialog(this);
        }

        private void условияПредупрежденийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var warnC = new WarningConditions();
            warnC.ShowDialog(this);
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            updateDataPackagesGrid();
            updateAlarmPackageGrid();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            updateDataPackagesGrid();
            updateAlarmPackageGrid();
        }

        private void подключенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var opt = new editOptions();
            opt.ShowDialog(this);
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (this.tabControl1.SelectedTab == tabPage3)
            {
                 var chart = this.chart1;
                 var context = new db_sqlceEntities();
            var cn = listView1.Items[3].SubItems[1].Text;
            try
            {
                var query = from c in context.package_iz
                            from p1 in context.impulse.Where(p => p.code == c.pw1).DefaultIfEmpty()
                            from p2 in context.impulse.Where(p => p.code == c.pw2).DefaultIfEmpty()
                            where (c.da >= dateTimePicker1.Value && c.da <= dateTimePicker2.Value) &&
                             (c.na.Equals(cn))

                    select  new
                            {
                                date = c.da,
                               // r1 = c.r1/100,// * c.pw1 / 1000,
                              //  r2 = c.r2/100 //* c.pw2 / 1000
                                r1 = (float)(c.r1 * p1.weight) / 1000,// * c.pw1 / 1000,
                                r2 = (float)(c.r2 * p2.weight) / 1000, //* c.pw2 / 1000
                            
                            };



                var results = query.ToList();
                if (results.Count <= 0) return;
                
                chart.Series[1].XValueType = ChartValueType.DateTime;
                chart.Series[1].YValueType = ChartValueType.Int64;
                chart.Series[0].XValueType = ChartValueType.DateTime;
                chart.Series[0].YValueType = ChartValueType.Int64;
                chart.Series[1].Points.DataBindXY(results, "date", results, "r2");
                chart.Series[0].Points.DataBindXY(results, "date", results, "r1");


            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }

               
                
               
            }
        }


      

    }
}
