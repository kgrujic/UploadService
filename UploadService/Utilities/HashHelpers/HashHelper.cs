using System;
using System.IO;
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
        private IServerClient _client;
        private IUploadServiceRepository _repository;

        public HashHelper(IServerClient client, IUploadServiceRepository repository)
        {
            _client = client;
            _repository = repository;
        }


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