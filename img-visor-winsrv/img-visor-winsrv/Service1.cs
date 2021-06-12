using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace img_visor_winsrv
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service V.1954 is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 1000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }

        private void capture()
        {
            try
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "CMD.exe",
                        Arguments = "/c E:\\crmk_audit_jn\\magick-capture.bat",
                        //Arguments = "/c E:\\crmk_audit_jn\\launch_file.vbs",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    string line = proc.StandardOutput.ReadLine();
                    WriteToFile("result:" + line);
                }
                Process.Start("E:\\crmk_audit_jn\\magick-capture.bat");
                WriteToFile("Capture call returned!");
            }
            catch (Exception ex)
            {
                WriteToFile("Capture Exception!" + ex.Message);
            }
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            timer.Interval = 15000;
            WriteToFile("Capturing Screenshot...");
            capture();
            WriteToFile("Capture call returned!");
        }
        public void WriteToFile(string Message)
        {
            Message = DateTime.Now + " "+ Message;

            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
