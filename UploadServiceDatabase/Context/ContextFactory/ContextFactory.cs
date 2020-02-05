using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UploadServiceDatabase.Context.ContextFactory
{
    public class ContextFactory : IContextFactory
    {
        private UploadServiceContext _context;

        public ContextFactory(UploadServiceContext context)
        {
            _context = context;
        }

        public UploadServiceContext CreateContext()
        {
            return new UploadServiceContext(_context.GetService<DbContextOptions<UploadServiceContext>>());
        }
    }
}