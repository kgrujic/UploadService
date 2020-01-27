namespace UploadService.Utilities.Clients
{
    public interface IServerClient
    {
        public void UploadFile(string remoteFile, string localFFile, bool overwrite);
        public void delete(string deleteFile);
        
        public bool checkIfFileExists(string filePath);

        public bool directoryExists(string directory);
    }
}