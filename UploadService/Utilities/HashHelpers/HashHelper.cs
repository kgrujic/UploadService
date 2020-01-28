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


        /*public async Task UploadFileOnChange(string localFilePath, string remoteFolder, byte[] localHash)
        {
            var remoteFilePath = Path.Combine("home/katarina/", remoteFolder, Path.GetFileName(localFilePath));

            if (_client.checkIfFileExists(remoteFilePath))
            {
                _client.UploadFile(remoteFilePath, localFilePath, true);

                _repository.UpdateFile(new FileDTO()
                {
                    FilePath = localFilePath, HashedContent = localHash
                });
            }
            else
            {
                _client.UploadFile(remoteFilePath, localFilePath, false);

                _repository.UpdateFile(new FileDTO()
                {
                    FilePath = localFilePath, HashedContent = localHash
                });
            }
        }

        public async Task UploadFileWithBackupHandling(UploadFileBackupDTO dto, IIOHelper _ioHelper)
        {
            var remoteFilePath = Path.Combine("home/katarina/", dto.remoteFolder, Path.GetFileName(dto.localFilePath));
            
            var localHash = this.GenerateHash(dto.localFilePath);
           

            if (_client.checkIfFileExists(remoteFilePath))
            {
                var hashFromDb = _repository.GetFileByPath(dto.localFilePath).HashedContent;
                if (!HashMatching(localHash, hashFromDb))
                {
                    _client.UploadFile(remoteFilePath, dto.localFilePath, true);

                    _repository.UpdateFile(new FileDTO()
                    {
                        FilePath = dto.localFilePath, HashedContent = localHash
                    });

                    /*_ioHelper.CleanOutdatedFiles(dto.archiveFolder, dto.fileMask, dto.cleanUpDays);
                    _ioHelper.SaveFileToArchiveFolder(dto.localFilePath,
                        Path.Combine(dto.archiveFolder, Path.GetFileName(dto.localFilePath)));#1#
                }
            }
            else
            {
                _client.UploadFile(remoteFilePath, dto.localFilePath, false);

                _repository.InsertFile(new FileDTO()
                {
                    FilePath = dto.localFilePath, HashedContent = localHash
                });

                /*_ioHelper.CleanOutdatedFiles(dto.archiveFolder, dto.fileMask, dto.cleanUpDays);
                _ioHelper.SaveFileToArchiveFolder(dto.localFilePath,
                    Path.Combine(dto.archiveFolder, Path.GetFileName(dto.localFilePath)));#1#
            }
        }*/
    }
}