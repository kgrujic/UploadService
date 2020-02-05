using System.IO;

namespace UploadService.Utilities.IO_Helpers
{
  
    public class IoHelper : IIoHelper
    {
        public void CreateDirectoryIfNotExist(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
        }

        public void MoveFile(string sourceFilePath, string destinationFilePath)
        {
            File.Move(sourceFilePath,destinationFilePath,true);
        }
        public void CopyFile(string sourceFilePath, string destinationFilePath)
        {
            File.Copy(sourceFilePath, destinationFilePath, true);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}