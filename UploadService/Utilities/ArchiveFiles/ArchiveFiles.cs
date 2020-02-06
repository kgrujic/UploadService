using System;
using System.IO;
using Microsoft.Extensions.Logging;
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
        private ILogger<Worker> _logger;

        public ArchiveFiles(IIoHelper ioHelper, ILogger<Worker> logger)
        {
            _ioHelper = ioHelper;
            _logger = logger;
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
                
                _logger.LogInformation($"File {Path.GetFileName(sourceFilePath)} from {sourceFilePath} location is archived to {backupFilePath} location at: {DateTime.Now}");
                
              
            }
            catch(IOException e)
            {
                _logger.LogError($"Exception {e.Message} occured at {DateTime.Now}");
             
            }
        }
    }
}