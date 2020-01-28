using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
using UploadServiceDatabase.Repositories;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class TimeSpecificStrategy : IUploadStrategy
    {
        //private static System.Timers.Timer aTimer;

        private IEnumerable<TimeSpecificUpload> _foldersToUpload;
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;
        //private DateTime scheduledTime;

        public TimeSpecificStrategy(IEnumerable<IUploadTypeConfiguration> foldersToUpload,IUpload upload, IArchive archive, IClineable clean)
        {
            _foldersToUpload = foldersToUpload.Cast<TimeSpecificUpload>();
            _upload = upload;
            _archive = archive;
            _clean = clean;
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

           // _ioHelper.CreateDirectoryIfNotExist(archiveFolder);
           
            foreach (string filePath in Directory.EnumerateFiles(localFolderPath, fileMask,
                SearchOption.AllDirectories))
            {

                //TODO bug

                var dto = new UploadFileBackupDTO
                {
                    archiveFolder = archiveFolder, cleanUpDays = cleanUpDays,
                    fileMask = fileMask, localFilePath = filePath, remoteFolder = remoteFolder
                };
                    _upload.UploadFile(dto.localFilePath, dto.remoteFolder);
                    _clean.CleanOutdatedFilesOnDays(dto.archiveFolder,dto.fileMask, dto.cleanUpDays);
                    _archive.SaveFileToArchiveFolder(dto.localFilePath,  Path.Combine(dto.archiveFolder, Path.GetFileName(dto.localFilePath)));
                
              
            }
        }
    }
}