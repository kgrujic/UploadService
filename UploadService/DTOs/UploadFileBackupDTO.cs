namespace UploadService.DTOs
{
    public class UploadFileBackupDTO
    {
        public string localFilePath { get; set; }
        public string remoteFolder { get; set; }
        public string archiveFolder { get; set; }
        public string fileMask { get; set; }
        public int cleanUpDays { get; set; }
    }
}