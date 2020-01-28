namespace UploadService.Utilities.ArchiveFiles
{
    public interface IArchive
    {
        public void SaveFileToArchiveFolder(string sourceFilePath, string backupFilePath);
    }
}