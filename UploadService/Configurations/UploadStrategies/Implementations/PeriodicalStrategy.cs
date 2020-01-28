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
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
using UploadServiceDatabase.Repositories;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class PeriodicalStrategy : IUploadStrategy
    {
        private IEnumerable<PeriodicalUpload> _foldersToUpload;
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;

        public PeriodicalStrategy(IEnumerable<IUploadTypeConfiguration> foldersToUpload, IUpload upload, IArchive archive, IClineable clean)
        {
            _foldersToUpload = foldersToUpload.Cast<PeriodicalUpload>();
            _upload = upload;
            _archive = archive;
            _clean = clean;
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

           // _ioHelper.CreateDirectoryIfNotExist(archiveFolder);

            foreach (string filePath in Directory.EnumerateFiles(localFolderPath,  fileMask,
                SearchOption.AllDirectories))
            {

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