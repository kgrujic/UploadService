namespace UploadService.Utilities.IO_Helpers
{
    public interface IIoHelper
    {
        void CreateDirectoryIfNotExist(string folderPath);
        
        public void DeleteFile(string filePath);

        void CopyFile(string sourceFilePath, string destinationFilePath);
      
    }
}