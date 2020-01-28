using System.Threading.Tasks;
using UploadService.DTOs;

namespace UploadService.Utilities.UploadFiles
{
    public interface IUpload
    {
        public Task UploadFile(string localFilePath, string remoteFolder);
       
    }
}