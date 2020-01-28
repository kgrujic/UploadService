using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using UploadServiceDatabase.Context;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Repositories
{
    public class UploadServiceRepository : IUploadServiceRepository
    {
       
        public UploadServiceRepository()
        {
           
        }

        public FileDTO GetFileByPath(string path)
        {
            using (var context = new UploadServiceContext())
            {
                return context.Files.Find(path);
            }
            
        } 
        
        public bool FileExistInDatabase(string path)
        {
            using (var context = new UploadServiceContext())
            {
                var b = context.Files.Any(f => f.FilePath == path);
                return b;
                
            }

        }

        public void InsertFile(FileDTO file)
        {
            using (var context = new UploadServiceContext())
            {
                context.Files.Add(file);
                context.SaveChanges();
            }
        }

        public void DeleteFile(string path)
        {
            using (var context = new UploadServiceContext())
            {
                FileDTO file = context.Files.Find(path);
                context.Files.Remove(file);
                context.SaveChanges();
            }
        }

        public void UpdateFile(FileDTO file)
        {
            using (var context = new UploadServiceContext())
            {
                context.Files.Update(file);
                context.SaveChanges();
            }
        }
        
        public void Save()        
        {
            using (var context = new UploadServiceContext())
            {
                context.SaveChanges();      
            }      
           
        }        
    
        private bool disposed = false;        
    
        protected virtual void Dispose(bool disposing)        
        {           using (var context = new UploadServiceContext()){       
            if (!this.disposed)            
            {                
                if (disposing)                
                {                    
                    context.Dispose();                
                }
            }            
            this.disposed = true;   
            }
        }        
    
        public void Dispose()        
        {            
            Dispose(true);            
            GC.SuppressFinalize(this);        
        }   
    }
}