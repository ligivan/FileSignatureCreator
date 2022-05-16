using FileSignatureCreator.Core.Logging;

namespace FileSignatureCreator.Core.Jobs
{
    public class Job : IDisposable
    {
        private readonly ILogger _logger;
        private readonly Action<CancellationToken> _action;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private EventWaitHandle Waiter { get; }

        public Job(
            Action<CancellationToken> action, 
            ILogger logger, 
            CancellationTokenSource cancellationTokenSource)
        {
            Waiter = new EventWaitHandle(false, EventResetMode.ManualReset);
            _action = action;
            _logger = logger;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public bool IsExceptional { get; private set; }

        public void Start()
        {
            new Thread(() =>
            {
                try
                {
                    _action(_cancellationTokenSource.Token);
                }
                catch (Exception exception)
                {
                    _logger.Exception(exception);
                    Abort();
                }
                finally
                {
                    Waiter.Set();
                }
            }).Start();
        }

        public void Abort()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
            
            IsExceptional = true;
        }

        public void Dispose()
        {
            Waiter.Dispose();
        }

        public static void WaitAll(params Job[] jobs)
        {
            WaitHandle.WaitAll(jobs.Select(job => job.Waiter).ToArray());
        }
    }
}
