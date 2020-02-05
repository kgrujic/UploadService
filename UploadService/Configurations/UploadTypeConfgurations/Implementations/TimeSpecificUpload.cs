using System;

namespace UploadService.Configurations.UploadTypeConfgurations.Implementations
{
    /// <summary>
    /// Time Specific Upload type
    /// Implements IUploadTypeConfiguration
    /// </summary>
    public class TimeSpecificUpload : IUploadTypeConfiguration
    {
        public string LocalFolderPath { get; set; }
        public string RemoteFolder { get; set; }
        public string FileMask { get; set; }
        public string ArchiveFolder { get; set; }
        public int CleanUpPeriodDays { get; set; }
        public DateTime Time { get; set; }
    }
}