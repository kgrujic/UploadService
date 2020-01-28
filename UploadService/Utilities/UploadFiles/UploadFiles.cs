using System.IO;
using System.Threading.Tasks;
using UploadService.DTOs;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadServiceDatabase.DTOs;
using UploadServiceDatabase.Repositories;

namespace UploadService.Utilities.UploadFiles
{
    public class UploadFiles : IUpload
    {
        private IServerClient _client;
        private IUploadServiceRepository _repository;
        private IHashHelper _hashHelper;

        public UploadFiles(IServerClient client, IUploadServiceRepository repository, IHashHelper hashHelper)
        {
            _client = client;
            _repository = repository;
            _hashHelper = hashHelper;
        }
        

        public async Task UploadFile(string localFilePath, string remoteFolder) 
        {
            var remoteFilePath = Path.Combine("home/katarina/", remoteFolder, Path.GetFileName(localFilePath));
            
            var localHash = _hashHelper.GenerateHash(localFilePath);
           

            if (_client.checkIfFileExists(remoteFilePath))
            {
                var hashFromDb = _repository.GetFileByPath(localFilePath).HashedContent;
                if (!_hashHelper.HashMatching(localHash, hashFromDb))
                {
                    _client.UploadFile(remoteFilePath, localFilePath, true);

                    _repository.UpdateFile(new FileDTO()
                    {
                        FilePath = localFilePath, HashedContent = localHash
                    });
                    
                }
            }
            else
            {
                _client.UploadFile(remoteFilePath, localFilePath, false);

                _repository.InsertFile(new FileDTO()
                {
                    FilePath = localFilePath, HashedContent = localHash
                });
                
            }
        }
    }
}