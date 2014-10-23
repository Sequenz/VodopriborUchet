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
    public partial class AddPlace : Form
    {
        public AddPlace()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (var context = new db_sqlceEntities())
            {

                var place = new objects_place()
                {
                    name = this.textBox1.Text,
                    comment = this.textBox4.Text,
                    kod_BTI = this.textBox2.Text,
                    net_id = (int?) this.comboBox2.SelectedValue,
                    object_type_id = (int) this.comboBox1.SelectedValue
                };
                context.objects_place.Add(place);
                context.SaveChanges();
                this.Close();
            } 

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddPlace_Load(object sender, EventArgs e)
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
                var results1 = query1.ToList();
                this.comboBox1.DataSource = results;
                this.comboBox1.DisplayMember = "name";
                this.comboBox1.ValueMember = "id";
                this.comboBox2.DataSource = results1;
                this.comboBox2.DisplayMember = "name";
                this.comboBox2.ValueMember = "id";
                //this.objects_placeBindingSource.Columns[0].Visible = false;
                // context.SaveChanges();




            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

    }
}
