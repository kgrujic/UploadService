namespace UploadService.Utilities.CleaningOutdatedFiles
{
    public interface IClineable
    {
        public void CleanOutdatedFilesOnDays(string folderPath, string fileMask, int numberOfDays);
    }
}