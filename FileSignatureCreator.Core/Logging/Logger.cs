using FileSignatureCreator.Core.Infrastructure;
using System.Runtime.CompilerServices;

namespace FileSignatureCreator.Core.Logging
{
    public class Logger : ILogger
    {
        private readonly bool _debugModeEnabled;
        private readonly IConsole _console;
        private const string DebugLogFileName = "FileSignatureCreator.log";
        private const string DebugSeverity = "DEBUG";
        private const string ErrorSeverity = "ERROR";

        public Logger(bool debugModeEnabled, IConsole console)
        {
            _debugModeEnabled = debugModeEnabled;
            _console = console;
        }

        public void Debug(string message, [CallerFilePath] string caller = "")
        {
            if(_debugModeEnabled)
            {
                WriteToLogFile(message, caller, DebugSeverity);
            }
        }

        public void Exception(Exception ex, [CallerFilePath] string caller = "")
        {
            var message = ex.ToString();
            _console.WriteLine(message);
            WriteToLogFile(message, caller, ErrorSeverity);
        }

        private void WriteToLogFile(string message, string caller, string severity)
        {
            lock (DebugLogFileName)
            {
                File.AppendAllLines(
                    DebugLogFileName,
                    new[] { FormatLogMessage(message, caller, severity) });
            }
        }

        private string FormatLogMessage(string message, string caller, string severity) => 
            $"{DateTime.Now} {severity} {Path.GetFileNameWithoutExtension(caller)} {message}";
    }
}