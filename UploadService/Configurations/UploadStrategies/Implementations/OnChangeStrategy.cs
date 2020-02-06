using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities;
using UploadService.Utilities.UploadFiles;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    /// <summary>
    /// OnChangeStrategy Class Handle uploading files when Change event happened
    /// Implements 'IUploadStrategy'<'UploadOnChange'>'
    /// </summary>
    public class OnChangeStrategy : IUploadStrategy<UploadOnChange>
    {
        private IUpload _upload;
        private ILogger<Worker> _logger;

        private List<MyFileSystemWatcher> _watchers;

        /// <summary>
        /// OnChangeStrategy constructor
        /// </summary>
        public OnChangeStrategy(IUpload upload, ILogger<Worker> logger)
        {
            _upload = upload;
            _logger = logger;
        }

        /// <summary>
        /// StartUpUpload method uploads changes that happened when service was inactive
        /// </summary>
        /// <param name="list">List of UploadOnChange objects</param>
        public void StartUpUpload(IEnumerable<UploadOnChange> list)
        {
            foreach (var file in list)
            {
                _upload.UploadFile(file.LocalFilePath, file.RemoteFolder);
            }
            
            _logger.LogInformation($"Start up upload for Upload On Change list happened at: {DateTime.Now}");
        }


        /// <summary>
        /// Upload method uploads changes in real time when service is active
        /// </summary>
        /// <param name="list">List of UploadOnChange objects</param>
        public void Upload(IEnumerable<UploadOnChange> onChangeUploads)
        {

            _watchers = new List<MyFileSystemWatcher>();

            foreach (var file in onChangeUploads)
            {
                MyFileSystemWatcher watcher = CreateWatcher(file);
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
                w.Renamed += async (sender, e) =>
                {
                    var localFilePath = Path.Combine(w.Path, w.Filter);
                    var remoteFolder = w.RemoteFolder;
                    await OnChangeEvent(localFilePath, remoteFolder);
                };
            }
        }

        /// <summary>
        /// CreateWatcher method creates new instance of MyFileSystemWatcher class 
        /// </summary>
        /// <param name="file">UploadOnChange object</param>
        /// <returns></returns>
        public MyFileSystemWatcher CreateWatcher(UploadOnChange file)
        {
            var watcher = new MyFileSystemWatcher()
            {
                Path = Path.GetDirectoryName(file.LocalFilePath),
                Filter = Path.GetFileName(file.LocalFilePath),
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                               NotifyFilters.FileName | NotifyFilters.DirectoryName,
                RemoteFolder = file.RemoteFolder,
                EnableRaisingEvents = true
            };

            return watcher;
        }


        /// <summary>
        /// OnChangeEvent method is calling UploadFile method from UploadFiles service
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="remoteFolder"></param>
        /// <returns>UploadFileBackupDto object</returns>
        private async Task OnChangeEvent(string localFilePath, string remoteFolder)
        {
            await _upload.UploadFile(localFilePath, remoteFolder);

        }
    }
}