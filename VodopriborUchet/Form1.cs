using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class Main : Form
    {
        private DataTable dtTable;
        private static int placeID;
        private int _treeNodeId;
        private string _treeNodeName;

        public Main()
        {
            InitializeComponent();
        }

        

        private void ресурсыToolStripMenuItem_Click(object sender, EventArgs e)
        {
           var edtResources = new EditResources();
           edtResources.ShowDialog(this);
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
                treeView1.SelectedNode = tn;
                if (tn == null)
                {
                    treeView1.SelectedNode = treeView1.TopNode;
                    contextMenuStrip1.Items[1].Enabled = false;
                    contextMenuStrip1.Items[2].Enabled = false;
                    contextMenuStrip1.Items[4].Enabled = false;
                }
                else
                {
                    contextMenuStrip1.Items[1].Enabled = true;
                    contextMenuStrip1.Items[2].Enabled = true;
                    contextMenuStrip1.Items[4].Enabled = true;
                }
                _treeNodeId = Convert.ToInt32(treeView1.SelectedNode.Name);
                _treeNodeName = treeView1.SelectedNode.Text;

               // MessageBox.Show(treeView1.SelectedNode.FullPath);
            }

                if (e.Button == MouseButtons.Left)
                {
                    TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);
                    treeView1.SelectedNode = tn;
                    var nodeID = Convert.ToInt32(treeView1.SelectedNode.Name);

                    using (var context = new db_sqlceEntities())
                    {
                        var query1 = from c in context.objects_place
                                     join o in context.owners on c.owner_id equals o.id
                                     where c.id == nodeID
                                     select new
                                     {
                                         fio = o.surname + " " + o.name + " " + o.patronymic,
                                         o.tel
                                     };
                        var rr = query1.ToList();
                        if (rr.LongCount() > 0)
                        {
                            this.listView1.Items[0].SubItems[1].Text = rr.First().fio;
                            this.listView1.Items[1].SubItems[1].Text = rr.First().tel;
                            this.listView1.Items[2].SubItems[1].Text = FindRootNode(treeView1.SelectedNode).Text;
                            this.listView1.Items[3].SubItems[1].Text = "423432432";
                            this.listView1.Items[4].SubItems[1].Text = "342334";
                            this.listView1.Items[5].SubItems[1].Text = "23423434";
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

        private void Main_Shown(object sender, EventArgs e)
        {
            loadTree();
        }

        public void loadTree()
        {
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
            List<TreeNode> nodeList = new List<TreeNode>();
            TreeNode node = null;
            foreach (var dRow in dRows)
            {
                node = new TreeNode(dRow["PlaceName"].ToString());
                placeID = Convert.ToInt16(dRow["PlaceID"]);

                node.Name = placeID.ToString();
                node.ToolTipText = placeID.ToString();

                if (nodeList.Find(FindNode) == null)
                {
                    DataRow[] childRows = dtTable.Select("PlaceParentID = " + dRow["PlaceID"]);
                    if (childRows.Length > 0)
                    {
                        TreeNode[] childNodes = RecurseRows(childRows);
                        node.Nodes.AddRange(childNodes);
                    }

                    nodeList.Add(node);
                }
            }

            TreeNode[] nodeArr = nodeList.ToArray();
            return nodeArr;
        }

        private static bool FindNode(TreeNode n)
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
        }

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
                    var  obPlc = context.objects_place.FirstOrDefault(p => p.id == _treeNodeId);
                    if (obPlc != null)
                    {
                        context.objects_place.Remove(obPlc);
                        context.SaveChanges();
                        loadTree();
                    }


                }
                catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }
                
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            


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

    }
}
