using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities;
using UploadService.Utilities.UploadFiles;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class OnChangeStrategy : IUploadStrategy
    {
        private IEnumerable<UploadOnChange> _filesToUpload;
        private IUpload _upload;


        private List<MyFileSystemWatcher> watchers;

        public OnChangeStrategy(IEnumerable<IUploadTypeConfiguration> filesToUpload, IUpload upload)
        {
            _filesToUpload = filesToUpload.Cast<UploadOnChange>();
            _upload = upload;
        }


        public void Upload()
        {
            watchers = new List<MyFileSystemWatcher>();

            foreach (var file in _filesToUpload)
            {
                MyFileSystemWatcher watcher = CreateWatcher(file);
                watchers.Add(watcher);
            }

            AddEventHandlers();
        }

        void AddEventHandlers()
        {
            foreach (var w in watchers)
            {
                w.Renamed += async (sender, e) =>
                {
                    var localFilePath = Path.Combine(w.Path, w.Filter);
                    var remoteFolder = w.RemoteFolder;
                    await OnChangeEvent(localFilePath, remoteFolder);
                };
            }
        }

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


        private async Task OnChangeEvent(string localFilePath, string remoteFolder)
        {
            await _upload.UploadFile(localFilePath, remoteFolder);
        }
    }
}