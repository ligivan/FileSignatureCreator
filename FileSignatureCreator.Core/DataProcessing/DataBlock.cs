namespace FileSignatureCreator.Core.DataProcessing
{
    public class DataBlock
    {
        public long Number { get; }

        public byte[] Data { get; }

        public DataBlock(long number, byte[] data)
        {
            Number = number;
            Data = data;
        }
    }
}