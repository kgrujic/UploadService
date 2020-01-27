namespace UploadService.Utilities.HashHelpers
{
    public interface IHashHelper
    {
        byte[] GenerateHash(string path);
        bool HashMatching(byte[] hashFirst, byte[] hashSecond);
    }
}