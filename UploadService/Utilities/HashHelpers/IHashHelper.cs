using System.Threading.Tasks;
using UploadService.DTOs;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Utilities.HashHelpers
{
    /// <summary>
    /// IHashHelper interface for HashHelper service
    /// </summary>
    public interface IHashHelper
    {
        byte[] GenerateHash(string path);
        bool HashMatching(byte[] hashFirst, byte[] hashSecond);
    }
}