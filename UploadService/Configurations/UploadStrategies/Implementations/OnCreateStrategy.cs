using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.UploadFiles;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    /// <summary>
    /// OnChangeStrategy Class Handle uploading files when Create event happened
    /// Implements 'IUploadStrategy'<'UploadOnCreate'>'
    /// </summary>
    public class OnCreateStrategy : IUploadStrategy<UploadOnCreate>
    {
        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;

        private List<MyFileSystemWatcher> _watchers;
        
        /// <summary>
        /// OnCreateStrategy constructor
        /// </summary>
        /// <param name="upload"></param>
        public OnCreateStrategy(IUpload upload, IArchive archive, IClineable clean)
        {
           
            _upload = upload;
            _archive = archive;
            _clean = clean;
        }
        /// <summary>
        /// StartUpUpload method uploads changes that happened in watched folder when service was inactive
        /// </summary>
        /// <param name="list">List of UploadOnCreate objects</param>
        public void StartUpUpload(IEnumerable<UploadOnCreate> list)
        {
            foreach (var item in list)
            {
                UploadFolder(item);
            }
        }


        /// <summary>
        /// Upload method uploads changes in real time when service is active
        /// </summary>
        /// <param name="list">List of UploadOnCreate objects</param>
        public void Upload(IEnumerable<UploadOnCreate> onCreateUploads)
        {
            _watchers = new List<MyFileSystemWatcher>();

            foreach (var item in onCreateUploads)
            {
                MyFileSystemWatcher watcher = CreateWatcher(item);
                _watchers.Add(watcher);
            }

            AddEventHandlers();
        }

        /// <summary>
        /// AddEventHandlers method adds event handlers for watchers
        /// </summary>
        void AddEventHandlers()
        {
            foreach (var w in _watchers)
            {
                w.Created += (s, e) => { OnCreateHandler(s, e, w); };
            }
        }

        /// <summary>
        /// OnCreateEvent method calls OnCreateEvent
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">FileSystemEventArgs</param>
        /// <param name="w">MyFileSystemWatcher</param>
        /// <returns></returns>
        private async Task OnCreateHandler(object sender, FileSystemEventArgs e, MyFileSystemWatcher w)
        {
          
            var localFilePath = Path.Combine(w.Path,e.Name);
              
            await OnCreateEvent(CreateUploadFileDto(w,localFilePath));
        }
        
        /// <summary>
        /// CreateWatcher method creates new instance of MyFileSystemWatcher class 
        /// </summary>
        /// <param name="file">UploadOnCreate object</param>
        /// <returns></returns>
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

        /// <summary>
        /// OnCreateEvent calls methods for upload, clean outdated files and arhive from services
        /// </summary>
        /// <param name="dto">UploadFileBackupDto object</param>
        /// <returns></returns>
        private async Task OnCreateEvent(UploadFileBackupDto dto)
        {
            await _upload.UploadFile(dto.LocalFilePath, dto.RemoteFolder);
            _clean.CleanOutdatedFilesOnDays(dto.ArchiveFolder,dto.FileMask, dto.CleanUpDays);
            _archive.MoveFileToArchiveFolder(dto.LocalFilePath,  Path.Combine(dto.ArchiveFolder, Path.GetFileName(dto.LocalFilePath)));
        }
        
        /// <summary>
        /// CreateUploadFileDto creates instance of UploadFileBackupDto
        /// </summary>
        /// <param name="item">MyFileSystemWatcher object</param>
        /// <param name="filePath">string</param>
        /// <returns>UploadFileBackupDto object</returns>
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
        
        /// <summary>
        /// CreateUploadFileDto creates instance of UploadFileBackupDto.
        /// Overloaded method
        /// </summary>
        /// <param name="item">UploadOnCreate object</param>
        /// <param name="filePath">string</param>
        /// <returns>UploadFileBackupDto object</returns>
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
        
        /// <summary>
        /// UploadFolder method uploads all files from from folder
        /// </summary>
        /// <param name="item">UploadOnCreate object</param>
        private async void UploadFolder(UploadOnCreate item)
        {
            foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask, SearchOption.AllDirectories))
            {
                var dto  = CreateUploadFileDto(item, filePath);

               await OnCreateEvent(dto);
               
            }
        }

       
    }
}