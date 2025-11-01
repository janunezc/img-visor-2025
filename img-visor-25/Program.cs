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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            SingleInstance.SingleApplication.Run(new  img_visor_25.Form1());

        }
    }
}