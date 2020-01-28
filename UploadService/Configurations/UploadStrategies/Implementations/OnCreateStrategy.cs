using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadServiceDatabase.DTOs;
using UploadServiceDatabase.Repositories;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class OnCreateStrategy : IUploadStrategy
    {
        private IServerClient _client;
        private IIOHelper _ioHelper;
        private IHashHelper _hashHelper;
        private IEnumerable<UploadOnCreate> _folders;
        private IUploadServiceRepository _repository;
        private List<MyFileSystemWatcher> watchers;

        public OnCreateStrategy(IServerClient client, IIOHelper ioHelper,
            IEnumerable<IUploadTypeConfiguration> filesToUpload, IHashHelper hashHelper,
            IUploadServiceRepository repository)
        {
            _client = client;
            _ioHelper = ioHelper;
            _hashHelper = hashHelper;
            _folders = filesToUpload.Cast<UploadOnCreate>();
            _repository = repository;
        }

        public void Upload()
        {
            watchers = new List<MyFileSystemWatcher>();

            foreach (var folder in _folders)
            {
                MyFileSystemWatcher watcher = CreateWatcher(folder);
                watchers.Add(watcher);
            }

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            foreach (var w in watchers)
            {
                w.Created += async (sender, e) =>
                {
                    Console.WriteLine("created" + e.ChangeType);

                    var localFilePath = w.Path + "/" + e.Name;
              
                    await OnCreateEvent(new UploadFileBackupDTO
                    {
                        archiveFolder = w.archiveFolder,
                        cleanUpDays = w.cleanUpDays,
                        fileMask = w.fileMask,
                        localFilePath = localFilePath,
                        remoteFolder = w.RemoteFolder
                    });
                };
            }
        }

        public MyFileSystemWatcher CreateWatcher(UploadOnCreate folder)
        {
            var watcher = new MyFileSystemWatcher()
            {
                Path = folder.LocalFolderPath,
                Filter = folder.FileMask,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                               NotifyFilters.FileName | NotifyFilters.DirectoryName,
                RemoteFolder = folder.RemoteFolder,
                archiveFolder = folder.ArchiveFolder,
                fileMask = folder.FileMask,
                cleanUpDays = folder.CleanUpPeriodDays,
                EnableRaisingEvents = true
            };

            return watcher;
        }

        private async Task OnCreateEvent(UploadFileBackupDTO dto)
        {
            Console.WriteLine("I am here");
            var localHash = _hashHelper.GenerateHash(dto.localFilePath);
            
            _repository.InsertFile(new FileDTO
            {
                FilePath = dto.localFilePath, 
                HashedContent = localHash
            });
            
            Console.WriteLine("change happend");

            await _hashHelper.UploadFileWithBackupHandling(dto, _ioHelper);
        }
    }
}