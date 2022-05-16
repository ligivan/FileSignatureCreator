namespace FileSignatureCreator.Core.Infrastructure
{
    public class DataChannel<TData> : IReadingChannel<TData>, IWritingChannel<TData>
    {
        private readonly Queue<TData> _queue = new Queue<TData>();
        private readonly SemaphoreSlim _readGuard;
        private readonly SemaphoreSlim _writeGuard;
        private volatile bool _isClosed = false;
        private const int GuardWaitingTime = 500;

        public DataChannel(int maxElementsinQueue)
        {
            _readGuard = new SemaphoreSlim(0, maxElementsinQueue);
            _writeGuard = new SemaphoreSlim(maxElementsinQueue, maxElementsinQueue);
        }

        public bool IsClosed => _isClosed;

        public bool TryRead(out TData? data, CancellationToken token)
        {
            while (true)
            {
                _readGuard.Wait(GuardWaitingTime, token);
                if(token.IsCancellationRequested)
                {
                    data = default;
                    return false;
                }

                lock (_queue)
                {
                    if (!_queue.TryDequeue(out data))
                    {
                        if (_isClosed)
                        {
                            data = default;
                            return false;
                        }
                    }
                    else
                    {
                        _writeGuard.Release();
                        return true;
                    }
                    
                }
            }
        }

        public void Write(TData data, CancellationToken token)
        {
            _writeGuard.Wait(token);
            if(token.IsCancellationRequested)
            {
                return;
            }

            lock (_queue)
            {
                _queue.Enqueue(data);
            }

            _readGuard.Release();
        }

        public void Close()
        {
            _isClosed = true;
        }
    }
}
