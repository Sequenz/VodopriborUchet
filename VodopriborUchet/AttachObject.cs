using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class AttachObject : Form
    {
        private int treeNodeID;
        private Main _main;
       /* public AttachObject(int treeNodeId)
        {
            InitializeComponent();
            treeNodeID = treeNodeId;
        }*/

        public AttachObject(int treeNodeId, Main @from)
        {
            InitializeComponent();
            treeNodeID = treeNodeId;
            _main = @from;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AttachObject_Load(object sender, EventArgs e)
        {
            
                 var context = new db_sqlceEntities();
            try
            {
                var query = from c in context.object_type
                    select new
                           {
                               c.id,
                               c.name //new {c_name = c.name, c_ucost = c.net, a_name = c.kod_BTI};
                           };
                var results = query.ToList();

                var query1 = from c in context.net
                    select new
                           {
                               c.id,
                               c.name //new {c_name = c.name, c_ucost = c.net, a_name = c.kod_BTI};
                           };

                var query2 = from c in context.owners
                             select new
                             {
                                 c.id,
                                 fio = c.surname + " " + c.name + " " + c.patronymic//new {c_name = c.name, c_ucost = c.net, a_name = c.kod_BTI};
                             };
                var results2 = query2.ToList();
                this.comboBox3.DataSource = results2;
                this.comboBox3.DisplayMember = "fio";
                this.comboBox3.ValueMember = "id";
                var results1 = query1.ToList();
                this.comboBox1.DataSource = results;
                this.comboBox1.DisplayMember = "name";
                this.comboBox1.ValueMember = "id";
                this.comboBox2.DataSource = results1;
                this.comboBox2.DisplayMember = "name";
                this.comboBox2.ValueMember = "id";

            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
         //  
            if (this.textBox1.Text != "")
            {
           
            using (var context = new db_sqlceEntities())
            {

                var place = new objects_place()
                {
                    name = this.textBox1.Text,
                    comment = this.textBox2.Text,
                    net_id = (int?)this.comboBox2.SelectedValue,
                    object_type_id = (int)this.comboBox1.SelectedValue,
                    owner_id = (int) this.comboBox3.SelectedValue,
                    objects_place_id =  treeNodeID,

                };
                
                context.objects_place.Add(place);
                context.SaveChanges();
                _main.loadTree();
                this.Close();
            }

            }
        }

    }
}
