using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace UploadService.Utilities.CleaningOutdatedFiles
{
    /// <summary>
    /// CleanOudatedFiles class handle deleting of outdated files
    /// Implements IClineable interface
    /// </summary>
    public class CleanOudatedFiles : IClineable
    {
        private ILogger<Worker> _logger;

        public CleanOudatedFiles(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// CleanOutdatedFilesOnDays method delete files that are not changed in some period
        /// </summary>
        /// <param name="folderPath">string</param>
        /// <param name="fileMask">string</param>
        /// <param name="numberOfDays">int</param>
        public void CleanOutdatedFilesOnDays(string folderPath, string fileMask, int numberOfDays)
        {
            try
            {
                Directory.EnumerateFiles(folderPath, fileMask, SearchOption.AllDirectories)
                    .Select(f => new FileInfo(f))
                    .Where(f => f.LastWriteTime < DateTime.Now.AddDays(-numberOfDays))
                    .ToList()
                    .ForEach(f => f.Delete());
                
                _logger.LogInformation($"Outdated files from {folderPath} location are deleted at: {DateTime.Now}");
            }
            catch(Exception e)
            {
                _logger.LogError($"Exception {e.Message} occured at {DateTime.Now}");
            }
        }
    }
}