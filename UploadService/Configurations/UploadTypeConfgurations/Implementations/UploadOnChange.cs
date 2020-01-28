namespace UploadService.Configurations.UploadTypeConfgurations.Implementations
{
    public class UploadOnChange : IUploadTypeConfiguration
    {
        public string RemoteFolder { get; set; }
        public string LocalFolderPath { get; set; }
        public string ArchiveFolder { get; set; }
        public int CleanUpPeriodDays { get; set; }
        public string FileMask { get; set; }
        public string LocalFilePath { get; set; }
    }
}