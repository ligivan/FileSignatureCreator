namespace FileSignatureCreator.Core.Infrastructure
{
    public class ConsoleAdapter : IConsole
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
