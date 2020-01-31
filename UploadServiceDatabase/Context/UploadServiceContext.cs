using Microsoft.EntityFrameworkCore;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Context
{
    public class UploadServiceContext : DbContext, IUploadServiceContext
    {
        //TODO to conf
        protected override void  OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=/home/katarina/RiderProjects/UploadService/UploadServiceDatabase/Database/UploadServiceDatabaseNew.db");
        
        public DbSet<FileDTO> Files { get; set; }
    }
}