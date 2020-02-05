using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using UploadServiceDatabase.Context;
using UploadServiceDatabase.Context.ContextFactory;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Repositories
{
    public class UploadServiceRepository : IUploadServiceRepository
    {
        //TODO dependency injection problem
        private IContextFactory _contextFactory;
        public UploadServiceRepository(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;

        }

        public FileDTO GetFileByPath(string path)
        {
            
            using (var context = _contextFactory.CreateContext())
            {
                return context.Files.Find(path);
            }
            
        } 
        
        public bool FileExistInDatabase(string path)
        {
            using (var context =_contextFactory.CreateContext())
            {
                var b = context.Files.Any(f => f.FilePath == path);
                return b;
                
            }

        }

        public void InsertFile(FileDTO file)
        {
            using (var context =_contextFactory.CreateContext())
            {
                context.Files.Add(file);
                context.SaveChanges();
            }
        }

        public void DeleteFile(string path)
        {
            using (var context =_contextFactory.CreateContext())
            {
                FileDTO file = context.Files.Find(path);
                context.Files.Remove(file);
                context.SaveChanges();
            }
        }

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