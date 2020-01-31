using System;
using System.IO;
using System.Linq;

namespace UploadService.Utilities.IO_Helpers
{
    // TODO create interface
    public class IOHelper : IIOHelper
    {

        public void CreateDirectoryIfNotExist(string folderPath)
        {
            // Check if exists and create
            Directory.CreateDirectory(folderPath);
        }

        public bool FileExists(string fileName)
        {
            throw new NotImplementedException();
        }

        public void CopyFile(string sourceFilePath, string backupFilePath)
        {
            File.Copy(sourceFilePath, backupFilePath, true);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}