using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class TimeSpecificStrategy : IUploadStrategy
    {
        //private static System.Timers.Timer aTimer;

        private IEnumerable<TimeSpecificUpload> _foldersToUpload;
        private IServerClient _client;
        private IIOHelper _ioHelper;
        private IHashHelper _hashHelper;

        private IUploadServiceRepository _repository;
        //private DateTime scheduledTime;

        public TimeSpecificStrategy(IEnumerable<IUploadTypeConfiguration> foldersToUpload, IServerClient client,
            IIOHelper ioHelper, IHashHelper hashHelper, IUploadServiceRepository repository)
        {
            _foldersToUpload = foldersToUpload.Cast<TimeSpecificUpload>();
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
                DateTime dt = item.Time.ToUniversalTime();

                var scheduledTime = DateTime.Today.AddDays(0).AddHours(dt.Hour).AddMinutes(dt.Minute);


                var timer = new Timer();
                timer.Enabled = true;

                if (DateTime.Now > scheduledTime)
                    scheduledTime = scheduledTime.AddDays(1);

                timer.Interval = scheduledTime.Subtract(DateTime.Now).TotalSeconds * 1000;
                timer.AutoReset = true;

                timerMatrix.Add(timer);

                timer.Elapsed += (sender, e) =>
                {
                    var path = item.LocalFolderPath;
                    OnTimedEvent(item, scheduledTime, timer);
                };
            }
        }

        private void OnTimedEvent(TimeSpecificUpload item, DateTime scheduledTime, Timer timer)
        {
            double tillNextInterval = scheduledTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            if (tillNextInterval < 0) tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            timer.Interval = tillNextInterval;


            var remoteFolder = item.RemoteFolder;
            var fileMask = item.FileMask;
            var archiveFolder = item.ArchiveFolder;
            var cleanUpDays = item.CleanUpPeriodDays;
            var localFolderPath = item.LocalFolderPath;

            _ioHelper.CreateDirectoryIfNotExist(archiveFolder);

            Console.WriteLine(localFolderPath);
            foreach (string filePath in Directory.EnumerateFiles(localFolderPath, fileMask,
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
                    }, _ioHelper);
                }
                else
                {
                    Console.WriteLine("change did not happen");
                }
            }
        }
    }
}