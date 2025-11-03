using System.Runtime.CompilerServices;
using System;

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
            SingleInstance.SingleApplication.Run(new img_visor_25_form());

        }
    }
}