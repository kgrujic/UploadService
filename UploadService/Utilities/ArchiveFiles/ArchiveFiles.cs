using System;
using System.IO;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Utilities.ArchiveFiles
{
    /// <summary>
    /// ArchiveFiles class handle archiving of files
    /// Implements IArchive interface
    /// </summary>
    public class ArchiveFiles : IArchive
    {
        private IIoHelper _ioHelper;

        public ArchiveFiles(IIoHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }
        
        public void MoveFileToArchiveFolder(string sourceFilePath, string backupFilePath)
        {
            try
            {
                _ioHelper.CreateDirectoryIfNotExist(Path.GetDirectoryName(backupFilePath));
                _ioHelper.CopyFile(sourceFilePath,backupFilePath);
                _ioHelper.DeleteFile(sourceFilePath);
            }
            catch(IOException e)
            {
                Console.WriteLine($"Exception occured" + e.Message);
             
            }
        }
    }
}