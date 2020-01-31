using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.UploadFiles;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class TimeSpecificStrategy : IUploadStrategy
    {
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;

        public TimeSpecificStrategy(IUpload upload, IArchive archive, IClineable clean)
        {
            _upload = upload;
            _archive = archive;
            _clean = clean;
        }

        public void Upload(IEnumerable<IUploadTypeConfiguration> timeSpecificlUploads)
        {
            List<Timer> timerMatrix = new List<Timer>();

            foreach (var item in timeSpecificlUploads.Cast<TimeSpecificUpload>())
            {
                DateTime dt = item.Time.ToUniversalTime();

                var scheduledTime = DateTime.Today.AddHours(dt.Hour).AddMinutes(dt.Minute);


                var timer = new Timer();
                timer.Enabled = true;

                if (DateTime.Now > scheduledTime)
                    scheduledTime = scheduledTime.AddDays(1);

                timer.Interval = scheduledTime.Subtract(DateTime.Now).TotalMilliseconds;
                timer.AutoReset = true;

                timerMatrix.Add(timer);

                timer.Elapsed += (sender, e) =>
                {
                   
                    OnTimedEvent(item, scheduledTime, timer);
                };
            }
        }

        private void OnTimedEvent(TimeSpecificUpload item, DateTime scheduledTime, Timer timer)
        {
            //double tillNextInterval = scheduledTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            //if (tillNextInterval < 0) tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            timer.Interval = TimeSpan.FromHours(24).Milliseconds;


            var remoteFolder = item.RemoteFolder;
            var fileMask = item.FileMask;
            var archiveFolder = item.ArchiveFolder;
            var cleanUpDays = item.CleanUpPeriodDays;
            var localFolderPath = item.LocalFolderPath;

            // _ioHelper.CreateDirectoryIfNotExist(archiveFolder);

            foreach (string filePath in Directory.EnumerateFiles(localFolderPath, fileMask, SearchOption.AllDirectories)
            )
            {
                var dto = new UploadFileBackupDTO
                {
                    archiveFolder = archiveFolder, cleanUpDays = cleanUpDays,
                    fileMask = fileMask, localFilePath = filePath, remoteFolder = remoteFolder
                };
                _upload.UploadFile(dto.localFilePath, dto.remoteFolder);
                _clean.CleanOutdatedFilesOnDays(dto.archiveFolder, dto.fileMask, dto.cleanUpDays);
                _archive.SaveFileToArchiveFolder(dto.localFilePath,
                    Path.Combine(dto.archiveFolder, Path.GetFileName(dto.localFilePath)));
            }
        }
    }
}