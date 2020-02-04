using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using UploadServiceDatabase.Context;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Repositories
{
    public class UploadServiceRepository : IUploadServiceRepository
    {
        //TODO dependency injection problem
        private UploadServiceContext _context;
        public UploadServiceRepository(UploadServiceContext context)
        {
            _context = context;

        }

        public FileDTO GetFileByPath(string path)
        {
            using (_context)
            {
                return _context.Files.Find(path);
            }
            
        } 
        
        public bool FileExistInDatabase(string path)
        {
            using (_context)
            {
                var b = _context.Files.Any(f => f.FilePath == path);
                return b;
                
            }

        }

        public void InsertFile(FileDTO file)
        {
            using (_context)
            {
                _context.Files.Add(file);
                _context.SaveChanges();
            }
        }

        public void DeleteFile(string path)
        {
            using (_context)
            {
                FileDTO file = _context.Files.Find(path);
                _context.Files.Remove(file);
                _context.SaveChanges();
            }
        }

        public  void UpdateFile(FileDTO file)
        {
            using (_context)
            {
                _context.Files.Update(file);
                _context.SaveChanges();
            }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }


        private bool disposed = false;        
    
        protected virtual void Dispose(bool disposing)        
        {           using (_context){       
            if (!this.disposed)            
            {                
                if (disposing)                
                {                    
                    _context.Dispose();                
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