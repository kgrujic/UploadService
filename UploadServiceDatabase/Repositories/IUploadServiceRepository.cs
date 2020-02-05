using System;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Repositories
{
    public interface IUploadServiceRepository 
    {
        FileDTO GetFileByPath(string path);
        public bool FileExistInDatabase(string path);
        void InsertFile(FileDTO file);
        void DeleteFile(string path);
        void UpdateFile(FileDTO file);
        
    }
}