using System;
using Microsoft.EntityFrameworkCore;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Context
{
    /// <summary>
    /// IUploadServiceContext interface for Upload Service Context
    /// Implements IDisposable interface
    /// </summary>
    public interface IUploadServiceContext : IDisposable
    {
        DbSet<FileDTO> Files { get; set; }
    }
}