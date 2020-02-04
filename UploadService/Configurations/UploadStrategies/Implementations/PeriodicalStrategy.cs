using System.Collections.Generic;
using System.IO;
using System.Timers;
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
            StartUpUpload(periodicalUploads);
            
            foreach (var item in periodicalUploads)
            {
                var timer = new Timer
                {
                    Enabled = true,
                    Interval = item.Interval,
                    AutoReset = true
                };
                

                timer.Elapsed += (sender, e) =>  OnTimedEvent(item);
            }
        }

        public void StartUpUpload(IEnumerable<PeriodicalUpload> list)
        {
            foreach (var item in list)
            {
                UploadFolder(item);
            }
        }


        private void OnTimedEvent(PeriodicalUpload item)
        {
            
            UploadFolder(item);
            
        }

        private void UploadFolder(PeriodicalUpload item)
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