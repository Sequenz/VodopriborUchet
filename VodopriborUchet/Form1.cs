using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            
/*          using (var context = new db_sqlceEntities())
          {
              foreach (var db in context.net.ToList())
                 
                {
                    Console.WriteLine(db.name);
                }
            var units = new units()
            {
                name = "test_rs12"
            };
            context.units.Add(units);
            context.SaveChanges();
            }*/

           

        }

        private void ресурсыToolStripMenuItem_Click(object sender, EventArgs e)
        {
           var edtResources = new EditResources();
           edtResources.ShowDialog(this);
        }

        private void объектыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var edtObjects = new EditObjects();
            edtObjects.ShowDialog(this);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
        private void Main_Load(object sender, EventArgs e)
        {
         treeView1.MouseDown += new MouseEventHandler(treeView1_MouseDown);
        }

        private void добавитьОбъектToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var addPlc = new AddPlace();
            addPlc.ShowDialog(this);
        }

        private void единицыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var edtUnits = new EditUnits();
            edtUnits.ShowDialog(this);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
      


    }
}
