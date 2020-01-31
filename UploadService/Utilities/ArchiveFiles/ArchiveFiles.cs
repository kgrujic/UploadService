using System;
using System.IO;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Utilities.ArchiveFiles
{
    public class ArchiveFiles : IArchive
    {
        private IIOHelper _ioHelper;

        public ArchiveFiles(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }
        //TODO delete file
        public void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath)
        {
            try
            {
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