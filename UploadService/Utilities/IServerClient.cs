namespace UploadService.Utilities
{
    public interface IServerClient
    {
        public void UploadFile(string remoteFile, string localFFile);
    }
}