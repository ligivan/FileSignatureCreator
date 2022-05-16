namespace FileSignatureCreator.Core.DataProcessing
{
    public class BlocksNumberCalculator : IBlocksNumberCalculator
    {
        public long CalculateExpectedBlocksNumber(long fileSize, int dataBlockSize)
        {
            return fileSize == 0? 1 : (long)Math.Ceiling(fileSize * 1.0/ dataBlockSize);
        }
    }
}