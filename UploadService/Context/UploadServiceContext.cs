using Microsoft.EntityFrameworkCore;
using UploadService.DTOs;

namespace UploadService.Context
{
    public class UploadServiceContext : DbContext, IUploadServiceContext
    {
        public void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=UploadServiceDatabase.db");
        
        public DbSet<FileDTO> Files { get; set; }
    }
}