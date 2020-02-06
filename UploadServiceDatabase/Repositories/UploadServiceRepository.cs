using System.Linq;
using UploadServiceDatabase.Context.ContextFactory;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Repositories
{
    /// <summary>
    /// UploadServiceRepository contains methods for database DML operations with help of UploadService Context
    /// Implements IUploadServiceRepository interface 
    /// </summary>
    public class UploadServiceRepository : IUploadServiceRepository
    {
        
        private IContextFactory _contextFactory;
        public UploadServiceRepository(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;

        }

        /// <summary>
        /// GetFileByPath method is finding and returning FileDto object from database where primary key is equal to one that is sent
        /// </summary>
        /// <param name="path">string</param>
        /// <returns>FileDTO</returns>
        public FileDTO GetFileByPath(string path)
        {
            
            using (var context = _contextFactory.CreateContext())
            {
                return context.Files.Find(path);
            }
            
        } 
        
        /// <summary>
        /// FileExistInDatabase method checks if FileDto object with given primary key exists in database
        /// </summary>
        /// <param name="path">string</param>
        /// <returns>bool</returns>
        public bool FileExistInDatabase(string path)
        {
            using (var context =_contextFactory.CreateContext())
            {
                var b = context.Files.Any(f => f.FilePath == path);
                return b;
                
            }

        }
        /// <summary>
        /// InsertFile inserts sent FileDto object to database
        /// </summary>
        /// <param name="file">FileDTO object</param>
        public void InsertFile(FileDTO file)
        {
            using (var context =_contextFactory.CreateContext())
            {
                context.Files.Add(file);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// DeleteFile method deletes FileDto object with given primary key from database 
        /// </summary>
        /// <param name="path"></param>
        public void DeleteFile(string path)
        {
            using (var context =_contextFactory.CreateContext())
            {
                FileDTO file = context.Files.Find(path);
                context.Files.Remove(file);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// UpdateFile method updates given FileDto object in database
        /// </summary>
        /// <param name="file"></param>
        public  void UpdateFile(FileDTO file)
        {
            using (var context =_contextFactory.CreateContext())
            {
                context.Files.Update(file);
                context.SaveChanges();
            }
        }

 
    }
}