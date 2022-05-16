namespace FileSignatureCreator.Core.DataProcessing
{
    public interface IBlocksNumberCalculator
    {
        long CalculateExpectedBlocksNumber(long fileSize, int dataBlockSize);
    }
}
