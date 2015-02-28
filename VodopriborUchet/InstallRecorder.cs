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
    public partial class InstallRecorder : Form
    {
        private int _treeNodeId ;
        private string _treeNodeName;
        public InstallRecorder(int treeNodeId, string treeNodeName)
        {
            InitializeComponent();
            this._treeNodeId = treeNodeId;
            this._treeNodeName = treeNodeName;
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

                    var q = context.counters.FirstOrDefault(c => c.serial_amspi == textBox2.Text);
                    if (q == null)
                    {
                     
                    var cnt = new counters()
                    {
                       objects_place_id = _treeNodeId,
                       serial_amspi = textBox2.Text,
                       date_init_calibration_r1 = dateTimePicker1.Value.Date,
                       date_init_calibration_r2 = dateTimePicker2.Value.Date,
                       date_calibration_r1 = dateTimePicker3.Value.Date,
                       date_calibration_r2 = dateTimePicker4.Value.Date,
                       date_calibration_amspi = dateTimePicker5.Value.Date,
                       diameter_r1 = Convert.ToInt32(textBox5.Text),
                       diameter_r2 = Convert.ToInt32(textBox6.Text),
                       date_start = dateTimePicker6.Value.Date
                    };
                    context.counters.Add(cnt);
                    context.SaveChanges();
                    this.Close();

                    }
                    else
                    {
                        MessageBox.Show("ППУ-РМ с таким серийным номером уже установлен!");
                    }
                }

            }
        }

        private void InstallRecorder_Load(object sender, EventArgs e)
        {
            textBox1.Text = _treeNodeName;
        }
    }
}
