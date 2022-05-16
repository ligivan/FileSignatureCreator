using FileSignatureCreator.Core.Infrastructure;
using FileSignatureCreator.Core.Jobs;
using FileSignatureCreator.Core.Logging;

namespace FileSignatureCreator.Core.DataProcessing
{
    public class FileSignatureCalculationProcessor
    {
        private readonly IFileProcessorsNumberCalculator _fileProcessorsNumberCalculator;
        private readonly IBlocksNumberCalculator _blocksNumberCalculator;
        private readonly ILogger _logger;
        private readonly IJobFactory _jobFactory;
        private readonly IConsole _console;

        public FileSignatureCalculationProcessor(
            IFileProcessorsNumberCalculator fileProcessorsNumberCalculator,
            ILogger logger,
            IJobFactory jobFactory,
            IBlocksNumberCalculator blocksNumberCalculator,
            IConsole console)
        {
            _fileProcessorsNumberCalculator = fileProcessorsNumberCalculator;
            _logger = logger;
            _jobFactory = jobFactory;
            _blocksNumberCalculator = blocksNumberCalculator;
            _console = console;
        }

        public OperationResult RunSignatureCalculation(string fileName, int blockSize)
        {
            var fileLength = new FileInfo(fileName).Length;
           
            var expectedBlocksNumber = _blocksNumberCalculator.CalculateExpectedBlocksNumber(fileLength, blockSize);
            var desiredProcessorsCount = _fileProcessorsNumberCalculator.CalculateNeededProcessorsNumber(expectedBlocksNumber);

            var maxElementsInDataChannel = desiredProcessorsCount << 1;
            _logger.Debug($"starting signature calculation for file {fileName} with block size {blockSize}" +
                $"using processors count {desiredProcessorsCount} expected blocks number will be {expectedBlocksNumber}");

            var inputDataChannel = new DataChannel<DataBlock>(maxElementsInDataChannel);
            var outputDataChannel = new DataChannel<DataBlock>(maxElementsInDataChannel);
            var reader = new FileDataBlocksReader(fileName, blockSize, inputDataChannel, _logger);

            var actions = new List<IExecutable>();
            actions.Add(reader);

            for (int i = 0; i < desiredProcessorsCount; i++)
            {
                actions.Add(new DataBlocksHasher(inputDataChannel, outputDataChannel, _logger));
            }

            actions.Add(new DataBlocksConsoleWriter(outputDataChannel, _logger, expectedBlocksNumber, _console));

            var jobs = _jobFactory.RunMultipleJobs(actions.ToArray());
            try
            {
                _logger.Debug("Waiting for all jobs to finish...");
                Job.WaitAll(jobs);
            }
            finally
            {
                foreach(var job in jobs)
                {
                    job.Dispose();
                }
            }

            return jobs.Any(job => job.IsExceptional) ? 
                OperationResult.Error : OperationResult.Success;
        }
    }
}