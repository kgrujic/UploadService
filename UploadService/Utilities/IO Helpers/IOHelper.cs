using System;
using System.IO;
using System.Linq;

namespace UploadService.Utilities.IO_Helpers
{
    // TODO create interface
    public class IOHelper : IIOHelper
    {
        /*public void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath)
        {
            File.Copy(sourceFilePath, backupFilePath, true);
          
        }*/

        public void CreateDirectoryIfNotExist(string folderPath)
        {
            // Check if exists and create
            Directory.CreateDirectory(folderPath);
        }
        /*public void CleanOutdatedFiles(string folderPath,string fileMask, int numberOfDays)
        {
            Directory.EnumerateFiles(folderPath,"*"+fileMask, SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastWriteTime < DateTime.Now.AddDays(-numberOfDays))
                .ToList()
                .ForEach(f => f.Delete());
            
        }*/
    }
}