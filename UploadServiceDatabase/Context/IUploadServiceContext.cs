using System;
using Microsoft.EntityFrameworkCore;
using UploadServiceDatabase.DTOs;

namespace UploadServiceDatabase.Context
{
    public interface IUploadServiceContext : IDisposable
    {
        DbSet<FileDTO> Files { get; set; }
    }
}