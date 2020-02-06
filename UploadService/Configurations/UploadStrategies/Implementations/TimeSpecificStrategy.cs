using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Microsoft.Extensions.Logging;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.UploadFiles;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    /// <summary>
    /// PeriodicalStrategy Class Handle uploading of files in folder at Specific time
    /// Implements 'IUploadStrategy'<'TimeSpecificUpload'>'
    /// </summary>
    public class TimeSpecificStrategy : IUploadStrategy<TimeSpecificUpload>
    {
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;
        private ILogger<Worker> _logger;

        /// <summary>
        /// TimeSpecificStrategy constructor
        /// </summary>
        public TimeSpecificStrategy(IUpload upload, IArchive archive, IClineable clean,ILogger<Worker> logger)
        {
            _upload = upload;
            _archive = archive;
            _clean = clean;
            _logger = logger;
        }

        /// <summary>
        /// StartUpUpload method uploads changes that happened in  folder when service was inactive
        /// </summary>
        /// <param name="list">List of TimeSpecificUpload objects</param>
        public void StartUpUpload(IEnumerable<TimeSpecificUpload> list)
        {
            foreach (var item in list)
            {
                UploadFolder(item);
            }
            
            _logger.LogInformation($"Start up upload for Time Specific Upload list happened at: {DateTime.Now}");
        }

        /// <summary>
        /// Upload method uploads folder at scheduled Time when service is active
        /// </summary>
        /// <param name="list">List of TimeSpecificUpload objects</param>
        public void Upload(IEnumerable<TimeSpecificUpload> timeSpecificlUploads)
        {
            
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

                timer.Elapsed += (sender, e) => { OnTimedEvent(item, scheduledTime, timer); };
            }
        }


        /// <summary>
        /// Method OnTimedEvent calls UploadFolder method when timer elapsed event happen and set scheduled time to next day
        /// </summary>
        /// <param name="item"> TimeSpecificUpload object </param>
        private void OnTimedEvent(TimeSpecificUpload item, DateTime scheduledTime, Timer timer)
        {
            UploadFolder(item);
            timer.Interval = TimeSpan.FromHours(24).Milliseconds;
        }

        /// <summary>
        /// UploadFolder method calls methods for upload, clean outdated files and arhive from services for all files in folder
        /// </summary>
        /// <param name="item">TimeSpecificUpload object</param>
        private void UploadFolder(TimeSpecificUpload item)
        {
            foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask,
                SearchOption.AllDirectories))
            {
                var dto = CreateUploadFileDto(item, filePath);

                _upload.UploadFile(dto.LocalFilePath, dto.RemoteFolder);
                _clean.CleanOutdatedFilesOnDays(dto.ArchiveFolder, dto.FileMask, dto.CleanUpDays);
                _archive.MoveFileToArchiveFolder(dto.LocalFilePath,
                    Path.Combine(dto.ArchiveFolder, Path.GetFileName(dto.LocalFilePath)));
            }
            _logger.LogInformation($"Folder at {item.LocalFolderPath} location is uploaded at: {DateTime.Now}");
        }

        /// <summary>
        /// CreateUploadFileDto creates instance of UploadFileBackupDto.
        /// </summary>
        /// <param name="item">TimeSpecificUpload object</param>
        /// <param name="filePath">string</param>
        /// <returns>UploadFileBackupDto object</returns>
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