namespace FileSignatureCreator.Console
{
    public class JobParameters
    {
        public JobParameters(string filePath, int dataBlockSize)
        {
            FilePath = filePath;
            DataBlockSize = dataBlockSize;
        }
        
        public string FilePath { get; }
        
        public int DataBlockSize { get; }
    }
}