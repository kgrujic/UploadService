namespace UploadService.Configurations.UploadTypeConfgurations
{
    /// <summary>
    /// Upload Type Configuration Interface
    /// </summary>
    public interface IUploadTypeConfiguration
    {
        public string RemoteFolder { get; set; }
        public string LocalFolderPath { get; set; }

        string ArchiveFolder { get; set; }
        int CleanUpPeriodDays { get; set; }
        string FileMask { get; set; }
    }
}