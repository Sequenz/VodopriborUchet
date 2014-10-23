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
    public partial class EditUnits : Form
    {
        public EditUnits()
        {
            InitializeComponent();
        }

        private void EditUnits_Load(object sender, EventArgs e)
        {
            var context = new db_sqlceEntities();
            try
            {
                var query = from c in context.units
                            select c;//new {c_name = c.name, c_ucost = c.net, a_name = c.kod_BTI};
                var results = query.ToList();
                this.unitsBindingSource.DataSource = results;
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
