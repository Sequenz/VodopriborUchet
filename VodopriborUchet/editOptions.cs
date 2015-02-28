using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace VodopriborUchet
{
    public partial class editOptions : Form
    {
        private HttpServer httpServer;

        public editOptions()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                this.textBox1.Enabled = true;
                this.textBox2.Enabled = true;
                this.checkBox4.Enabled = true;
            }
            else
            {
                this.textBox1.Enabled = false;
                this.textBox2.Enabled = false;
                this.checkBox4.Enabled = false;

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
                this.textBox5.Enabled = true;
                this.textBox6.Enabled = true;
                this.textBox7.Enabled = true;
                this.checkBox3.Enabled = true;
                this.numericUpDown1.Enabled = true;
            }
            else
            {
                this.textBox3.Enabled = false;
                this.textBox4.Enabled = false;
                this.textBox5.Enabled = false;
                this.textBox6.Enabled = false;
                this.textBox7.Enabled = false;
                this.checkBox3.Enabled = false;
                this.numericUpDown1.Enabled = false;
            }
        }
        private void MailService()
        {
            MailService mailService = VodopriborUchet.MailService.GetInstance();
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    if (opt != null)
                    {
                        if (opt.is_mail == true)
                        {
                            mailService.StartMailService();
                        }
                        if (opt.is_mail == false)
                        {
                            mailService.StopMailService();
                        }

                    }

                }
                catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }

        }

        private bool validateFields()
        {
                if (validateIP() && validateEmail() && validateEmailPPU() && validateIPNew() && validatePOPIP() && validatePOPPort() && validatePort())
                {
                    return true;
                }
            return false;


        }

        private bool validateIP()
        {
            if (this.checkBox1.Checked)
            {
                string patternIP = @"((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)";
                Regex regex = new Regex(patternIP);
                Match match = regex.Match(this.textBox1.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный ip адрес");
                return false;
            }
            return true;
        }

        private bool validatePort()
        {
            if (this.checkBox1.Checked)
            {
                string patternPort = @"^[0-9]";
                Regex regex = new Regex(patternPort);
                Match match = regex.Match(this.textBox2.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный порт для ip адреса");
                return false;
            }
            return true;
        }
        private bool validateEmail()
        {
            if (this.checkBox2.Checked)
            {
                string patternPort = @"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$";
                Regex regex = new Regex(patternPort);
                Match match = regex.Match(this.textBox5.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный E-mail");
                return false;
            }
            return true;
        }
        private bool validatePasswd()
        {
            if (this.checkBox2.Checked)
            {
                string patternPort = @"(?=^.{4,}$)((?=.*\d)|(?=.*\W+))(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$";
                Regex regex = new Regex(patternPort);
                Match match = regex.Match(this.textBox6.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный пароль для почты");
                return false;
            }
            return true;
        }
        private bool validateEmailPPU()
        {
            if (this.checkBox2.Checked)
            {
                string patternPort = @"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$";
                Regex regex = new Regex(patternPort);
                Match match = regex.Match(this.textBox7.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный E-mail ППУ-РМ");
                return false;
            }
            return true;
        }
        private bool validateIPNew()
        {
            if (this.checkBox1.Checked && this.checkBox4.Checked)
            {
                string patternPort = @"((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)";
                Regex regex = new Regex(patternPort);
                Match match = regex.Match(this.textBox8.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный новый ip-адрес");
                return false;
            }
            return true;
        }
        private bool validatePOPIP()
        {
            if (this.checkBox2.Checked)
            {
                string patternIP = @"^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$";
                Regex regex = new Regex(patternIP);
                Match match = regex.Match(this.textBox3.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный адрес POP сервера");
                return false;
            }
            return true;
        }

        private bool validatePOPPort()
        {
            if (this.checkBox2.Checked)
            {
                string patternPort = @"^[0-9]";
                Regex regex = new Regex(patternPort);
                Match match = regex.Match(this.textBox4.Text);
                if (match.Success)
                    return true;
                MessageBox.Show("Укажите корректный порт для POP сервера");
                return false;
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (validateFields())
            {
                
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    if (opt != null)
                    {
                        opt.ip_addr = this.textBox1.Text;
                        opt.ip_port = Convert.ToInt32(this.textBox2.Text);
                        opt.pop_addr = this.textBox3.Text.Trim();
                        opt.pop_port =  Convert.ToInt32(this.textBox4.Text);
                        opt.pop_mail = this.textBox5.Text.Trim(); ;
                        opt.pop_passwd = this.textBox6.Text.Trim();
                        opt.pop_ssl = this.checkBox3.Checked;
                        opt.ppu_mail = this.textBox7.Text.Trim();
                        opt.pop_int =  Convert.ToInt32(this.numericUpDown1.Value);
                        opt.is_ip = this.checkBox1.Checked;
                        opt.is_mail = this.checkBox2.Checked;
                        opt.new_ip = checkBox4.Checked;
                        opt.new_ip_addr = this.textBox8.Text;

                        context.SaveChanges();
                        this.Close();
                    }
                    
                    else
                    {
                        var opti = new options()
                                  {
                                        ip_addr = this.textBox1.Text,
                                        ip_port = Convert.ToInt32(this.textBox2.Text),
                                        pop_addr = this.textBox3.Text,
                                        pop_port =  Convert.ToInt32(this.textBox4.Text),
                                        pop_mail = this.textBox5.Text,
                                        pop_passwd = this.textBox6.Text,
                                        pop_ssl = this.checkBox3.Checked,
                                        ppu_mail = this.textBox7.Text,
                                        pop_int =  Convert.ToInt32(this.numericUpDown1.Value),
                                        is_ip = this.checkBox1.Checked,
                                        is_mail = this.checkBox2.Checked,
                                        new_ip = checkBox4.Checked,
                                        new_ip_addr = this.textBox8.Text

                                  };

                    context.options.Add(opti);
                    context.SaveChanges();
                    
                    this.Close();
                    }
                    MailService();
                }
               catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }

            }  
        }

        private void editOptions_Load(object sender, EventArgs e)
        {
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    if (opt != null)
                    {
                        this.textBox1.Text = opt.ip_addr;
                        this.textBox2.Text = opt.ip_port.ToString();
                        this.textBox3.Text = opt.pop_addr;
                        this.textBox4.Text = opt.pop_port.ToString();
                        this.textBox5.Text = opt.pop_mail;
                        this.textBox6.Text = opt.pop_passwd;
                        this.checkBox3.Checked = opt.pop_ssl;
                        this.textBox7.Text = opt.ppu_mail;
                        this.numericUpDown1.Value = (decimal)opt.pop_int;
                        this.checkBox1.Checked = opt.is_ip;
                        this.checkBox2.Checked = opt.is_mail;
                        this.checkBox4.Checked = (bool) opt.new_ip;
                        this.textBox8.Text = opt.new_ip_addr;
                    }
                    /*else
                    {
                     *  opt.ip_addr = "test";
                        context.options.Attach(opt);
                        context.Entry(opt).State = EntityState.Modified;
                        context.SaveChanges();
                        opt.ip_addr = "test";
                        opt.is_ip = false;
                        opt.is_mail = false;
                        context.options.Add(opt);
                        context.SaveChanges();

                    }*/
                }
                catch (EntitySqlException ex)
                {

                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            this.textBox8.Enabled = checkBox4.Checked;
        }
    }
}
