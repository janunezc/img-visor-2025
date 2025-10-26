using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

internal static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        if (args.Length == 0 || args[0].Equals("--help", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  img-visor-capture --capture <outPath> [quality 1-100]");
            Console.WriteLine("  img-visor-capture --compare <baseImage> <newImage>");
            return 1;
        }

        try
        {
            switch (args[0])
            {
                case "--capture":
                    var outPath = args[1];
                    var quality = (args.Length > 2 && int.TryParse(args[2], out var q) && q is >= 1 and <= 100) ? q : 75;
                    CaptureAllScreens(outPath, quality);
                    return 0;

                case "--compare":
                    var baseImg = args[1];
                    var newImg = args[2];
                    var diff = ComparePixelCount(baseImg, newImg, out var total);
                    var pct = total > 0 ? 100.0 * diff / total : 0.0;
                    Console.WriteLine($"{diff} ({pct:F4}%)");
                    return 0;

                default:
                    Console.Error.WriteLine("Unknown command.");
                    return 1;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }
    }

    static void CaptureAllScreens(string outPath, int jpegQuality)
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

    static long ComparePixelCount(string leftPath, string rightPath, out int totalPixels)
    {
        using var left = new Bitmap(leftPath);
        using var right = new Bitmap(rightPath);
        if (left.Width != right.Width || left.Height != right.Height)
            throw new InvalidOperationException("Images must have same dimensions.");

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
}
