namespace FileSignatureCreator.Core.DataProcessing
{
    internal interface IHashCalculator
    {
        byte[] CalculateHash(byte[] data);
    }
}
