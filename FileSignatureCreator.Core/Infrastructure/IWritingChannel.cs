namespace FileSignatureCreator.Core.Infrastructure
{
    public interface IWritingChannel<TData>
    {
        void Write(TData data, CancellationToken token);

        void Close();
    }
}
