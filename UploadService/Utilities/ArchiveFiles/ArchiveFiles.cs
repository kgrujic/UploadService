using System.IO;

namespace UploadService.Utilities.ArchiveFiles
{
    public class ArchiveFiles : IArchive
    {
        public void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath)
        {
            File.Copy(sourceFilePath, backupFilePath, true);
          
        }
    }
}