namespace FileSignatureCreator.Core.Jobs
{
    public interface IExecutable
    {
        void Execute(CancellationToken cancellationToken);
    }
}
