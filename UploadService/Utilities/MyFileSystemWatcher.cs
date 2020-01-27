using System.IO;

namespace UploadService.Utilities
{
    public class MyFileSystemWatcher : FileSystemWatcher
    {
        public string RemoteFolder { get; set; }
        public string archiveFolder { get; set; }
        public string fileMask { get; set; }
        public int cleanUpDays { get; set; }
    }
}