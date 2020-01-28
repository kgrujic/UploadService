using System.Threading.Tasks;
using UploadService.DTOs;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Utilities.HashHelpers
{
    public interface IHashHelper
    {
        byte[] GenerateHash(string path);
        bool HashMatching(byte[] hashFirst, byte[] hashSecond);
    }
}