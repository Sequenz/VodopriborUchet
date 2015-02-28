using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class AddOwner : Form
    {
        private EditOwners mEditOwners;
        public AddOwner(EditOwners editOwners)
        {
            InitializeComponent();
            this.mEditOwners = editOwners;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "" && this.textBox2.Text != "" && this.textBox3.Text != "")
            {
                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        var objct = new owners()
                        {
                            name = this.textBox1.Text,
                            surname = this.textBox2.Text,
                            patronymic = this.textBox3.Text,
                            tel = this.textBox4.Text,
                            category_pay_id = (int?) this.comboBox1.SelectedValue

                        };
                        context.owners.Add(objct);
                        context.SaveChanges();
                        mEditOwners.LoadData();
                        this.Close();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        MessageBox.Show(ex.ToString());

                    }

                   
                }

            }
        }

        private void AddOwner_Load(object sender, EventArgs e)
        {
            var context = new db_sqlceEntities();
            try
            {
                var query = from c in context.category_pay_type
                            select new
                            {
                                c.id,
                                c.name //new {c_name = c.name, c_ucost = c.net, a_name = c.kod_BTI};
                            };
                var results = query.ToList();
                this.comboBox1.DataSource = results;
                this.comboBox1.DisplayMember = "name";
                this.comboBox1.ValueMember = "id";
                
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
