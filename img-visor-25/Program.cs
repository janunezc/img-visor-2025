using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace img_visor_25
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WriteToFile("--------------------------------");
            WriteToFile("Initiated!");
            captureAndCompare();
            vbsSteps();
            WriteToFile("Complete!");
            WriteToFile("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

        }

        private static void captureAndCompare()
        {
            WriteToFile("Capturing and comparing images...");
            String imagesFolder = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["imagesFolder"] ?? "");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            String fileFormat = ConfigurationManager.AppSettings["fileFormat"] ?? "yyyy_MM_dd_HH_mm_ss";
            String newFileName = $"sc_{DateTime.Now.ToString(fileFormat)}.jpg";
            String newFilePath = Path.Combine(imagesFolder, newFileName);
            int jpgResolution = Int32.Parse(ConfigurationManager.AppSettings["jpgResolution"] ?? "10");

            CaptureAllScreens(newFilePath, jpgResolution);

            String baselineImagePath = findPreviousImage(imagesFolder);

            if (baselineImagePath == String.Empty)
            {
                WriteToFile("No baseline image found. Skipping comparison.");
                return;
            }

            WriteToFile("Baseline image: '" + baselineImagePath + "'");

            long diff = ComparePixels(baselineImagePath, newFilePath, out int totalPixels);
            long diffPercentage = (diff * 100) / totalPixels;

            WriteToFile($"Difference: {diff} pixels ({diffPercentage}%)");
            WriteToFile("Compared pixels: " + totalPixels);

            if (diffPercentage < 30)
            {
                deleteImage(newFilePath);
            }


        }

        private static void deleteImage(string imagePath)
        {
            WriteToFile("Deleting image due to low difference: " + imagePath);
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

        private static String findPreviousImage(String imagesFolder)
        {
            WriteToFile("Finding previous image in folder: " + imagesFolder);
            var directoryInfo = new DirectoryInfo(imagesFolder);
            var lastFile = directoryInfo.GetFiles("*.jpg")
                                        .OrderByDescending(f => f.CreationTime)
                                        .Skip(1)
                                        .FirstOrDefault();
            return lastFile != null ? lastFile.FullName : string.Empty;
        }

        private static void vbsSteps()
        {
                WriteToFile("Executing VBS Steps...");
            string pwd = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings.Get("caller_scripts_pwd") ?? "");
            string winscp_caller = Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings.Get("winscp_caller") ?? "");

            executeStep(winscp_caller, pwd);
        }

        private static void executeStep(string stepValue, string pwd)
        {
            WriteToFile("Executing step: " + stepValue);

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
            WriteToFile("Sync Complete!");

        }

        public static void WriteToFile(string Message)
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

        static void CaptureAllScreens(string outPath, int jpegQuality)
        {
            try
            {
                WriteToFile("Capturing all screens to: " + outPath);
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
                WriteToFile("Comparing images: " + leftPath + " and " + rightPath);
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