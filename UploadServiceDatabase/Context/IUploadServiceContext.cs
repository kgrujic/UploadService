using Microsoft.EntityFrameworkCore;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Context
{
    public interface IUploadServiceContext 
    {
        DbSet<FileDTO> Files { get; set; }
    }
}