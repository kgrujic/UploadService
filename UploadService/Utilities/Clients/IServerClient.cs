namespace UploadService.Utilities.Clients
{
    public interface IServerClient
    {
        public void UploadFile(string remoteFile, string localFFile, bool overwrite);
     
        
        public bool CheckIfFileExists(string filePath);

       
    }
}