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
    public class PeriodicalStrategy : IUploadStrategy<PeriodicalUpload>
    {
       
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;

        public PeriodicalStrategy(IUpload upload,
            IArchive archive, IClineable clean)
        {
            
            _upload = upload;
            _archive = archive;
            _clean = clean;
        }
        

        public void Upload(IEnumerable<PeriodicalUpload> periodicalUploads)
        {
           // List<Timer> timerMatrix = new List<Timer>();

           foreach (var item in periodicalUploads)
            {
                var timer = new System.Timers.Timer()
                {
                    Enabled = true,
                    Interval = item.Interval,
                    AutoReset = true
                };

               // timerMatrix.Add(timer);

                timer.Elapsed += (sender, e) =>  OnTimedEvent(item);
            }
        }


        private void OnTimedEvent(PeriodicalUpload item)
        {
            /*var remoteFolder = item.RemoteFolder;
            var fileMask = item.FileMask;
            var archiveFolder = item.ArchiveFolder;
            var cleanUpDays = item.CleanUpPeriodDays;
            var localFolderPath = item.LocalFolderPath;

            // _ioHelper.CreateDirectoryIfNotExist(archiveFolder);

            foreach (string filePath in Directory.EnumerateFiles(localFolderPath, fileMask, SearchOption.AllDirectories))
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
            }*/
            
            HandleMyEvent(item);
        }

        public void HandleMyEvent(PeriodicalUpload item)
        {
            var remoteFolder = item.RemoteFolder;
            var fileMask = item.FileMask;
            var archiveFolder = item.ArchiveFolder;
            var cleanUpDays = item.CleanUpPeriodDays;
            var localFolderPath = item.LocalFolderPath;

            // _ioHelper.CreateDirectoryIfNotExist(archiveFolder);
          
            foreach (string filePath in Directory.EnumerateFiles(localFolderPath, fileMask, SearchOption.AllDirectories))
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