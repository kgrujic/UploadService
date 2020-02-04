using Microsoft.EntityFrameworkCore;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Context
{
    public class UploadServiceContext : DbContext, IUploadServiceContext
    {
        
        
        public UploadServiceContext(DbContextOptions<UploadServiceContext> options) : base(options){}
        
        public DbSet<FileDTO> Files { get; set; }
    }
}