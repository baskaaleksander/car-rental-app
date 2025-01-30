using System;
using System.IO;

namespace CarRentalApp.Utilities
{
    public delegate void LogEventHandler(string message);

    public static class Logger
    {
        private const string LogFilePath = $"logs.txt";
        
        public static event LogEventHandler LogEvent;

        private static string GetProjectDirectory()
        {
            return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        }
        private static string GetDataFilePath(string fileName)
        {
            string projectDirectory = GetProjectDirectory();
            return Path.Combine(projectDirectory, "Data", fileName);
        }
        static Logger()
        {
            if (!File.Exists(LogFilePath))
            {
                File.Create(LogFilePath).Close();
            }
        }

        public static void Log(string message)
        {
            string logEntry = $"[{DateTime.Now}] {message}";
            string logsFilePath = GetDataFilePath(LogFilePath);
            File.AppendAllText(logsFilePath, logEntry + Environment.NewLine);

            LogEvent?.Invoke(logEntry);
        }
    }
}