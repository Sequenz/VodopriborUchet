using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class WarningConditions : Form
    {
        public WarningConditions()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WarningConditions_Load(object sender, EventArgs e)
        {
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var war = context.warning_conditions.FirstOrDefault();
                    if (war != null)
                    {
                        this.gprs_sig_min.Text = war.gprs_sig_min.ToString();
                        this.gprs_sig_max.Text = war.gprs_sig_max.ToString();
                        this.gprs_mod_min.Text = war.gprs_mod_min.ToString();
                        this.gprs_mod_max.Text = war.gprs_mod_max.ToString();
                        this.cont_min.Text = war.controller_voltage_min.ToString();
                        this.cont_max.Text = war.controller_voltage_max.ToString();
                    }
                }
                catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var war = context.warning_conditions.FirstOrDefault();
                    if (war != null)
                    {
                        war.gprs_sig_min = float.Parse(this.gprs_sig_min.Text, CultureInfo.InvariantCulture.NumberFormat);
                        war.gprs_sig_max = float.Parse(this.gprs_sig_max.Text, CultureInfo.InvariantCulture.NumberFormat);
                        war.gprs_mod_min = float.Parse(this.gprs_mod_min.Text, CultureInfo.InvariantCulture.NumberFormat);
                        war.gprs_mod_max = float.Parse(this.gprs_mod_max.Text, CultureInfo.InvariantCulture.NumberFormat);
                        war.controller_voltage_min = float.Parse(this.cont_min.Text, CultureInfo.InvariantCulture.NumberFormat);
                        war.controller_voltage_max = float.Parse(this.cont_max.Text, CultureInfo.InvariantCulture.NumberFormat);
                        context.SaveChanges();
                        this.Close();
                    }
                    else
                    {
                        var wr = new warning_conditions()
                                 {
                                     gprs_sig_min =
                                         float.Parse(this.gprs_sig_min.Text, CultureInfo.InvariantCulture.NumberFormat),
                                     gprs_sig_max =
                                         float.Parse(this.gprs_sig_max.Text, CultureInfo.InvariantCulture.NumberFormat),
                                     gprs_mod_min =
                                         float.Parse(this.gprs_mod_min.Text, CultureInfo.InvariantCulture.NumberFormat),
                                     gprs_mod_max =
                                         float.Parse(this.gprs_mod_max.Text, CultureInfo.InvariantCulture.NumberFormat),
                                     controller_voltage_min =
                                         float.Parse(this.cont_min.Text, CultureInfo.InvariantCulture.NumberFormat),
                                     controller_voltage_max =
                                         float.Parse(this.cont_max.Text, CultureInfo.InvariantCulture.NumberFormat)

                                 };
                        context.warning_conditions.Add(wr);
                        context.SaveChanges();

                        this.Close();
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
