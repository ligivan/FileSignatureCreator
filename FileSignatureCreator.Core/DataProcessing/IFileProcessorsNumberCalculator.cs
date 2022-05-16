namespace FileSignatureCreator.Core.DataProcessing
{
    public interface IFileProcessorsNumberCalculator
    {
        int CalculateNeededProcessorsNumber(long blocksNumber);
    }
}
