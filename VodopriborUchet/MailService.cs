using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using Message = OpenPop.Mime.Message;
using Timer = System.Timers.Timer;

namespace VodopriborUchet
{
    internal class MailService
    {
        private string pop3_server;
        private int pop3_port;
        private bool use_ssl;
        private string mail;
        private string passwd;
        private string mail_ppu;

        private static Timer timer;
        private Log log = new Log();
        private static MailService mailService;

        private MailService()
        {

        }

        public static MailService GetInstance()
        {
            if (mailService == null)
            {
                lock (typeof (MailService))
                {
                    if (mailService == null)
                    {
                        mailService = new MailService();
                    }
                }
            }
            return mailService;
        }

        /* public MailService(string pop3s, int pop3p, bool ssl, string m, string pass, string mailppu)
        {
            this.pop3_server = pop3s;
            this.pop3_port = pop3p;
            this.use_ssl = ssl;
            this.mail = m;
            this.passwd = pass;
            this.mail_ppu = mailppu;
           // "pop.mail.ru", 995, true, "gdfgdfxxx@mail.ru", "dontask_mewhy", "test@mail.ru"
        }*/

        public void StartMailService()
        {
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    if (opt != null && opt.is_mail == true)
                    {
                        this.pop3_server = opt.pop_addr;
                        this.pop3_port = (int) opt.pop_port;
                        this.use_ssl = opt.pop_ssl;
                        this.mail = opt.pop_mail;
                        this.passwd = opt.pop_passwd;
                        this.mail_ppu = opt.ppu_mail;

                        if (timer != null) return;
                        timer = new Timer {Interval = (double) (opt.pop_int*60000)};
                           
                        timer.Elapsed += timerElapsed;
                        timer.Start();

                        Debug.WriteLine("MailService Started: " + DateTime.Now.TimeOfDay);
                    }
                }
                catch (SqlCeException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }



        }

        public void StopMailService()
        {
            if (timer == null) return;
            timer.Stop();
            timer.Close();
            Debug.WriteLine("MailService Stoped: " + DateTime.Now.TimeOfDay);

        }

