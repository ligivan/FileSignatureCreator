namespace FileSignatureCreator.Core.DataProcessing
{
    public class FileProcessorsNumberCalculator : IFileProcessorsNumberCalculator
    {
        private const int MinimumProcessorsNumber = 1;
        private readonly static int MaximumProcessorsNumber = Environment.ProcessorCount;

        public int CalculateNeededProcessorsNumber(long blocksNumber)
        {
            if(blocksNumber > MaximumProcessorsNumber)
            {
                return MaximumProcessorsNumber;
            }

            if(blocksNumber <= MinimumProcessorsNumber)
            {
                return MinimumProcessorsNumber;
            }

            return (int)blocksNumber;
        }

    }
}
