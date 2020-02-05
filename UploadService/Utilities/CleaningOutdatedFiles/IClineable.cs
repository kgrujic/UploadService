namespace UploadService.Utilities.CleaningOutdatedFiles
{
    /// <summary>
    /// Clean outdated files service interface
    /// </summary>
    public interface IClineable
    {
        public void CleanOutdatedFilesOnDays(string folderPath, string fileMask, int numberOfDays);
    }
}