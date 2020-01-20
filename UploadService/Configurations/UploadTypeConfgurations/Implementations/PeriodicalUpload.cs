using System;

namespace UploadService.Configurations.UploadTypeConfgurations.Implementations
{
    public class PeriodicalUpload : IUploadTypeConfiguration
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string LocalFolderPath { get; set; }
        public string RemoteFolder { get; set; }
        public string FileMask { get; set; }
        public int Interval { get; set; }
        public string ArchiveFolder { get; set; }
        public int CleanUpPeriodDays { get; set; }
    }
}