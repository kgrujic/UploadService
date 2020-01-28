namespace UploadService.Configurations.UploadTypeConfgurations
{
    public interface IUploadTypeConfiguration
    {
        
        public string RemoteFolder { get; set; }
        public string LocalFolderPath { get; set; }
        
        string ArchiveFolder { get; set; }
        int CleanUpPeriodDays { get; set; }
        string FileMask { get; set; }
    }
}