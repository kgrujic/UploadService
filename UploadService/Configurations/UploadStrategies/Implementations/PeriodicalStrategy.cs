using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Timers;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadServiceDatabase.Repositories;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class PeriodicalStrategy : IUploadStrategy
    {
        private IEnumerable<PeriodicalUpload> _foldersToUpload;
        private IServerClient _client;
        private IIOHelper _ioHelper;
        private IHashHelper _hashHelper;
        private IUploadServiceRepository _repository;


        public PeriodicalStrategy(IEnumerable<IUploadTypeConfiguration> foldersToUpload, IServerClient client,
            IIOHelper ioHelper, IHashHelper hashHelper, IUploadServiceRepository repository
        )
        {
            _foldersToUpload = foldersToUpload.Cast<PeriodicalUpload>();
            _client = client;
            _ioHelper = ioHelper;
            _hashHelper = hashHelper;
            _repository = repository;
        }

        public void Upload()
        {
            List<Timer> timerMatrix = new List<Timer>();

            foreach (var item in _foldersToUpload)
            {
                var timer = new System.Timers.Timer()
                {
                    Enabled = true,
                    Interval = item.Interval,
                    AutoReset = true
                };

                timerMatrix.Add(timer);

                timer.Elapsed += (sender, e) => { OnTimedEvent(item); };
            }
        }


        private void OnTimedEvent(PeriodicalUpload item)
        {
            var remoteFolder = item.RemoteFolder;
            var fileMask = item.FileMask;
            var archiveFolder = item.ArchiveFolder;
            var cleanUpDays = item.CleanUpPeriodDays;
            var localFolderPath = item.LocalFolderPath;

            _ioHelper.CreateDirectoryIfNotExist(archiveFolder);

            foreach (string filePath in Directory.EnumerateFiles(localFolderPath, "*" + fileMask,
                SearchOption.AllDirectories))
            {
                
                var localHash = _hashHelper.GenerateHash(filePath);
                var hashFromDb = _repository.GetFileByPath(filePath).HashedContent;

                //TODO bug
                if (!_hashHelper.HashMatching(localHash, hashFromDb))
                {
                    Console.WriteLine("change happend");

                    _hashHelper.UploadFileWithBackupHandling(new UploadFileBackupDTO
                    {
                        archiveFolder = archiveFolder, cleanUpDays = cleanUpDays,
                        fileMask = fileMask, localFilePath = filePath, remoteFolder = remoteFolder
                    }, localHash, _ioHelper);
                }
                else
                {
                    Console.WriteLine("change did not happen");
                }
            }
        }
    }
}