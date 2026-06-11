// first modified in 18/04/20
//
using System;
using System.Windows.Forms;
using Library_app_test.Data;

namespace Library_app_test
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
            ApplicationConfiguration.Initialize();
            // Ensure database exists before showing UI
            LibraryDb.EnsureDatabase();
            // global exception handlers to show errors instead of silent crashes
            Application.ThreadException += (s, e) =>
            {
                try { MessageBox.Show($"Unhandled UI exception:\n{e.Exception}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                try {
                    var ex = e.ExceptionObject as Exception;
                    MessageBox.Show($"Unhandled exception:\n{ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } catch { }
            };


            // Show splash screen first as a modal dialog
            using (var splash = new Form1())
            {
                splash.ShowDialog();
            }

            // Then start the main application with the sign-in form
            Application.Run(new Form2());
        }
    }
}