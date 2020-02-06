using Microsoft.EntityFrameworkCore;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Context
{
    /// <summary>
    /// UploadServiceContext class implements DbContext(Entity Framework Core) and IUploadServiceContext
    /// Database abstraction
    /// </summary>
    public class UploadServiceContext : DbContext, IUploadServiceContext
    {
        public UploadServiceContext(DbContextOptions<UploadServiceContext> options) : base(options)
        {
        }

        /// <summary>
        /// Files are DbSet of FileDto objects, abstraction for Files Data Table
        /// </summary>
        public DbSet<FileDTO> Files { get; set; }
    }
}