using System;
using System.IO;
using System.Linq;

namespace UploadService.Utilities.CleaningOutdatedFiles
{
    /// <summary>
    /// CleanOudatedFiles class handle deleting of outdated files
    /// Implements IClineable interface
    /// </summary>
    public class CleanOudatedFiles : IClineable
    {
        /// <summary>
        /// CleanOutdatedFilesOnDays method delete files that are not changed in some period
        /// </summary>
        /// <param name="folderPath">string</param>
        /// <param name="fileMask">string</param>
        /// <param name="numberOfDays">int</param>
        public void CleanOutdatedFilesOnDays(string folderPath, string fileMask, int numberOfDays)
        {
            Directory.EnumerateFiles(folderPath, fileMask, SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastWriteTime < DateTime.Now.AddDays(-numberOfDays))
                .ToList()
                .ForEach(f => f.Delete());
        }
    }
}