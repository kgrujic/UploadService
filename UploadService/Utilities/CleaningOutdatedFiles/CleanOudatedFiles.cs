using System;
using System.IO;
using System.Linq;

namespace UploadService.Utilities.CleaningOutdatedFiles
{
    public class CleanOudatedFiles : IClineable
    {
        public void CleanOutdatedFilesOnDays(string folderPath,string fileMask, int numberOfDays)
        {
            Directory.EnumerateFiles(folderPath,fileMask, SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastWriteTime < DateTime.Now.AddDays(-numberOfDays))
                .ToList()
                .ForEach(f => f.Delete());
            
        }
        
    }
}