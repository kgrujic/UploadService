using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadServiceDatabase.DTOs;
using UploadServiceDatabase.Repositories;
using UploadService.Utilities;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class OnChangeStrategy : IUploadStrategy
    {
        private IServerClient _client;
        private IIOHelper _ioHelper;
        private IHashHelper _hashHelper;
        private IEnumerable<UploadOnChange> _filesToUpload;
        private IUploadServiceRepository _repository;
        private List<MyFileSystemWatcher> watchers;

        public OnChangeStrategy(IServerClient client, IIOHelper ioHelper,
            IEnumerable<IUploadTypeConfiguration> filesToUpload, IHashHelper hashHelper)
        {
            _client = client;
            _ioHelper = ioHelper;
            _hashHelper = hashHelper;
            _filesToUpload = filesToUpload.Cast<UploadOnChange>();
            _repository = new UploadServiceRepository();
        }

        public void Upload()
        {
            watchers = new List<MyFileSystemWatcher>();

            foreach (var file in _filesToUpload)
            {
                MyFileSystemWatcher watcher = CreateWatcher(file);
                watchers.Add(watcher);
            }

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            foreach (var w in watchers)
            {
                w.Renamed += async (sender, e) =>
                {
                    var localFilePath = w.Path + "/" + w.Filter;
                    var remoteFolder = w.RemoteFolder;
                    await OnChangeEvent(localFilePath, remoteFolder);
                };
            }
        }

        public MyFileSystemWatcher CreateWatcher(UploadOnChange file)
        {
            var watcher = new MyFileSystemWatcher()
            {
                Path = Path.GetDirectoryName(file.LocalFilePath),
                Filter = Path.GetFileName(file.LocalFilePath),
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                               NotifyFilters.FileName | NotifyFilters.DirectoryName,
                RemoteFolder = file.RemoteFolder,
                EnableRaisingEvents = true
            };

            return watcher;
        }


        private async Task OnChangeEvent(string localFilePath, string remoteFolder)
        {
            Console.WriteLine("I am here");
            var localHash = _hashHelper.GenerateHash(localFilePath);


            var hashFromDb = _repository.GetFileByPath(localFilePath).HashedContent;
            Console.WriteLine(hashFromDb);


            //TODO bug
            if (!_hashHelper.HashMatching(localHash, hashFromDb))
            {
                Console.WriteLine("change happend");

                await UploadFile(localFilePath, remoteFolder, localHash);
            }
            else
            {
                Console.WriteLine("change did not happen");
            }
        }

        private async Task UploadFile(string localFilePath, string remoteFolder, byte[] localHash)
        {
            var remoteFilePath = $"{"home/katarina/" + remoteFolder + "/"}{Path.GetFileName(localFilePath)}";
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
    }
}