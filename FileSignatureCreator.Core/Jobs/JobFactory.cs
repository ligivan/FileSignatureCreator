using FileSignatureCreator.Core.Logging;

namespace FileSignatureCreator.Core.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _tokenSource;

        public JobFactory(ILogger logger, CancellationTokenSource tokenSource)
        {
            _logger = logger;
            _tokenSource = tokenSource;
        }

        public Job[] RunMultipleJobs(IExecutable[] actions)
        {
            _logger.Debug($"starting {actions.Length} jobs");
            var jobs = new List<Job>(actions.Length);
            foreach (var action in actions)
            {
                var job = new Job(action.Execute, _logger, _tokenSource);
                jobs.Add(job);
                job.Start();
            }

            return jobs.ToArray();
        }
    }
}
