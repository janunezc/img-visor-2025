using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Configuration;

namespace imgvisor_winfrm
{

    public partial class Form1 : Form
    {
        public string VERSION = "V.21.0612.0609";
        System.Windows.Forms.Timer wftimer;
        System.Windows.Forms.Timer minTimer;

        public Form1()
        {
            InitializeComponent();

            int intervalms = Int32.Parse(ConfigurationManager.AppSettings.Get("intervalms"));
            int minIntervalms = Int32.Parse(ConfigurationManager.AppSettings.Get("minintervalms"));

            WriteToFile("Service " + this.VERSION + " is started at " + DateTime.Now);

            wftimer = new System.Windows.Forms.Timer();
            wftimer.Interval = intervalms;
            wftimer.Tick += new EventHandler(timer_Tick);

            minTimer = new System.Windows.Forms.Timer();
            minTimer.Interval = minIntervalms;
            minTimer.Tick += new EventHandler(minimize);
            executeRoutines();
            wftimer.Start();
            minTimer.Start();
        }

        private bool firstTime = true;
        private void minimize(object sender, EventArgs e)
        {
            if (firstTime)
            {
                this.WindowState = FormWindowState.Normal;
                firstTime = false;
            }
            this.WindowState = FormWindowState.Minimized;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

            WriteToFile("Executing Routines...");
            executeRoutines();
            WriteToFile("Execution Complete!");

        }
        protected void Displaynotify()
        {
            try
            {
                notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath(@"image\graph.ico"));
                notifyIcon1.Text = "IMGVisor Control";
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "Bienvenido a Imagevisor";
                notifyIcon1.BalloonTipText = "Su sistema está protegido";
                notifyIcon1.ShowBalloonTip(100);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void executeRoutines()
        {
            try
            {
                for (int s = 1; s <= 10; s++)
                {
                    string configKey = "step_" + s;
                    string stepValue = "";
                    try
                    {
                        stepValue = ConfigurationManager.AppSettings.Get(configKey);
                    }
                    catch (Exception ex)
                    {

                    }

                    if (!String.IsNullOrEmpty(stepValue))
                    {
                        executeStep(stepValue);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToFile("Capture Exception!" + ex.Message);
            }
        }

        private void executeStep(string stepValue)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "CMD.exe",
                    Arguments = "/c " + stepValue,
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
            WriteToFile("Capture call returned!");

        }

        public void WriteToFile(string Message)
        {
            Message = DateTime.Now + " " + Message;

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

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Displaynotify();

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
        }
    }
}
