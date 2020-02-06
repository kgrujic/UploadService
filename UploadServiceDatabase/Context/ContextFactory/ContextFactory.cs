using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UploadServiceDatabase.Context.ContextFactory
{
    /// <summary>
    /// ContextFactory class contains method for context creation
    /// Implements IContextFactory inteface
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        private UploadServiceContext _context;

        public ContextFactory(UploadServiceContext context)
        {
            _context = context;
        }
        /// <summary>
        /// CreateContext method creates new instance of UploadServiceContext class
        /// </summary>
        /// <returns>UploadServiceContext object</returns>
        public UploadServiceContext CreateContext()
        {
            return new UploadServiceContext(_context.GetService<DbContextOptions<UploadServiceContext>>());
        }
    }
}