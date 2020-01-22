using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using UploadService.DTOs;

namespace UploadService.Context
{
    public interface IUploadServiceContext 
    {
        void OnConfiguring(DbContextOptionsBuilder options);
        DbSet<FileDTO> Files { get; set; }
    }
}