        private void timerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Debug.WriteLine("The Elapsed event was raised at {0}", elapsedEventArgs.SignalTime);
            FetchAllMessages(pop3_server, pop3_port, use_ssl, mail, passwd, mail_ppu);
            //
        }

        private void FetchAllMessages(string hostname, int port, bool useSsl, string username,
            string password, string mail_ppu)
        {

            List<Message> newMessages = new List<Message>();
/*
            var test = FetchAllMessages("pop.mail.ru", 995, true, "gdfgdfxxx@mail.ru", "dontask_mewhy");
            //var ttt = test.First().MessagePart.Body.ToString();
            var ttt1 = test.First().FindFirstPlainTextVersion().GetBodyAsText();
            var ttt2 = test.First().Headers.Subject;
 */
            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                try
                {
                    // Connect to the server
                    client.Connect(hostname, port, useSsl);

                    // Authenticate ourselves towards the server
                    client.Authenticate(username, password);

                    // Get the number of messages in the inbox
                    int messageCount = client.GetMessageCount();

                    // We want to download all messages
                    List<Message> allMessages = new List<Message>(messageCount);
                    var mUID = getMessagesUIDFromBase(messageCount, client);
                    List<string> uids = client.GetMessageUids();




                    for (int i = 0; i < uids.Count; i++)
                    {
                        string currentUidOnServer = uids[i];
                        if (!mUID.Contains(currentUidOnServer))
                        {
                            // We have not seen this message before.
                            // Download it and add this new uid to seen uids

                            // the uids list is in messageNumber order - meaning that the first
                            // uid in the list has messageNumber of 1, and the second has 
                            // messageNumber 2. Therefore we can fetch the message using
                            // i + 1 since messageNumber should be in range [1, messageCount]
                            Message unseenMessage = client.GetMessage(i + 1);

                            // Add the message to the new messages
                            //newMessages.Add(unseenMessage);
                            LoadMassagesInDb(unseenMessage, client, i + 1);
                            // Add the uid to the seen uids, as it has now been seen
                            //seenUids.Add(currentUidOnServer);
                        }
                    }
                    //return allMessages;
                    // Only want to download message if:
                    //  - is from test@xample.com
                    //  - has subject "Some subject"
                    /*if (from.HasValidMailAddress && from.Address.Equals("mylostlenor@gmail.com"))
                        //&& "Some subject".Equals(subject)
                    {
                        // Messages are numbered in the interval: [1, messageCount]
                        // Ergo: message numbers are 1-based.
                        // Most servers give the latest message the highest number
                        for (int i = messageCount; i > 0; i--)
                        {
                            allMessages.Add(client.GetMessage(i));
                        }

                        // Now return the fetched messages
                        return allMessages;
                    }*/
                }
                catch (PopServerNotFoundException exception)
                {
                    StopMailService();
                    MessageBox.Show("Невозможно подключиться к серверу электронной почты, проверьте настройки.");
                }
                 catch (InvalidLoginException exception)
                {

                    MessageBox.Show("Неправильный логин или пароль для почты");
                }

                client.Disconnect();
                client.Dispose();
            }


        }

        private void LoadMassagesInDb(Message message, Pop3Client client, int i_msg)
        {
            string[] separator = new string[] {"\r\n", "\n"};

            MessageHeader headers = client.GetMessageHeaders(i_msg);
            RfcMailAddress from = headers.From;


            if (from.HasValidMailAddress && from.Address.Equals(this.mail_ppu))
            {

                if (message.Headers.Subject.Contains("_TEST"))
                {
                    Debug.WriteLine("MAIL_TS");
                    var mail_strings =
                        message.FindFirstPlainTextVersion()
                            .GetBodyAsText()
                            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Split(new[] {'='}));

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var item in mail_strings)
                    {
                        dict.Add(item[0], item[1]);
                    }

                    using (var context = new db_sqlceEntities())
                    {
                        try
                        {
                            var package_ts = new package_ts()
                                             {
                                                 pt = "/home/ts",
                                                 na = dict["na"],
                                                 r1 = Convert.ToInt32(dict["r1"]),
                                                 r2 = Convert.ToInt32(dict["r2"]),
                                                 pw1 = Convert.ToInt32(dict["pw1"]),
                                                 pw2 = Convert.ToInt32(dict["pw2"]),
                                                 gp = Convert.ToInt32(dict["gp"]),
                                                 te = Convert.ToInt32(dict["te"]),
                                                 vc = Convert.ToInt32(dict["vc"]),
                                                 vg = Convert.ToInt32(dict["vg"]),
                                                 ti = Convert.ToInt32(dict["ti"]),
                                                 sn1 = dict["sn1"],
                                                 sn2 = dict["sn2"],
                                                 mail_uid = client.GetMessageUid(i_msg),
                                                 ismail = true,
                                                 da = DateTime.ParseExact(dict["da"], "yyMMdd", null).Date,
                                                 date_time_inc = DateTime.Now
                                             };
                            context.package_ts.Add(package_ts);
                            context.SaveChanges();
                        }

                        catch (DbEntityValidationException ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }
                    }
                    log.Write(message.FindFirstPlainTextVersion()
                    .GetBodyAsText());
                }

                if (message.Headers.Subject.Contains("_TM"))
                {
                    Debug.WriteLine("MAIL_TM");
                    var mail_strings =
                        message.FindFirstPlainTextVersion()
                            .GetBodyAsText()
                            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Split(new[] {'='}));

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var item in mail_strings)
                    {
                        dict.Add(item[0], item[1]);
                    }

                    using (var context = new db_sqlceEntities())
                    {
                        try
                        {
                            var package_tm = new package_tm()
                                             {
                                                 pt = "/home/tm",
                                                 na = dict["na"],
                                                 ismail = true,
                                                 mail_uid = client.GetMessageUid(i_msg),
                                                 date_inc = DateTime.Now
                                             };
                            context.package_tm.Add(package_tm);
                            context.SaveChanges();
                        }

                        catch (DbEntityValidationException ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }
                    }
                }

                if (message.Headers.Subject.Contains("_ON"))
                {
                    Debug.WriteLine("MAIL_ON");

                    var mail_strings =
                        message.FindFirstPlainTextVersion()
                            .GetBodyAsText()
                            .Trim()
                            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Split(new[] {'='}));

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var item in mail_strings)
                    {
                        dict.Add(item[0], item[1]);
                    }

                    using (var context = new db_sqlceEntities())
                    {
                        try
                        {
                            var package_on = new package_on()
                                             {
                                                 pt = "/home/on",
                                                 na = dict["na"],
                                                 r1 = Convert.ToInt32(dict["r1"]),
                                                 r2 = Convert.ToInt32(dict["r2"]),
                                                 pw1 = Convert.ToInt32(dict["pw1"]),
                                                 pw2 = Convert.ToInt32(dict["pw2"]),
                                                 gp = Convert.ToInt32(dict["gp"]),
                                                 te = Convert.ToInt32(dict["te"]),
                                                 vc = Convert.ToInt32(dict["vc"]),
                                                 vg = Convert.ToInt32(dict["vg"]),
                                                 ti = Convert.ToInt32(dict["ti"]),
                                                 sn1 = dict["sn1"],
                                                 sn2 = dict["sn2"],
                                                 mail_uid = client.GetMessageUid(i_msg),
                                                 ismail = true,
                                                 da = DateTime.ParseExact(dict["da"], "yyMMdd", null).Date,
                                                 date_time_inc = DateTime.Now
                                             };
                            context.package_on.Add(package_on);
                            context.SaveChanges();
                        }

                        catch (DbEntityValidationException ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }
                    }
                    log.Write(message.FindFirstPlainTextVersion()
                    .GetBodyAsText());
                }

                if (message.Headers.Subject.Contains("_AL"))
                {
                    Debug.WriteLine("MAIL_AL");
                    var mail_strings =
                        message.FindFirstPlainTextVersion()
                            .GetBodyAsText()
                            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Split(new[] {'='}));

                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (var item in mail_strings)
                    {
                        dict.Add(item[0], item[1]);
                    }

                    var wr_txt = "";
                    var war = Convert.ToInt32(dict["w"]);
                    switch (war)
                    {
                        case (0):
                            wr_txt = "Вскрыте крышки!";
                            break;
                        case (1):
                            wr_txt = "Расход за час для первого счетчика больше предельного!";
                            break;
                        case (2):
                            wr_txt = "Расход за час для второго счетчика больше предельного!";
                            break;
                    }

                    using (var context = new db_sqlceEntities())
                    {
                        try
                        {
                            var package_al = new package_al()
                                             {
                                                 pt = "/home/wr",
                                                 na = dict["na"],
                                                 wr = war,
                                                 date_inc = DateTime.Now,
                                                 ismail = true,
                                                 mail_uid = client.GetMessageUid(i_msg),
                                                 wr_txt = wr_txt
                                             };
                            context.package_al.Add(package_al);
                            context.SaveChanges();
                        }

                        catch (DbEntityValidationException ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }
                    }
                }

                if (message.Headers.Subject.Contains("_Data"))
                {
                    Debug.WriteLine("MAIL_IZ");
                    var mail_strings =
                        message.FindFirstPlainTextVersion()
                            .GetBodyAsText()
                            .Trim()
                            .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Split(new[] {'='}));
                    Dictionary<string, string> dict = mail_strings.ToDictionary(item => item[0], item => item[1]);

                    

                    using (var context = new db_sqlceEntities())
                    {
                        try
                        {
                            string rw_1 = dict["r1"].Substring(0, 8);
                            string rw_2 = dict["r2"].Substring(0, 8);

                            string r1 = dict["r1"].Substring(8, 96);
                            string r2 = dict["r2"].Substring(8, 96);

                           // r1 = r1.Replace('g', '0');
                           // r2 = r2.Replace('g', '0');



                            r1 = Regex.Replace(r1, @"([^0-9])(\d){3}", "0000");
                            r2 = Regex.Replace(r2, @"([^0-9])(\d){3}", "0000");
                            IEnumerable<string> groups1 = Enumerable.Range(0, r1.Length/4)
                                .Select(i => r1.Substring(i*4, 4));
                            List<int> rw1 = new List<int>();
                            foreach (var grp in groups1)
                            {
                                rw1.Add(Convert.ToInt32(grp));
                            }

                            IEnumerable<string> groups2 = Enumerable.Range(0, r2.Length/4)
                                .Select(i => r2.Substring(i*4, 4));
                            List<int> rw2 = new List<int>();
                            foreach (var grp in groups2)
                            {
                                rw2.Add(Convert.ToInt32(grp));
                            }

                            string ppu_na = null;
                            if (!string.IsNullOrEmpty(dict["na"]))
                            {
                                ppu_na = dict["na"];

                            }
                            var package_iz = new package_iz()
                                             {
                                                 pt = "/home/iz",
                                                 na = dict["na"],
                                                 r1 = Convert.ToInt32(rw_1),
                                                 r1_0 = Convert.ToInt32(groups1.ElementAt(0)),
                                                 r1_1 = Convert.ToInt32(groups1.ElementAt(1)),
                                                 r1_2 = Convert.ToInt32(groups1.ElementAt(2)),
                                                 r1_3 = Convert.ToInt32(groups1.ElementAt(3)),
                                                 r1_4 = Convert.ToInt32(groups1.ElementAt(4)),
                                                 r1_5 = Convert.ToInt32(groups1.ElementAt(5)),
                                                 r1_6 = Convert.ToInt32(groups1.ElementAt(6)),
                                                 r1_7 = Convert.ToInt32(groups1.ElementAt(7)),
                                                 r1_8 = Convert.ToInt32(groups1.ElementAt(8)),
                                                 r1_9 = Convert.ToInt32(groups1.ElementAt(9)),
                                                 r1_10 = Convert.ToInt32(groups1.ElementAt(10)),
                                                 r1_11 = Convert.ToInt32(groups1.ElementAt(11)),
                                                 r1_12 = Convert.ToInt32(groups1.ElementAt(12)),
                                                 r1_13 = Convert.ToInt32(groups1.ElementAt(13)),
                                                 r1_14 = Convert.ToInt32(groups1.ElementAt(14)),
                                                 r1_15 = Convert.ToInt32(groups1.ElementAt(15)),
                                                 r1_16 = Convert.ToInt32(groups1.ElementAt(16)),
                                                 r1_17 = Convert.ToInt32(groups1.ElementAt(17)),
                                                 r1_18 = Convert.ToInt32(groups1.ElementAt(18)),
                                                 r1_19 = Convert.ToInt32(groups1.ElementAt(19)),
                                                 r1_20 = Convert.ToInt32(groups1.ElementAt(20)),
                                                 r1_21 = Convert.ToInt32(groups1.ElementAt(21)),
                                                 r1_22 = Convert.ToInt32(groups1.ElementAt(22)),
                                                 r1_23 = Convert.ToInt32(groups1.ElementAt(23)),
                                                 // r1_24 = Convert.ToInt32(groups1.ElementAt(24)),

                                                 r2 = Convert.ToInt32(rw_2),
                                                 r2_0 = Convert.ToInt32(groups2.ElementAt(0)),
                                                 r2_1 = Convert.ToInt32(groups2.ElementAt(1)),
                                                 r2_2 = Convert.ToInt32(groups2.ElementAt(2)),
                                                 r2_3 = Convert.ToInt32(groups2.ElementAt(3)),
                                                 r2_4 = Convert.ToInt32(groups2.ElementAt(4)),
                                                 r2_5 = Convert.ToInt32(groups2.ElementAt(5)),
                                                 r2_6 = Convert.ToInt32(groups2.ElementAt(6)),
                                                 r2_7 = Convert.ToInt32(groups2.ElementAt(7)),
                                                 r2_8 = Convert.ToInt32(groups2.ElementAt(8)),
                                                 r2_9 = Convert.ToInt32(groups2.ElementAt(9)),
                                                 r2_10 = Convert.ToInt32(groups2.ElementAt(10)),
                                                 r2_11 = Convert.ToInt32(groups2.ElementAt(11)),
                                                 r2_12 = Convert.ToInt32(groups2.ElementAt(12)),
                                                 r2_13 = Convert.ToInt32(groups2.ElementAt(13)),
                                                 r2_14 = Convert.ToInt32(groups2.ElementAt(14)),
                                                 r2_15 = Convert.ToInt32(groups2.ElementAt(15)),
                                                 r2_16 = Convert.ToInt32(groups2.ElementAt(16)),
                                                 r2_17 = Convert.ToInt32(groups2.ElementAt(17)),
                                                 r2_18 = Convert.ToInt32(groups2.ElementAt(18)),
                                                 r2_19 = Convert.ToInt32(groups2.ElementAt(19)),
                                                 r2_20 = Convert.ToInt32(groups2.ElementAt(20)),
                                                 r2_21 = Convert.ToInt32(groups2.ElementAt(21)),
                                                 r2_22 = Convert.ToInt32(groups2.ElementAt(22)),
                                                 r2_23 = Convert.ToInt32(groups2.ElementAt(23)),
                                                 //r2_24 = Convert.ToInt32(groups2.ElementAt(24)),

                                                 pw1 = Convert.ToInt32(dict["pw1"]),
                                                 pw2 = Convert.ToInt32(dict["pw2"]),
                                                 gp = Convert.ToInt32(dict["gp"]),
                                                 te = dict["te"],
                                                 vc = Convert.ToInt32(dict["vc"]),
                                                 vg = Convert.ToInt32(dict["vg"]),
                                                 ti = Convert.ToInt32(dict["ti"]),
                                                 sn1 = dict["sn1"],
                                                 sn2 = dict["sn2"],
                                                 da = DateTime.ParseExact(dict["da"], "yyMMdd", null).Date,
                                                 ismail = true,
                                                 mail_uid = client.GetMessageUid(i_msg),
                                                 date_inc = DateTime.Now
                                             };
                            context.package_iz.Add(package_iz);
                            context.SaveChanges();

                            var qu = context.counters.FirstOrDefault(c => c.serial_amspi == ppu_na);

                            if (qu != null)
                            {
                                qu.serial_r1 = dict["sn1"];
                                qu.serial_r2 = dict["sn2"];
                                context.SaveChanges();

                            }
                        }

                        catch (DbEntityValidationException ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.ToString());
                        }
                    }
                    log.Write(message.FindFirstPlainTextVersion()
                    .GetBodyAsText());
                }
                
            }

        }

        private List<string> getMessagesUIDFromBase(int c, Pop3Client client)
        {
            List<string> uids = new List<string>();
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    uids.AddRange(from al in context.package_al
                        where al.mail_uid != null
                        select al.mail_uid);
                    uids.AddRange(from al in context.package_iz
                        where al.mail_uid != null
                        select al.mail_uid);
                    uids.AddRange(from al in context.package_on
                        where al.mail_uid != null
                        select al.mail_uid);
                    uids.AddRange(from al in context.package_tm
                        where al.mail_uid != null
                        select al.mail_uid);
                    uids.AddRange(from al in context.package_ts
                        where al.mail_uid != null
                        select al.mail_uid);

                }
                catch (SqlCeException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return uids;


           
        }


    }
}


