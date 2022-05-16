using FileSignatureCreator.Core.Infrastructure;
using FileSignatureCreator.Core.Jobs;
using FileSignatureCreator.Core.Logging;
using System.Text;

namespace FileSignatureCreator.Core.DataProcessing
{
    internal class DataBlocksConsoleWriter : IExecutable
    {
        private readonly IReadingChannel<DataBlock> _dataBlocksSource;
        private readonly ILogger _logger;
        private readonly long _expectedBlocksNumber;
        private readonly IConsole _console;

        public DataBlocksConsoleWriter(
            IReadingChannel<DataBlock> dataBlocksSource, 
            ILogger logger,
            long expectedBlocksNumber,
            IConsole console)
        {
            _dataBlocksSource = dataBlocksSource;
            _logger = logger;
            _expectedBlocksNumber = expectedBlocksNumber;
            _console = console;
        }

        public void Execute(CancellationToken token)
        {
            DataBlock? dataBlock;
            long currentBlockNumber = 0;
            var cachedBlocks = new List<DataBlock>();
            _logger.Debug("execution started");

            while (!token.IsCancellationRequested)
            {
                if (!_dataBlocksSource.TryRead(out dataBlock, token))
                {
                    if (_dataBlocksSource.IsClosed)
                    {
                        break;
                    }

                    continue;
                }

                _logger.Debug($"got block number {dataBlock.Number}, expected number {currentBlockNumber}");
                if (dataBlock.Number != currentBlockNumber)
                {
                    _logger.Debug($"adding block number {dataBlock.Number} to cache");
                    cachedBlocks.Add(dataBlock);
                    continue;
                }

                WriteDataBlock(dataBlock);
                currentBlockNumber++;

                while ((dataBlock = cachedBlocks.FirstOrDefault(block => block.Number == currentBlockNumber)) != null)
                {
                    _logger.Debug($"getting block number {dataBlock.Number} from cache");
                    cachedBlocks.Remove(dataBlock);
                    WriteDataBlock(dataBlock);
                    currentBlockNumber++;
                }
            }

            if (!token.IsCancellationRequested)
            {
                if (cachedBlocks.Count > 0)
                {
                    throw new Exception("Some blocks were missing");
                }

                if (currentBlockNumber != _expectedBlocksNumber)
                {
                    throw new Exception(
                        $"Expected {_expectedBlocksNumber} data blocks, but only {currentBlockNumber} blocks were processed");
                }
            }

            _logger.Debug("execution finished");
        }

        private void WriteDataBlock(DataBlock dataBlock)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < dataBlock.Data.Length; i++)
            {
                builder.Append(dataBlock.Data[i].ToString("x2"));
            }

            _console.WriteLine($"Block {dataBlock.Number} Hash {builder}");
        }
    }
}
