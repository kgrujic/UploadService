namespace UploadService.DTOs
{
    /// <summary>
    /// Helper dto for uploading with backup handling
    /// </summary>
    public class UploadFileBackupDto
    {
        public string LocalFilePath { get; set; }
        public string RemoteFolder { get; set; }
        public string ArchiveFolder { get; set; }
        public string FileMask { get; set; }
        public int CleanUpDays { get; set; }
    }
}