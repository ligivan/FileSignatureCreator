using FileSignatureCreator.Core.Infrastructure;
using FileSignatureCreator.Core.Jobs;
using FileSignatureCreator.Core.Logging;

namespace FileSignatureCreator.Core.DataProcessing
{
    internal class DataBlocksHasher : IExecutable
    {
        private readonly IReadingChannel<DataBlock> _inputDataChannel;
        private readonly IWritingChannel<DataBlock> _outputDataChannel;
        private readonly ILogger _logger;

        public DataBlocksHasher(
            IReadingChannel<DataBlock> inputDataChannel,
            IWritingChannel<DataBlock> outputDataChannel,
            ILogger logger)
        {
            _inputDataChannel = inputDataChannel;
            _outputDataChannel = outputDataChannel;
            _logger = logger;
        }

        public void Execute(CancellationToken token)
        {
            DataBlock? sourceDataBlock;
            _logger.Debug("execution started");

            try
            {
                using (var hashCalculator = new SHA256HashCalculator())
                {
                    while (!token.IsCancellationRequested)
                    {
                        if(!_inputDataChannel.TryRead(out sourceDataBlock, token))
                        {
                            if (_inputDataChannel.IsClosed)
                            {
                                break;
                            }

                            continue;
                        }

                        _logger.Debug($"calculating hash for block {sourceDataBlock.Number}");

                        var hashData = hashCalculator.CalculateHash(sourceDataBlock.Data);
                        _logger.Debug($"sending block {sourceDataBlock.Number} to output");
                        _outputDataChannel.Write(
                            new DataBlock(
                                sourceDataBlock.Number,
                                hashData),
                            token);
                    }
                }
            }
            finally
            {
                _outputDataChannel.Close();
                _logger.Debug("execution finished");
            }
        }
    }
}
