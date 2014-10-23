using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class AddObject : Form
    {
        private EditObjects mEditObjects;
        public AddObject(EditObjects editObjects)
        {
            InitializeComponent();
            mEditObjects = editObjects;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text != "")
            {
                 using (var context = new db_sqlceEntities())
            {
             
            var objct = new object_type()
            {
                name = this.textBox1.Text,
                comment = this.textBox2.Text
            };
            context.object_type.Add(objct);
            context.SaveChanges();
            mEditObjects.LoadData();
            this.Close();
            }
                
            }
        }

    }
}
