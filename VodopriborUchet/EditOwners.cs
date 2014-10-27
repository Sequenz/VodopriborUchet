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
    public partial class EditOwners : Form
    {
        public EditOwners()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            var context = new db_sqlceEntities();
            try
            {
                var query = from c in context.owners
                            select c;
                var results = query.ToList();
                this.ownersBindingSource.DataSource = results;
                


            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void EditOwners_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            var addOwner = new AddOwner(this);
            addOwner.ShowDialog();  
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (ownersDataGridView.SelectedCells.Count > 0)
            {
                int i = (int)ownersDataGridView.CurrentRow.Cells[0].Value;
                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        var obPlc = context.owners.FirstOrDefault(p => p.id == i);
                        if (obPlc != null)
                        {
                            context.owners.Remove(obPlc);
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

    }
}
