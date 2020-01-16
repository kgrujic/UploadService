namespace UploadService.Configurations.UploadTypeConfgurations
{
    public interface IUploadTypeConfiguration
    {
        
        public string RemoteFolder { get; set; }
        public string LocalFolderPath { get; set; }
    }
}