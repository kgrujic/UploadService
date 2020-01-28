using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UploadService.Configurations.ServerConfigurations;
using UploadService.Configurations.UploadStrategies;
using UploadService.Configurations.UploadStrategies.Implementations;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.DTOs;
using UploadService.Utilities;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadServiceDatabase.DTOs;
using UploadServiceDatabase.Repositories;

namespace UploadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public IEnumerable<IUploadTypeConfiguration> PeriodicalUploads;
        public IEnumerable<IUploadTypeConfiguration> TimeSpecificUploads;
        public IEnumerable<IUploadTypeConfiguration> OnChangeUploads;
        public IEnumerable<IUploadTypeConfiguration> OnCreateUploads;
        
        
        public IServerConfiguration ftpServerConfiguration;
        public IServerClient client;
        public IIOHelper IoHelper;
        public IHashHelper hashHelper;
        public IUploadServiceRepository repository;
        
      
        
        private IUploadStrategy _PeriodicalStrategy;
        private IUploadStrategy _TimeStrategy;
        private IUploadStrategy _OnChangeStrategy;
        private IUploadStrategy _OnCreateStrategy;


        public Worker(ILogger<Worker> logger,IOptions<AppSettings> settings)
        {
            
           PeriodicalUploads = settings.Value.PeriodicalUploads;
           TimeSpecificUploads = settings.Value.TimeSpecificUploads;
           OnChangeUploads = settings.Value.OnChangeUploads;
           OnCreateUploads = settings.Value.OnCreateUploads;
           
           ftpServerConfiguration = settings.Value.ftpServerConfiguration;
           client = new FTPClient(ftpServerConfiguration.HostAddress,ftpServerConfiguration.Username,ftpServerConfiguration.Password);
           
           IoHelper = new IOHelper();
           repository = new UploadServiceRepository();
           hashHelper = new HashHelper(client,repository);


               //TODO add context to other strategies
           _PeriodicalStrategy = new PeriodicalStrategy(PeriodicalUploads, client, IoHelper,hashHelper,repository);
           _TimeStrategy = new TimeSpecificStrategy(TimeSpecificUploads, client, IoHelper, hashHelper, repository);
           _OnChangeStrategy = new OnChangeStrategy(client,IoHelper,OnChangeUploads, hashHelper, repository);
           _OnCreateStrategy = new OnCreateStrategy(client,IoHelper,OnCreateUploads, hashHelper, repository);
            _logger = logger;
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker started at: {DateTime.Now}");
            
            foreach (var file in OnChangeUploads.Cast<UploadOnChange>())
            {
                var localHash = hashHelper.GenerateHash(file.LocalFilePath);
                if (!repository.FileExistInDatabase(file.LocalFilePath))
                {
                    repository.InsertFile(new FileDTO
                    {
                        FilePath = file.LocalFilePath, 
                        HashedContent = localHash
                    });
                }
                else if(!hashHelper.HashMatching(localHash, repository.GetFileByPath(file.LocalFilePath).HashedContent))
                {
                    hashHelper.UploadFileOnChange(file.LocalFilePath, file.RemoteFolder, localHash);
                }
              
            }
            
            HandleOnStart(PeriodicalUploads);
            HandleOnStart(TimeSpecificUploads);
            HandleOnStart(OnCreateUploads);

            /*foreach (var item in PeriodicalUploads.Cast<PeriodicalUpload>())
            {
                foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask, SearchOption.AllDirectories))
                {
                    var localHash = hashHelper.GenerateHash(filePath);
                    
                    if (!repository.FileExistInDatabase(filePath))
                    {
                        repository.InsertFile(new FileDTO
                        {
                            FilePath = filePath, 
                            HashedContent = localHash
                        });
                    }
                    else if(!hashHelper.HashMatching(localHash, repository.GetFileByPath(filePath).HashedContent))
                    {
                        //hashHelper.UploadFileOnChange(filePath, item.RemoteFolder, localHash);
                        hashHelper.UploadFileWithBackupHandling(new UploadFileBackupDTO
                        {
                            archiveFolder = item.ArchiveFolder,
                            cleanUpDays = item.CleanUpPeriodDays,
                            fileMask = item.FileMask,
                            localFilePath = filePath,
                            remoteFolder = item.RemoteFolder
                        }, localHash, IoHelper);
                    }
                }
                
            }    
            foreach (var item in TimeSpecificUploads.Cast<TimeSpecificUpload>())
            {
                foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask, SearchOption.AllDirectories))
                {
                    var localHash = hashHelper.GenerateHash(filePath);
                    if (!repository.FileExistInDatabase(filePath))
                    {
                        repository.InsertFile(new FileDTO
                        {
                            FilePath = filePath, 
                            HashedContent = localHash
                        });
                    }else if(!hashHelper.HashMatching(localHash, repository.GetFileByPath(filePath).HashedContent))
                    {
                        hashHelper.UploadFileOnChange(filePath, item.RemoteFolder, localHash);
                    }
                }
                
            }*/
 
            return base.StartAsync(cancellationToken);
        }

        private void HandleOnStart(IEnumerable<IUploadTypeConfiguration> list) 
        {
            foreach (var item in list.ToList())
            {
                foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask, SearchOption.AllDirectories))
                {
                    //var localHash = hashHelper.GenerateHash(filePath);
                    
                    
                    
                    
                        //hashHelper.UploadFileOnChange(filePath, item.RemoteFolder, localHash);
                        hashHelper.UploadFileWithBackupHandling(new UploadFileBackupDTO
                        {
                            archiveFolder = item.ArchiveFolder,
                            cleanUpDays = item.CleanUpPeriodDays,
                            fileMask = item.FileMask,
                            localFilePath = filePath,
                            remoteFolder = item.RemoteFolder
                        }, IoHelper);
                    
                }
                
            }    
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
    
             //_PeriodicalStrategy.Upload();
             //_TimeStrategy.Upload();
             //_OnChangeStrategy.Upload();
             _OnCreateStrategy.Upload();
             

        }
    }
}