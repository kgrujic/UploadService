using System.IO;

namespace UploadService.Utilities
{
    public class MyFileSystemWatcher : FileSystemWatcher
    {
        public string RemoteFolder { get; set; }
    }
}