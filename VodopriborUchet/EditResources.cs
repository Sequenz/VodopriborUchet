using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using VodopriborUchet.Properties;

namespace VodopriborUchet
{
    public partial class EditResources : Form
    {
        public EditResources()
        {
            InitializeComponent();
        }

        private void EditResources_Load(object sender, EventArgs e)
        {
           LoadData();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (resource_typeDataGridView.SelectedCells.Count > 0)
            {
                int i = (int)resource_typeDataGridView.CurrentRow.Cells[0].Value;
                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        var obPlc = context.resource_type.FirstOrDefault(p => p.id == i);
                        if (obPlc != null)
                        {
                            context.resource_type.Remove(obPlc);
                            context.SaveChanges();
                            LoadData();
                            //loadTree();
                        }


                    }
                    catch (EntitySqlException ex)
                    {

                        MessageBox.Show(ex.Message);
                    }

                }

            }
        }

        public void LoadData()
        {
            var context = new db_sqlceEntities();
            try
            {
                var query = from c in context.resource_type
                            select c;//new {c_name = c.name, c_ucost = c.net, a_name = c.kod_BTI};
                var results = query.ToList();
                this.resource_typeBindingSource.DataSource = results;
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
