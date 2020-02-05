namespace UploadService.Configurations.UploadTypeConfgurations.Implementations
{
    /// <summary>
    /// Periodical Upload type
    /// Implements IUploadTypeConfiguration
    /// </summary>
    public class PeriodicalUpload : IUploadTypeConfiguration
    {
        public string LocalFolderPath { get; set; }
        public string RemoteFolder { get; set; }
        public string FileMask { get; set; }
        public int Interval { get; set; }
        public string ArchiveFolder { get; set; }
        public int CleanUpPeriodDays { get; set; }
    }
}