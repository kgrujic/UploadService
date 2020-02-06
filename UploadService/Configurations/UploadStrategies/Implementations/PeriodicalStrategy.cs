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
    /// PeriodicalStrategy Class Handle Periodical uploading of files in folder
    /// Implements 'IUploadStrategy'<'PeriodicalUpload'>'
    /// </summary>
    public class PeriodicalStrategy : IUploadStrategy<PeriodicalUpload>
    {
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;
        private ILogger<Worker> _logger;

        /// <summary>
        /// PeriodicalStrategy constructor
        /// </summary>
        public PeriodicalStrategy(IUpload upload,
            IArchive archive, IClineable clean,ILogger<Worker> logger)
        {
            _upload = upload;
            _archive = archive;
            _clean = clean;
            _logger = logger;
        }

        /// <summary>
        /// StartUpUpload method uploads changes that happened in  folder when service was inactive
        /// </summary>
        /// <param name="list">List of PeriodicalUpload objects</param>
        public void StartUpUpload(IEnumerable<PeriodicalUpload> list)
        {
            foreach (var item in list)
            {
                UploadFolder(item);
            }
            
            _logger.LogInformation($"Start up upload for Periodical Upload list happened at: {DateTime.Now}");
        }

        /// <summary>
        /// Upload method uploads folder periodically when service is active
        /// </summary>
        /// <param name="list">List of PeriodicalUpload objects</param>
        public void Upload(IEnumerable<PeriodicalUpload> periodicalUploads)
        {

            foreach (var item in periodicalUploads)
            {
                var timer = new Timer
                {
                    Enabled = true,
                    Interval = TimeSpan.FromMinutes(item.Interval).TotalMilliseconds,
                    AutoReset = true
                };


                timer.Elapsed += (sender, e) => OnTimedEvent(item);
            }
        }


        /// <summary>
        /// Method OnTimedEvent calls UploadFolder method when timer elapsed event happen
        /// </summary>
        /// <param name="item"> PeriodicalUpload object </param>
        private void OnTimedEvent(PeriodicalUpload item)
        {
            UploadFolder(item);
        }

        /// <summary>
        /// UploadFolder method calls methods for upload, clean outdated files and arhive from services for all files in folder
        /// </summary>
        /// <param name="item">PeriodicalUpload object</param>
        private void UploadFolder(PeriodicalUpload item)
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
        
            
        }

        /// <summary>
        /// CreateUploadFileDto creates instance of UploadFileBackupDto.
        /// </summary>
        /// <param name="item">PeriodicalUpload object</param>
        /// <param name="filePath">string</param>
        /// <returns>UploadFileBackupDto object</returns>
        private UploadFileBackupDto CreateUploadFileDto(PeriodicalUpload item, string filePath)
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