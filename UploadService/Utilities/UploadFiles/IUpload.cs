using System.Threading.Tasks;
using UploadService.DTOs;

namespace UploadService.Utilities.UploadFiles
{
    /// <summary>
    /// IUpload interface for upload service
    /// </summary>
    public interface IUpload
    {
        public Task UploadFile(string localFilePath, string remoteFolder);
       
    }
}