using System;
using System.IO;
using System.Threading;
using VMManager.Services.Interfaces;

namespace VMManager.Services
{
    /// <summary>
    /// Simple, thread-safe logger that logs to console and daily rotating log file.
    /// </summary>
    public sealed class Logger : ILogger
    {
        private static readonly string LogDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        private static readonly string LogFilePath =
            Path.Combine(LogDirectory, $"log_{DateTime.Now:yyyyMMdd}.txt");

        private static readonly object _fileLock = new();

        public void Info(string message) => Log("INFO", message);
        public void Warn(string message) => Log("WARN", message);
        public void Error(string message) => Log("ERROR", message);

        private void Log(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] [{level}] {message}";

            // Always log to console
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = level switch
            {
                "ERROR" => ConsoleColor.Red,
                "WARN" => ConsoleColor.Yellow,
                _ => ConsoleColor.Gray
            };

            Console.WriteLine(logEntry);
            Console.ForegroundColor = originalColor;

            // Append to daily log file
            try
            {
                lock (_fileLock)
                {
                    Directory.CreateDirectory(LogDirectory);
                    File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
                }
            }
            catch
            {
                // 🛑 Silently suppress logging failures (avoid recursive crash)
            }
        }
    }
}
