using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.UploadFiles;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class OnCreateStrategy : IUploadStrategy<UploadOnCreate>
    {
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;

        private List<MyFileSystemWatcher> _watchers;

        public OnCreateStrategy(IUpload upload, IArchive archive, IClineable clean)
        {
           
            _upload = upload;
            _archive = archive;
            _clean = clean;
        }
        
        public void StartUpUpload(IEnumerable<UploadOnCreate> list)
        {
            foreach (var item in list)
            {
                UploadFolder(item);
            }
        }


        public void Upload(IEnumerable<UploadOnCreate> onCreateUploads)
        {
            StartUpUpload(onCreateUploads);
            
            _watchers = new List<MyFileSystemWatcher>();

            foreach (var item in onCreateUploads)
            {
                MyFileSystemWatcher watcher = CreateWatcher(item);
                _watchers.Add(watcher);
            }

            AddEventHandlers();
        }

       
        void AddEventHandlers()
        {
            foreach (var w in _watchers)
            {
                w.Created += (s, e) => { OnCreateHandler(s, e, w); };
            }
        }

        private async Task OnCreateHandler(object sender, FileSystemEventArgs e, MyFileSystemWatcher w)
        {
          
            var localFilePath = Path.Combine(w.Path,e.Name);
              
            await OnCreateEvent(CreateUploadFileDto(w,localFilePath));
        }

        private MyFileSystemWatcher CreateWatcher(UploadOnCreate item)
        {
            var watcher = new MyFileSystemWatcher
            {
                Path = item.LocalFolderPath,
                Filter = item.FileMask,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                               NotifyFilters.FileName | NotifyFilters.DirectoryName,
                RemoteFolder = item.RemoteFolder,
                ArchiveFolder = item.ArchiveFolder,
                FileMask = item.FileMask,
                CleanUpPeriodDays = item.CleanUpPeriodDays,
                EnableRaisingEvents = true
            };

            return watcher;
        }

        private async Task OnCreateEvent(UploadFileBackupDto dto)
        {
            await _upload.UploadFile(dto.LocalFilePath, dto.RemoteFolder);
            _clean.CleanOutdatedFilesOnDays(dto.ArchiveFolder,dto.FileMask, dto.CleanUpDays);
            _archive.SaveFileToArchiveFolder(dto.LocalFilePath,  Path.Combine(dto.ArchiveFolder, Path.GetFileName(dto.LocalFilePath)));
        }
        
        private UploadFileBackupDto CreateUploadFileDto(MyFileSystemWatcher item, string filePath)
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
        private UploadFileBackupDto CreateUploadFileDto(UploadOnCreate item, string filePath)
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
        
        private void UploadFolder(UploadOnCreate item)
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

       
    }
}