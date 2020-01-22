using System;
using System.Linq;
using UploadService.Context;
using UploadService.DTOs;

namespace UploadService.Repositories
{
    public class UploadServiceRepository : IUploadServiceRepository
    {
        private UploadServiceContext context;

        public UploadServiceRepository(IUploadServiceContext context)
        {
            this.context = (UploadServiceContext) context;
        }

        public FileDTO GetFileByPath(string path)
        {
            return context.Files.Find(path);
        } 
        
        public bool FileExistInDatabase(string path)
        {
            //Console.WriteLine(path);
            if (context.Files.Count(f => f.FilePath == path) > 0)
            {
                Console.WriteLine("found");
                return true;
            }
           
            return false;

        }

        public void InsertFile(FileDTO file)
        {
            context.Files.Add(file);
            Save();
        }

        public void DeleteFile(string path)
        {
            FileDTO file = context.Files.Find(path);
            context.Files.Remove(file);
            Save();
        }

        public void UpdateFile(FileDTO file)
        {
            context.Files.Update(file);
            Save();
        }
        
        public void Save()        
        {            
            context.SaveChanges();        
        }        
    
        private bool disposed = false;        
    
        protected virtual void Dispose(bool disposing)        
        {            
            if (!this.disposed)            
            {                
                if (disposing)                
                {                    
                    context.Dispose();                
                }
            }            
            this.disposed = true;        
        }        
    
        public void Dispose()        
        {            
            Dispose(true);            
            GC.SuppressFinalize(this);        
        }   
    }
}