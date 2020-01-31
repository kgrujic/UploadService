namespace UploadService.Utilities.IO_Helpers
{
    public interface IIOHelper
    {
        /*void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath);*/
        void CreateDirectoryIfNotExist(string folderPath);
        
        bool FileExists(string fileName);

        public void DeleteFile(string filePath);

        void CopyFile(string sourceFilePath, string backupFilePath);
      
    }
}