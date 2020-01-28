using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class OnCreateStrategy : IUploadStrategy
    {
        
        private IEnumerable<UploadOnCreate> _folders;
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;

        private List<MyFileSystemWatcher> watchers;

        public OnCreateStrategy(IEnumerable<IUploadTypeConfiguration> folders, IUpload upload, IArchive archive, IClineable clean)
        {
            _folders = folders.Cast<UploadOnCreate>();
            _upload = upload;
            _archive = archive;
            _clean = clean;
        }

        public void Upload()
        {
            watchers = new List<MyFileSystemWatcher>();

            foreach (var folder in _folders)
            {
                MyFileSystemWatcher watcher = CreateWatcher(folder);
                watchers.Add(watcher);
            }

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            foreach (var w in watchers)
            {
                w.Created += async (sender, e) =>
                {
                    Console.WriteLine("created" + e.ChangeType);

                    var localFilePath = w.Path + "/" + e.Name;
              
                    await OnCreateEvent(new UploadFileBackupDTO
                    {
                        archiveFolder = w.archiveFolder,
                        cleanUpDays = w.cleanUpDays,
                        fileMask = w.fileMask,
                        localFilePath = localFilePath,
                        remoteFolder = w.RemoteFolder
                    });
                };
            }
        }

        public MyFileSystemWatcher CreateWatcher(UploadOnCreate folder)
        {
            var watcher = new MyFileSystemWatcher()
            {
                Path = folder.LocalFolderPath,
                Filter = folder.FileMask,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                               NotifyFilters.FileName | NotifyFilters.DirectoryName,
                RemoteFolder = folder.RemoteFolder,
                archiveFolder = folder.ArchiveFolder,
                fileMask = folder.FileMask,
                cleanUpDays = folder.CleanUpPeriodDays,
                EnableRaisingEvents = true
            };

            return watcher;
        }

        private async Task OnCreateEvent(UploadFileBackupDTO dto)
        {
            Console.WriteLine("I am here");
            
            /*_repository.InsertFile(new FileDTO
            {
                FilePath = dto.localFilePath, 
                HashedContent = localHash
            });*/
            
            Console.WriteLine("change happend");

            await _upload.UploadFile(dto.localFilePath, dto.remoteFolder);
            _clean.CleanOutdatedFilesOnDays(dto.archiveFolder,dto.fileMask, dto.cleanUpDays);
            _archive.SaveFileToArchiveFolder(dto.localFilePath,  Path.Combine(dto.archiveFolder, Path.GetFileName(dto.localFilePath)));
        }
    }
}