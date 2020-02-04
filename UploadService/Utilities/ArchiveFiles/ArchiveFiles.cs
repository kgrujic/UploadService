using System;
using System.IO;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Utilities.ArchiveFiles
{
    public class ArchiveFiles : IArchive
    {
        private IIoHelper _ioHelper;

        public ArchiveFiles(IIoHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }
        
        public void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath)
        {
            try
            {
                _ioHelper.CreateDirectoryIfNotExist(Path.GetDirectoryName(backupFilePath));
                _ioHelper.CopyFile(sourceFilePath,backupFilePath);
                _ioHelper.DeleteFile(sourceFilePath);
            }
            catch(DirectoryNotFoundException e)
            {
                //Console.WriteLine($"Exception occured");
                //throw;
            }
        }
    }
}