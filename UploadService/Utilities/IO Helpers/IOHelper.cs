using System;
using System.IO;
using System.Linq;

namespace UploadService.Utilities.IO_Helpers
{
    // TODO create interface
    public class IoHelper : IIoHelper
    {
        public void CreateDirectoryIfNotExist(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
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