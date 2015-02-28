using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class AddImpulse : Form
    {
        private EditImpulse editImpulse;
        public AddImpulse(EditImpulse edtImpulse)
        {
            InitializeComponent();
            this.editImpulse = edtImpulse;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "" && this.textBox2.Text != "")
            {
                if (ValidateInt())
                {
                 
                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        var objct = new impulse()
                        {
                            weight = Convert.ToInt32(this.textBox1.Text),
                            code = Convert.ToInt32(this.textBox2.Text)

                        };
                        context.impulse.Add(objct);
                        context.SaveChanges();
                        editImpulse.LoadData();
                        this.Close();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        MessageBox.Show(ex.ToString());

                    }


                }

            }

            }
        }

        private bool ValidateInt()
        {
           string patternInt = @"^[0-9]";
           Regex regex = new Regex(patternInt);
           Match match1 = regex.Match(this.textBox1.Text);
           Match match2 = regex.Match(this.textBox2.Text);
           if (match1.Success && match2.Success)
               return true;
           MessageBox.Show("Допускаются только цифры!");
           return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
