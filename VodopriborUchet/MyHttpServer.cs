using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace VodopriborUchet
{


    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();
        
        
        private Log log = new Log();
        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }


        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];


            var spaceIndex = http_url.IndexOf("%20", System.StringComparison.Ordinal);
            string s;
            s = spaceIndex != -1 ? http_url.Substring(0, spaceIndex) : http_url;

            var url= new Uri("http://www.example.com" + s);
            var url_path = url.AbsolutePath;
            NameValueCollection qscollection = HttpUtility.ParseQueryString(url.Query);
            string ppu_na = null;
            if (!string.IsNullOrEmpty(qscollection["na"]))
            {
                ppu_na = qscollection["na"];
                
            }
            //var pac = qscollection.Get(0).ToLower();
           /* switch (qscollection.Get(0))
            {*/
            if (url_path.IndexOf("/home/ts") > -1)
            {

                Debug.WriteLine("ts");
                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        var package_ts = new package_ts()
                                         {
                                             pt = url_path,
                                             na = qscollection["na"],
                                             r1 = Convert.ToInt32(qscollection["r1"]),
                                             r2 = Convert.ToInt32(qscollection["r2"]),
                                             pw1 = Convert.ToInt32(qscollection["pw1"]),
                                             pw2 = Convert.ToInt32(qscollection["pw2"]),
                                             gp = Convert.ToInt32(qscollection["gp"]),
                                             te = Convert.ToInt32(qscollection["te"]),
                                             vc = Convert.ToInt32(qscollection["vc"]),
                                             vg = Convert.ToInt32(qscollection["vg"]),
                                             ti = Convert.ToInt32(qscollection["ti"]),
                                             sn1 = qscollection["sn1"],
                                             sn2 = qscollection["sn2"],
                                             ismail = false,
                                             da = DateTime.ParseExact(qscollection["da"], "yyMMdd", null).Date,
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
            }

            if (url_path.IndexOf("/home/iz") > -1)
            {
                Debug.WriteLine("iz");

                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        string rw_1 = qscollection["r1"].Substring(0, 8);
                        string rw_2 = qscollection["r2"].Substring(0, 8);

                        string r1 = qscollection["r1"].Substring(8, 96);
                        string r2 = qscollection["r2"].Substring(8, 96);

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

                        var package_iz = new package_iz()
                                         {
                                             pt = url_path,
                                             na = qscollection["na"],
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

                                             pw1 = Convert.ToInt32(qscollection["pw1"]),
                                             pw2 = Convert.ToInt32(qscollection["pw2"]),
                                             gp = Convert.ToInt32(qscollection["gp"]),
                                             te = qscollection["te"],
                                             vc = Convert.ToInt32(qscollection["vc"]),
                                             vg = Convert.ToInt32(qscollection["vg"]),
                                             ti = Convert.ToInt32(qscollection["ti"]),
                                             sn1 = qscollection["sn1"],
                                             sn2 = qscollection["sn2"],
                                             ismail = false,
                                             da = DateTime.ParseExact(qscollection["da"], "yyMMdd", null).Date,
                                             date_inc = DateTime.Now
                                         };
                        context.package_iz.Add(package_iz);
                        context.SaveChanges();

                        var que = context.counters.FirstOrDefault(c => c.serial_amspi == ppu_na);

                        if (que != null)
                        {
                            que.serial_r1 = qscollection["sn1"];
                            que.serial_r2 = qscollection["sn2"];
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
            }
            if (url_path.IndexOf("/home/tm") > -1)
            {
                Debug.WriteLine("tm");
                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        var package_tm = new package_tm()
                                         {
                                             pt = url_path,
                                             na = qscollection["na"],
                                             ismail = false,
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
            if (url_path.IndexOf("/home/on") > -1)
            {
                Debug.WriteLine("on");
                using (var context = new db_sqlceEntities())
                {
                    try
                    {
                        var package_on = new package_on()
                                         {
                                             pt = url_path,
                                             na = qscollection["na"],
                                             r1 = Convert.ToInt32(qscollection["r1"]),
                                             r2 = Convert.ToInt32(qscollection["r2"]),
                                             pw1 = Convert.ToInt32(qscollection["pw1"]),
                                             pw2 = Convert.ToInt32(qscollection["pw2"]),
                                             gp = Convert.ToInt32(qscollection["gp"]),
                                             te = Convert.ToInt32(qscollection["te"]),
                                             vc = Convert.ToInt32(qscollection["vc"]),
                                             vg = Convert.ToInt32(qscollection["vg"]),
                                             ti = Convert.ToInt32(qscollection["ti"]),
                                             sn1 = qscollection["sn1"],
                                             sn2 = qscollection["sn2"],
                                             ismail = false,
                                             da = DateTime.ParseExact(qscollection["da"], "yyMMdd", null).Date,
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
            }
            if (url_path.IndexOf("/home/al") > -1)
            {
                Debug.WriteLine("al");
                var wr_txt = "";
                var war = Convert.ToInt32(qscollection.Get(2));
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
                                             pt = url_path,
                                             na = qscollection["na"],
                                             wr = war,
                                             date_inc = DateTime.Now,
                                             ismail = false,
                                             wr_txt = wr_txt
                                         };
                        context.package_al.Add(package_al);
                        context.SaveChanges();

                        var query1 = from c in context.objects_place
                            from cou in context.counters.Where(cou => cou.objects_place_id == c.id).DefaultIfEmpty()
                            where cou.serial_amspi == package_al.na
                            select new
                                   {
                                       c.name
                                   };
                        string rr = query1.FirstOrDefault().name;

                        MessageBox.Show("ППУ-РМ №: " + package_al.na + " " + rr + " - " + package_al.wr_txt, "АВАРИЯ!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.ServiceNotification);

                    }

                    catch (DbEntityValidationException ex)
                    {

                        MessageBox.Show(ex.ToString());
                    }
                }
            }
           
            
            log.Write(http_url);
           
        }

        public void readHeaders()
        {
            Console.WriteLine("readHeaders()");
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

           /* Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            srv.handlePOSTRequest(this, new StreamReader(ms));*/

        }

        public void writeSuccess(string content_type = "text/html")
        {
            outputStream.WriteLine("HTTP/1.1 200 OK");
            outputStream.WriteLine("Content-Type: text/html; charset=utf-8"); //  + content_type  Ключ	Значение Content-Type	text/html; charset=utf-8
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("Date: " + DateTime.Now);
            outputStream.WriteLine("Content-Length: 13");
            outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.1 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }
    }

    public abstract class HttpServer
    {
        
        protected int port;
        TcpListener listener;
        public bool is_active = true;

       

        public HttpServer(int port)
        {
            this.port = port;
        }



        private string getExternalIp()
        {
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(externalIP)[0].ToString();
                return externalIP;
            }
            catch { return null; }
        }

        public void listen()
        {
            string ip_adr = "127.0.0.1";
           
            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    ip_adr = opt.ip_addr;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

           /* IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip_adr = ip.ToString();
                }
            }*/

            IPAddress localAddr = IPAddress.Parse(ip_adr);
            try
            {
                listener = new TcpListener(localAddr, port);
                listener.Start();
                while (is_active)
                {
                    Debug.WriteLine("HTTPServer listener thread start!");
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    Thread thread = new Thread(new ThreadStart(processor.process));
                    thread.IsBackground = true;
                    thread.Start();
                    Thread.Sleep(1);
                }
                //listener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Указан неверный ip-адрес для сервера!"); 
            }
           
        }

       
        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    public class MyHttpServer : HttpServer
    {
        public MyHttpServer(int port)
            : base(port)
        {
        }

        
        public override void handleGETRequest(HttpProcessor p)
        {

            using (var context = new db_sqlceEntities())
            {
                try
                {
                    var opt = context.options.FirstOrDefault();
                    if (opt.new_ip)
                    {
                        Console.WriteLine("request: {0}", p.http_url);
                        p.writeSuccess();
                        p.outputStream.WriteLine("T##" + DateTime.Now.ToString("yyMMddHHmm") + "YIP=" + opt.new_ip_addr);
                    }
                    else
                    {
                        Console.WriteLine("request: {0}", p.http_url);
                        p.writeSuccess();
                        p.outputStream.Write("T##" + DateTime.Now.ToString("yyMMddHHmm"));
                    }
                }
                catch (SqlCeException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

           
            /*p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("url : {0}", p.http_url);*/
          
         /*   p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
            p.outputStream.WriteLine("</form>");*/
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {/*
            Console.WriteLine("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();

            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);*/


        }
    }


}
