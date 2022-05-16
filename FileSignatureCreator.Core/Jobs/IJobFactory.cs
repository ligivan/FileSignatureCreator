namespace FileSignatureCreator.Core.Jobs
{
    public interface IJobFactory
    {
        Job[] RunMultipleJobs(IExecutable[] actions);
    }
}
