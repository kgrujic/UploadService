namespace UploadService.Utilities.ArchiveFiles
{
    /// <summary>
    /// Archive service interface
    /// </summary>
    public interface IArchive
    {
        public void MoveFileToArchiveFolder(string sourceFilePath, string backupFilePath);
    }
}