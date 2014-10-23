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
    public partial class EditObjects : Form
    {
        public EditObjects()
        {
            InitializeComponent();
        }

        private void EditObjects_Load(object sender, EventArgs e)
        {
           LoadData();
        }

        public void LoadData()
        {
            var context = new db_sqlceEntities();
            try
            {
                var query = from c in context.object_type
                            select c;//new {c_name = c.name, c_ucost = c.net, a_name = c.kod_BTI};
                var results = query.ToList();
                this.object_typeBindingSource.DataSource = results;
                //this.objects_placeBindingSource.Columns[0].Visible = false;
                // context.SaveChanges();


            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            var addObj = new AddObject(this);
            addObj.ShowDialog();     
        }
    }
}
