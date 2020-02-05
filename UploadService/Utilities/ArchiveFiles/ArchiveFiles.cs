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
        
        /// <summary>
        /// MoveFileToArchiveFolder method call methods from IOHelper service to handle moving files to Archive Folder
        /// </summary>
        /// <param name="sourceFilePath">string</param>
        /// <param name="backupFilePath">string</param>
        public void MoveFileToArchiveFolder(string sourceFilePath, string backupFilePath)
        {
            try
            {
                _ioHelper.CreateDirectoryIfNotExist(Path.GetDirectoryName(backupFilePath));
                _ioHelper.MoveFile(sourceFilePath,backupFilePath);
              
            }
            catch(IOException e)
            {
                Console.WriteLine($"Exception occured" + e.Message);
             
            }
        }
    }
}