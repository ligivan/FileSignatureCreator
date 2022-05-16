using FileSignatureCreator.Core.Infrastructure;

namespace FileSignatureCreator.Tests
{
    internal class ConsoleMock : IConsole
    {
        private readonly Queue<string> _logs = new Queue<string>();

        public void WriteLine(string message)
        {
            lock (_logs)
            {
                _logs.Enqueue(message);
            }
        }

        public string ReadLine()
        {
            lock (_logs)
            {
                return _logs.Dequeue();
            }
        }

        public string[] ReadAllLines()
        {
            var result = new List<string>();
            lock (_logs)
            {
                while(_logs.TryDequeue(out string line))
                {
                    result.Add(line);
                }

                return result.ToArray();
            }
        }
    }
}