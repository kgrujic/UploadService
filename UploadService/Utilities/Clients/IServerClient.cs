namespace UploadService.Utilities.Clients
{
    /// <summary>
    /// IServerClient contains signatures for methods for Uploading and Check existence of file on Server
    /// </summary>
    public interface IServerClient
    {
        public void UploadFile(string remoteFile, string localFFile, bool overwrite);
     
        
        public bool CheckIfFileExists(string filePath);

       
    }
}