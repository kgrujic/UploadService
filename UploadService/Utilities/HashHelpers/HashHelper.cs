using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace UploadService.Utilities.HashHelpers
{
    /// <summary>
    /// Class HashHelper contains methods for hash value manipulations
    /// </summary>
    public class HashHelper : IHashHelper
    {
        
        public HashHelper()
        {
           
        }
        /// <summary>
        /// GenerateHash method generate hash value from content of file with MD5 algoritam
        /// </summary>
        /// <param name="path">string</param>
        /// <returns>byte array</returns>
        public byte[] GenerateHash(string path)
        {
            try
            {
                byte[] tmpHash;
                using (HashAlgorithm hashAlg = HashAlgorithm.Create("MD5"))
                {
                    using (FileStream fsA = new FileStream(path, FileMode.Open))
                    {
                        // Calculate the hash for the files.
                        tmpHash = hashAlg.ComputeHash(fsA);
                    }
                }

                return tmpHash;
            }
            catch(DirectoryNotFoundException e)
            {
                throw;
            }
        }
        
        /// <summary>
        /// HashMatching method checks if two hash values are equal
        /// </summary>
        /// <param name="hashFirst">byte array </param>
        /// <param name="hashSecond">byte array </param>
        /// <returns>bool</returns>
        public bool HashMatching(byte[] hashFirst, byte[] hashSecond)
        {
            return hashFirst.SequenceEqual(hashSecond);
        }
    }
}