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
        
        
        public IServerConfiguration ftpServerConfiguration;
        public IServerClient client;
        public IIOHelper IoHelper;
        public IHashHelper hashHelper;
        public IUploadServiceRepository repository;
        
      
        
        private IUploadStrategy _PeriodicalStrategy;
        private IUploadStrategy _TimeStrategy;
        private IUploadStrategy _OnChangeStrategy;


        public Worker(ILogger<Worker> logger,IOptions<AppSettings> settings)
        {
            
           PeriodicalUploads = settings.Value.PeriodicalUploads;
           TimeSpecificUploads = settings.Value.TimeSpecificUploads;
           OnChangeUploads = settings.Value.OnChangeUploads;
           
           ftpServerConfiguration = settings.Value.ftpServerConfiguration;
           client = new FTPClient(ftpServerConfiguration.HostAddress,ftpServerConfiguration.Username,ftpServerConfiguration.Password);
           
           IoHelper = new IOHelper();
           repository = new UploadServiceRepository();
           hashHelper = new HashHelper(client,repository);


               //TODO add context to other strategies
           _PeriodicalStrategy = new PeriodicalStrategy(PeriodicalUploads, client, IoHelper,hashHelper,repository);
           _TimeStrategy = new TimeSpecificStrategy(TimeSpecificUploads, client, IoHelper, hashHelper, repository);
           _OnChangeStrategy = new OnChangeStrategy(client,IoHelper,OnChangeUploads, hashHelper, repository);
           
            _logger = logger;
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker started at: {DateTime.Now}");
            foreach (var file in OnChangeUploads.Cast<UploadOnChange>())
            {
                var localHash = hashHelper.GenerateHash(file.LocalFilePath);
                var dbHash = repository.GetFileByPath(file.LocalFilePath).HashedContent;
                if (!repository.FileExistInDatabase(file.LocalFilePath))
                {
                    repository.InsertFile(new FileDTO
                    {
                        FilePath = file.LocalFilePath, 
                        HashedContent = localHash
                    });
                }
                else if(!hashHelper.HashMatching(localHash, dbHash))
                {
                    hashHelper.UploadFile(file.LocalFilePath, file.RemoteFolder, localHash);
                }
              
            }

            foreach (var item in PeriodicalUploads.Cast<PeriodicalUpload>())
            {
                foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, "*" + item.FileMask, SearchOption.AllDirectories))
                {
                    var localHash = hashHelper.GenerateHash(filePath);
                    var dbHash = repository.GetFileByPath(filePath).HashedContent;
                    if (!repository.FileExistInDatabase(filePath))
                    {
                        repository.InsertFile(new FileDTO
                        {
                            FilePath = filePath, 
                            HashedContent = localHash
                        });
                    }else if(!hashHelper.HashMatching(localHash, dbHash))
                    {
                        hashHelper.UploadFile(filePath, item.RemoteFolder, localHash);
                    }
                }
                
            }    
            foreach (var item in TimeSpecificUploads.Cast<TimeSpecificUpload>())
            {
                foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, "*" + item.FileMask, SearchOption.AllDirectories))
                {
                    var localHash = hashHelper.GenerateHash(filePath);
                    var dbHash = repository.GetFileByPath(filePath).HashedContent;
                    if (!repository.FileExistInDatabase(filePath))
                    {
                        repository.InsertFile(new FileDTO
                        {
                            FilePath = filePath, 
                            HashedContent = localHash
                        });
                    }else if(!hashHelper.HashMatching(localHash, dbHash))
                    {
                        hashHelper.UploadFile(filePath, item.RemoteFolder, localHash);
                    }
                }
                
            }
 
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
    
             _PeriodicalStrategy.Upload();
             //_TimeStrategy.Upload();
             //_OnChangeStrategy.Upload();
             
        }
    }
}