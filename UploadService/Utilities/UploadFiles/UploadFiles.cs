using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadServiceDatabase.DTOs;
using UploadServiceDatabase.Repositories;

namespace UploadService.Utilities.UploadFiles
{
    /// <summary>
    /// UploadFiles class contains method for file uploading on remote FTP server and storing hash content in database
    /// Implements IUpload interface
    /// </summary>
    public class UploadFiles : IUpload
    {
        private IServerClient _client;
        private IUploadServiceRepository _repository;
        private IHashHelper _hashHelper;
        private ILogger<Worker> _logger;

        public UploadFiles(IServerClient client, IUploadServiceRepository repository, IHashHelper hashHelper, ILogger<Worker> logger)
        {
            _client = client;
            _repository = repository;
            _hashHelper = hashHelper;
            _logger = logger;
        }
        
        /// <summary>
        /// UploadFile method  uploads file on remote server and store generated hash from content in database, with validation of file existence on those locations and validation of changes
        /// </summary>
        /// <param name="localFilePath">string</param>
        /// <param name="remoteFolder">string</param>
        /// <returns>Task</returns>
        public async Task UploadFile(string localFilePath, string remoteFolder)
        {
            var remoteFilePath = Path.Combine(remoteFolder, Path.GetFileName(localFilePath));

            var localHash = _hashHelper.GenerateHash(localFilePath);

            if (_repository.FileExistInDatabase(localFilePath) && _client.CheckIfFileExists(remoteFilePath))
            {
                var hashFromDb = _repository.GetFileByPath(localFilePath).HashedContent;
                if (!_hashHelper.HashMatching(localHash, hashFromDb))
                {
                    var dto = new FileDTO()
                    {
                        FilePath = localFilePath, HashedContent = localHash
                    };
                    
                    _client.UploadFile(remoteFilePath, localFilePath, true);
                    _repository.UpdateFile(dto);
                    
                }
            }

            else
            {
                var dto = new FileDTO()
                {
                    FilePath = localFilePath, HashedContent = localHash
                };
                _client.UploadFile(remoteFilePath, localFilePath, false);
                
                if (_repository.FileExistInDatabase(localFilePath))
                {
                    _repository.UpdateFile(dto);
                }
                else
                {
                    _repository.InsertFile(dto);
                }
                
            }
            
            _logger.LogInformation($"File {Path.GetFileName(localFilePath)} from location {localFilePath} is uploaded on remote server in folder {remoteFolder}  at: {DateTime.Now}");
        }
    }
}