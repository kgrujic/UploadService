using System.IO;

namespace UploadService.Utilities
{
    public class MyFileSystemWatcher : FileSystemWatcher
    {
        public string RemoteFolder { get; set; }
        public string ArchiveFolder { get; set; }
        public string FileMask { get; set; }
        public int CleanUpPeriodDays { get; set; }
    }
}