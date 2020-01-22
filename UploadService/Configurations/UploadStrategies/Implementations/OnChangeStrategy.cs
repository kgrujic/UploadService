using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using INotify.Core;
using Microsoft.Extensions.Hosting.Systemd;
using SQLitePCL;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Context;
using UploadService.DTOs;
using UploadService.Repositories;
using UploadService.Utilities;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class OnChangeStrategy: IUploadStrategy
    {
        private IUploadServiceContext _context;
        private IServerClient _client;
        private IIOHelper _ioHelper;
        private IEnumerable<UploadOnChange> _filesToUpload;
        private IUploadServiceRepository _repository;

        public OnChangeStrategy(IUploadServiceContext context, IServerClient client, IIOHelper ioHelper, IEnumerable<IUploadTypeConfiguration> filesToUpload)
        {
            _context = (UploadServiceContext) context;
            _client = client;
            _ioHelper = ioHelper;
            _filesToUpload = filesToUpload.Cast<UploadOnChange>();
            _repository = new UploadServiceRepository(context);
        }

        public void Upload()
        {

            List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

            foreach (var file in _filesToUpload)
            {
                Console.WriteLine(file.LocalFilePath);
                var localFilePathS = file.LocalFilePath;
               // var remoteFolder = file.RemoteFolder;

                    var watcher = new FileSystemWatcher()
                {
                    Path = Path.GetDirectoryName(localFilePathS),
                    Filter = Path.GetFileName(localFilePathS),
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastAccess | NotifyFilters.LastWrite| NotifyFilters.FileName | NotifyFilters.DirectoryName,
                  //  EnableRaisingEvents = true
                    
                };
                    
               
                watchers.Add(watcher);
                watcher.Renamed += (sender, e) => OnChangeEvent(file.LocalFilePath, file.RemoteFolder,e);
                
                watcher.EnableRaisingEvents = true;
                /*foreach (var watch in watchers)
                {
                    watcher.EnableRaisingEvents = true; ;
                    Console.WriteLine("Watching this folder {0}", watcher.Filter);
                }*/





            }

          
        }

        private void OnChangeEvent(string localFilePath, string remoteFolder,FileSystemEventArgs e)
        {
            
                    var localHash = GenerateHash(localFilePath);

                    bool ex = _repository.FileExistInDatabase(localFilePath);
                    Console.WriteLine(ex);

                    if (_repository.FileExistInDatabase(localFilePath))
                    {
                        
                        var hashFromDb = _repository.GetFileByPath(localFilePath).HashedContent;
                        if (!HashMatching(localHash,hashFromDb))
                        {
                            Console.WriteLine("change happend");
                            var remoteFilePath = $"{"home/katarina/" + remoteFolder + "/"}{Path.GetFileName(localFilePath)}";
                            if (_client.checkIfFileExists(remoteFilePath))
                            {
                                _client.delete(remoteFilePath);
                                _client.UploadFile(remoteFilePath,localFilePath);
                                _repository.UpdateFile(new FileDTO()
                                {
                                    FilePath = localFilePath, HashedContent = localHash
                                });
                               
                            }
                            _client.UploadFile(remoteFilePath,localFilePath);
                            _repository.UpdateFile(new FileDTO()
                            {
                                FilePath = localFilePath, HashedContent = localHash
                            });
                            
                        }
                        Console.WriteLine("change did not happen");
                    }
                    else
                    {
                        _repository.InsertFile(new FileDTO()
                        {
                            FilePath = localFilePath, HashedContent = localHash
                        });
                    
                        Console.WriteLine("file added to database");
                    }
                    
        }

        
        private byte[] GenerateHash(string path)
        {
            byte[] tmpHash;
            using (HashAlgorithm hashAlg = HashAlgorithm.Create("MD5"))
            {
                using (FileStream fsA = new FileStream(path, FileMode.Open))
                {
                    // Calculate the hash for the files.
                    tmpHash = hashAlg.ComputeHash(fsA);
                 
                }
            }

            return tmpHash;
        }
        
        private bool HashMatching(byte[] hashFirst, byte[] hashSecond)
        {
            if (BitConverter.ToString(hashFirst) == BitConverter.ToString(hashSecond))
            {
                return true;
            }
            
            return false;
            

        }

        
      
    }
}