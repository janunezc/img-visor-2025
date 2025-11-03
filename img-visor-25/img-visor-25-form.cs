using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace img_visor_25
{
    public partial class img_visor_25_form : Form
    {

        public string VERSION = "V.21.0612.0609";
        readonly System.Windows.Forms.Timer wftimer;
        readonly System.Windows.Forms.Timer minTimer;

        public img_visor_25_form()
        {
            InitializeComponent();
            statusStrip.Text = "Welcome! System is inactive.";

            string? intervalmsStr = ConfigurationManager.AppSettings.Get("intervalms");
            string? minIntervalmsStr = ConfigurationManager.AppSettings.Get("minintervalms");

            if (string.IsNullOrEmpty(intervalmsStr) || string.IsNullOrEmpty(minIntervalmsStr))
            {
                throw new ConfigurationErrorsException("Required appSettings 'intervalms' or 'minintervalms' is missing or empty.");
            }

            int intervalms = Int32.Parse(intervalmsStr);
            int minIntervalms = Int32.Parse(minIntervalmsStr);

            WriteToFile("Service " + this.VERSION + " is started at " + DateTime.Now);

            wftimer = new System.Windows.Forms.Timer();
            wftimer.Interval = intervalms;
            wftimer.Tick += new EventHandler(timer_Tick);

            minTimer = new System.Windows.Forms.Timer();
            minTimer.Interval = minIntervalms;
            minTimer.Tick += new EventHandler(minimize);
            statusStrip.Text = "Executing Routines...";
            //executeRoutines();
            captureAndCompare();
            vbsSteps();
            statusStrip.Text = "On Timers...";
            wftimer.Start();
            minTimer.Start();
        }

        private bool firstTime = true;
        private void minimize(object? sender, EventArgs e)
        {
            if (firstTime)
            {
                this.WindowState = FormWindowState.Normal;
                firstTime = false;
            }
            this.WindowState = FormWindowState.Minimized;
        }
        private void timer_Tick(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

            WriteToFile("Executing Routines...");
            captureAndCompare();
            vbsSteps();
            WriteToFile("Execution Complete!");

        }
        protected void Displaynotify()
        {
            try
            {
                notifyIcon.Icon = new System.Drawing.Icon(Path.GetFullPath(@"image\graph.ico"));
                notifyIcon.Text = "IMGVisor Control";
                notifyIcon.Visible = false;
                notifyIcon.BalloonTipTitle = "Bienvenido a Imagevisor";
                notifyIcon.BalloonTipText = "Su sistema está protegido";
                notifyIcon.ShowBalloonTip(100);
            }
            catch (Exception ex)
            {
                WriteToFile("NotifyIcon Exception!" + ex.Message);
            }
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle |= CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }


        private void captureAndCompare()
        {
            String imagesFolder = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["imagesFolder"] ?? "");
            String fileFormat = ConfigurationManager.AppSettings["fileFormat"] ?? "yyyy_MM_dd_HH_mm_ss";
            String newFileName = $"{DateTime.Now.ToString(fileFormat)}.jpg";
            String newFilePath = Path.Combine(imagesFolder, newFileName);
            int jpgResolution = Int32.Parse(ConfigurationManager.AppSettings["jpgResolution"] ?? "10");

            CaptureAllScreens(newFilePath, jpgResolution);

            String baselineImagePath = findPreviousImage(imagesFolder);

            if (baselineImagePath == String.Empty)
            {
                WriteToFile("No baseline image found. Skipping comparison.");
                return;
            }

            long diff = ComparePixels(baselineImagePath, newFilePath, out int totalPixels);
            long diffPercentage = (diff * 100) / totalPixels;
            WriteToFile($"Difference: {diff} pixels ({diffPercentage}%)");
            WriteToFile("Compared pixels: " + totalPixels);

            if (diffPercentage < 30)
            {
                deleteImage(newFilePath);
            }


        }

        private void deleteImage(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                    WriteToFile("Deleted image: " + imagePath);
                }
                else
                {
                    WriteToFile("Image not found for deletion: " + imagePath);
                }
            }
            catch (Exception ex)
            {
                WriteToFile("Error deleting image: " + ex.Message);
            }
        }

        private String findPreviousImage(String imagesFolder)
        {
            var directoryInfo = new DirectoryInfo(imagesFolder);
            var lastFile = directoryInfo.GetFiles("*.jpg")
                                        .OrderByDescending(f => f.CreationTime)
                                        .Skip(1)
                                        .FirstOrDefault();
            return lastFile != null ? lastFile.FullName : string.Empty;
        }

        private void vbsSteps()
        {
            string pwd = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings.Get("caller_scripts_pwd") ?? "");
            string winscp_caller = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings.Get("winscp_caller") ?? "");

            executeStep(winscp_caller, pwd);
        }

        private void executeStep(string stepValue, string pwd)
        {


            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + stepValue,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = pwd
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string? line = proc.StandardOutput.ReadLine();
                if (line != null)
                {
                    WriteToFile("result:" + line);
                }
            }
            WriteToFile("Capture call returned!");

        }

        public void WriteToFile(string Message)
        {

            string basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "img-visor",
                "logs"
            );
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            Message = DateTime.Now + " " + Message;

            string filepath = Path.Combine(
                basePath,
                $"ServiceLog_{DateTime.Now:yyyy_MM_dd}.txt"
            );

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
            Displaynotify();

        }

        private void notifyIcon_Click(object sender, EventArgs e)
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
            notifyIcon.Visible = false;
        }


        static void CaptureAllScreens(string outPath, int jpegQuality)
        {
            try
            {
                var v = SystemInformation.VirtualScreen;
                using var bmp = new Bitmap(v.Width, v.Height, PixelFormat.Format24bppRgb);
                using var g = Graphics.FromImage(bmp);
                g.CopyFromScreen(v.X, v.Y, 0, 0, v.Size);

                var enc = ImageCodecInfo.GetImageEncoders().First(e => e.FormatID == ImageFormat.Jpeg.Guid);
                using var ps = new EncoderParameters(1);
                ps.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);
                bmp.Save(outPath, enc, ps);
                Console.WriteLine($"Saved: {outPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error capturing screens: " + ex.Message);
                return;
            }

        }

        static long ComparePixels(string leftPath, string rightPath, out int totalPixels)
        {
            try
            {
                using var left = new Bitmap(leftPath);
                using var right = new Bitmap(rightPath);
                if (left.Width != right.Width || left.Height != right.Height)
                    Console.WriteLine("Images have different dimensions.");

                int w = left.Width, h = left.Height;
                totalPixels = w * h;
                long diff = 0;

                // Simple, clear version—good enough for a first pass
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                        if (left.GetPixel(x, y).ToArgb() != right.GetPixel(x, y).ToArgb())
                            diff++;

                return diff;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error comparing images: " + ex.Message);
                totalPixels = 1024 * 768;
                long pixelsToReturn = 1024 * 768;

                return pixelsToReturn;
            }
        }
    }
}
