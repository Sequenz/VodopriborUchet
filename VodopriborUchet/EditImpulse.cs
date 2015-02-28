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
    public partial class EditImpulse : Form
    {
        public EditImpulse()
        {
            InitializeComponent();
        }

        private void EditImpulse_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        public void LoadData()
        {
            var context = new db_sqlceEntities();
            try
            {
                var query = from c in context.impulse
                            select c;
                var results = query.ToList();
                this.impulseBindingSource.DataSource = results;

            }
            catch (EntitySqlException ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            var addImp = new AddImpulse(this);
            addImp.ShowDialog();
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (impulseDataGridView.SelectedCells.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Вы действительно хотите удалить выбранный пункт?",
                    "Подтвердите удаление!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    int i = (int)impulseDataGridView.CurrentRow.Cells[0].Value;
                    using (var context = new db_sqlceEntities())
                    {
                        try
                        {
                            var obPlc = context.impulse.FirstOrDefault(p => p.Id == i);
                            if (obPlc != null)
                            {
                                context.impulse.Remove(obPlc);
                                context.SaveChanges();
                                LoadData();
                               
                            }


                        }
                        catch (EntitySqlException ex)
                        {

                            MessageBox.Show(ex.Message);
                        }

                    }

                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            if (impulseDataGridView.SelectedCells.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Вы действительно хотите удалить выбранный пункт?",
                    "Подтвердите удаление!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    int i = (int)impulseDataGridView.CurrentRow.Cells[0].Value;
                    using (var context = new db_sqlceEntities())
                    {
                        try
                        {
                            var obPlc = context.impulse.FirstOrDefault(p => p.Id == i);
                            if (obPlc != null)
                            {
                                context.impulse.Remove(obPlc);
                                context.SaveChanges();
                                LoadData();
                            }


                        }
                        catch (EntitySqlException ex)
                        {

                            MessageBox.Show(ex.Message);
                        }

                    }

                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
        }
    }
}
