using System;
using System.IO;
using System.Security.Cryptography;

namespace UploadService.Utilities.HashHelpers
{
    public class HashHelper : IHashHelper
    {
        public byte[] GenerateHash(string path)
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

        public bool HashMatching(byte[] hashFirst, byte[] hashSecond)
        {
            if (BitConverter.ToString(hashFirst) == BitConverter.ToString(hashSecond))
            {
                return true;
            }

            return false;
        }
    }
}