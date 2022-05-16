using System.Runtime.CompilerServices;

namespace FileSignatureCreator.Core.Logging
{
    public interface ILogger
    {
        void Exception(Exception ex, [CallerFilePath] string caller = "");

        void Debug(string message, [CallerFilePath] string caller = "");
    }
}
