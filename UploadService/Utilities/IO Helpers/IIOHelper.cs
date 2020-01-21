namespace UploadService.Utilities.IO_Helpers
{
    public interface IIOHelper
    {
        void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath);
        void CreateDirectoryIfNotExist(string folderPath);
        void CleanOutdatedFiles(string folderPath,string fileMask, int numberOfDays);
    }
}