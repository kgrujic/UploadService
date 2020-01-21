using System;
using System.IO;
using System.Linq;

namespace UploadService.Utilities.IO_Helpers
{
    public class IOHelper
    {
        public void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath)
        {
            using (FileStream stream = File.OpenRead(sourceFilePath))
            using (FileStream writeStream = File.OpenWrite(backupFilePath))
            {
                BinaryReader reader = new BinaryReader(stream);
                BinaryWriter writer = new BinaryWriter(writeStream);

                // create a buffer to hold the bytes 
                byte[] buffer = new Byte[1024];
                int bytesRead;

                // while the read method returns bytes
                // keep writing them to the output stream
                while ((bytesRead =
                           stream.Read(buffer, 0, 1024)) > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        public void CreateDirectoryIfNotExist(string folderPath)
        {
            // Check if exists and create
            Directory.CreateDirectory(folderPath);
        }
        public void CleanOutdatedFiles(string folderPath,string fileMask, int numberOfDays)
        {
            Directory.EnumerateFiles(folderPath,"*"+fileMask, SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastAccessTime < DateTime.Now.AddDays(-numberOfDays))
                .ToList()
                .ForEach(f => f.Delete());
            
        }
    }
}