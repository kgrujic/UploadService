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
    public class TimeSpecificStrategy : IUploadStrategy<TimeSpecificUpload>
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
        public void StartUpUpload(IEnumerable<TimeSpecificUpload> list)
        {
            foreach (var item in list)
            {
                UploadFolder(item);
            }
        }
        public void Upload(IEnumerable<TimeSpecificUpload> timeSpecificlUploads)
        {
            StartUpUpload(timeSpecificlUploads);
            
            List<Timer> timerMatrix = new List<Timer>();

            foreach (var item in timeSpecificlUploads)
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
            UploadFolder(item);
            timer.Interval = TimeSpan.FromHours(24).Milliseconds;
          
        }
        
        private void UploadFolder(TimeSpecificUpload item)
        {
            foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask, SearchOption.AllDirectories))
            {
                var dto  = CreateUploadFileDto(item, filePath);
                
                _upload.UploadFile(dto.LocalFilePath, dto.RemoteFolder);
                _clean.CleanOutdatedFilesOnDays(dto.ArchiveFolder, dto.FileMask, dto.CleanUpDays);
                _archive.SaveFileToArchiveFolder(dto.LocalFilePath,
                    Path.Combine(dto.ArchiveFolder, Path.GetFileName(dto.LocalFilePath)));
            }
        }

        private UploadFileBackupDto CreateUploadFileDto(TimeSpecificUpload item, string filePath)
        {
            var dto = new UploadFileBackupDto
            {
                ArchiveFolder = item.ArchiveFolder,
                CleanUpDays = item.CleanUpPeriodDays,
                FileMask = item.FileMask,
                LocalFilePath = filePath,
                RemoteFolder = item.RemoteFolder
            };

            return dto;
        }
    }
}