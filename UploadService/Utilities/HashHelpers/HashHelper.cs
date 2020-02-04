using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UploadService.DTOs;
using UploadService.Utilities.Clients;
using UploadService.Utilities.IO_Helpers;
using UploadServiceDatabase.DTOs;
using UploadServiceDatabase.Repositories;

namespace UploadService.Utilities.HashHelpers
{
    public class HashHelper : IHashHelper
    {
        
        public HashHelper()
        {
           
        }
        
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

        public bool HashMatching(byte[] hashFirst, byte[] hashSecond)
        {

            if (hashFirst.SequenceEqual(hashSecond))
            {
                return true;
            }

            return false;
        }
    }
}