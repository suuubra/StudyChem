
using System;
using System.IO;
using System.Text;

namespace StudyChem.Models
{
    public static class ErrorLogger
    {
        // Path for saving all error logs
        private static readonly string ErrorLogPath = Path.Combine("Data", "logs", "error_log.txt");


        /// <summary>
        /// Logs an exception with detailed context and stack trace
        /// </summary>
        /// <param name="context">Where the error occurred (e.g., method or class name)</param>
        /// <param name="ex">The caught exception</param>
        public static void Log(string context, Exception ex)
        {
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(ErrorLogPath));

                // Create detailed error message
                var sb = new StringBuilder();
                //Using Append to create a structured error message.
                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
                sb.AppendLine($"Context: {context}");
                sb.AppendLine($"Message: {ex.Message}");
                sb.AppendLine($"StackTrace: {ex.StackTrace}");
                sb.AppendLine(new string('-', 80));

                // Append to the error log
                File.AppendAllText(ErrorLogPath, sb.ToString());
            }
            catch
            {
                // If logging fails, fallback to console output
                Console.WriteLine("Critical error: Unable to log error.");
            }
        }

        /// <summary>
        /// Logs a custom message, useful for tracking events or warnings
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogMessage(string message)
        {
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(ErrorLogPath));

                // Format log entry
                string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MESSAGE: {message}\n";

                // Append to the error log
                File.AppendAllText(ErrorLogPath, entry);
            }
            catch
            {
                // Fallback to console if file logging fails
                Console.WriteLine("Critical error: Unable to log message.");
            }
        }
    }
}
