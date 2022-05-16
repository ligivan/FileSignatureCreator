using FileSignatureCreator.Core.Infrastructure;
using FileSignatureCreator.Core.Jobs;
using FileSignatureCreator.Core.Logging;

namespace FileSignatureCreator.Core.DataProcessing
{
    public class FileDataBlocksReader : IExecutable
    {
        private readonly string _filename;
        private readonly int _blockSize;
        private readonly IWritingChannel<DataBlock> _destinationChannel;
        private readonly ILogger _logger;

        public FileDataBlocksReader(
            string fileName,
            int blockSize,
            IWritingChannel<DataBlock> destinationChannel,
            ILogger logger)
        {
            _filename = fileName;
            _blockSize = blockSize;
            _destinationChannel = destinationChannel;
            _logger = logger;
        }

        public void Execute(CancellationToken token)
        {
            _logger.Debug("execution started");
            try
            {
                using (var fileStream = File.OpenRead(_filename))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        long currentBlockNumber = 0;
                        while (!token.IsCancellationRequested)
                        {
                            var data = reader.ReadBytes(_blockSize);
                            _logger.Debug($"block number {currentBlockNumber}: data length {data.Length}");
                            
                            SendDataForProcessing(
                                new DataBlock(
                                currentBlockNumber++,
                                data), 
                                token);
                          
                            if (fileStream.Position == fileStream.Length)
                            {
                                _logger.Debug("reading complete");
                                break;
                            }
                        }

                    }
                }
            }
            finally
            {
                _destinationChannel.Close();
            }

            _logger.Debug("execution finished");
        }

        private void SendDataForProcessing(DataBlock dataBlock, CancellationToken token)
        {
            _logger.Debug($"sending block number {dataBlock.Number} to output");
            _destinationChannel.Write(dataBlock, token);
        }
    }
}
