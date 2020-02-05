namespace UploadService.Configurations.UploadTypeConfgurations.Implementations
{
    /// <summary>
    /// Upload On Create type
    /// Implements IUploadTypeConfiguration
    /// </summary>
    public class UploadOnCreate : IUploadTypeConfiguration
    {
        public string LocalFolderPath { get; set; }
        public string RemoteFolder { get; set; }
        public string FileMask { get; set; }
        public string ArchiveFolder { get; set; }
        public int CleanUpPeriodDays { get; set; }
    }
}