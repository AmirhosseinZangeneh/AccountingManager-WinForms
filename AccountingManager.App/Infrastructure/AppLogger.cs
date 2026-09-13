using System;
using System.IO;
using System.Text;

namespace AccountingManager.App.Infrastructure
{
    internal static class AppLogger
    {
        private static readonly object SyncRoot = new object();

        public static void Error(Exception exception, string context)
        {
            if (exception == null)
            {
                return;
            }

            try
            {
                string logDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AccountingManager",
                    "Logs");
                Directory.CreateDirectory(logDirectory);

                string logPath = Path.Combine(logDirectory, "application.log");
                StringBuilder entry = new StringBuilder()
                    .AppendLine(new string('-', 72))
                    .AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    .AppendLine("Context: " + (context ?? "Unknown"))
                    .AppendLine(exception.ToString());

                lock (SyncRoot)
                {
                    File.AppendAllText(logPath, entry.ToString());
                }
            }
            catch
            {
                // Logging must never interrupt the user's workflow.
            }
        }
    }
}
