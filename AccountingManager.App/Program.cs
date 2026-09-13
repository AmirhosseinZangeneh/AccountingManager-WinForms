using AccountingManager.App.UI;
using System;
using System.Windows.Forms;

namespace AccountingManager.App
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, args) =>
                UserMessages.ShowError(args.Exception, "Unhandled UI exception");
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception exception = args.ExceptionObject as Exception;
                if (exception != null)
                {
                    Infrastructure.AppLogger.Error(exception, "Unhandled application exception");
                }
            };

            using (frmLogin loginForm = new frmLogin())
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            Application.Run(new Form1());
        }
    }
}
