namespace FileSignatureCreator.Core.Infrastructure
{
    internal interface IReadingChannel<TData>
    {
        bool TryRead(out TData? data, CancellationToken token);

        bool IsClosed { get; }
    }
}
