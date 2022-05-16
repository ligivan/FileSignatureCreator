using System.Security.Cryptography;

namespace FileSignatureCreator.Core.DataProcessing
{
    public class SHA256HashCalculator : IDisposable
    {
        private readonly SHA256 _hasher = SHA256.Create();

        public byte[] CalculateHash(byte[] data)
        {
            var result = _hasher.ComputeHash(data);

            return result;
        }

        public void Dispose()
        {
            _hasher.Dispose();
        }
    }
}