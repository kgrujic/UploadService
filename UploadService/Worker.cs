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
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
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
        
        public IUpload _upload;
        public IArchive _archive;
        public IClineable _clean;



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
           
           _upload = new UploadFiles(client,repository, hashHelper);
           _archive = new ArchiveFiles();
           _clean = new CleanOudatedFiles();


               //TODO add context to other strategies
           _PeriodicalStrategy = new PeriodicalStrategy(PeriodicalUploads,_upload,_archive,_clean);
           _TimeStrategy = new TimeSpecificStrategy(TimeSpecificUploads, _upload,_archive,_clean);
           _OnChangeStrategy = new OnChangeStrategy(OnChangeUploads,_upload);
           _OnCreateStrategy = new OnCreateStrategy(OnCreateUploads, _upload,_archive,_clean);
            _logger = logger;
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker started at: {DateTime.Now}");
            
            foreach (var file in OnChangeUploads.Cast<UploadOnChange>())
            {
                /*var localHash = hashHelper.GenerateHash(file.LocalFilePath);
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
                    _upload.UploadFileOnChange(file.LocalFilePath, file.RemoteFolder);
                }*/
                _upload.UploadFile(file.LocalFilePath, file.RemoteFolder);
              
            }
            
            /*HandleOnStart(PeriodicalUploads);
            HandleOnStart(TimeSpecificUploads);
            HandleOnStart(OnCreateUploads);*/
            
 
            return base.StartAsync(cancellationToken);
        }

        private void HandleOnStart(IEnumerable<IUploadTypeConfiguration> list) 
        {
            foreach (var item in list.ToList())
            {
                foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask, SearchOption.AllDirectories))
                {
                    
                    var dto = new UploadFileBackupDTO
                    {
                        archiveFolder = item.ArchiveFolder,
                        cleanUpDays = item.CleanUpPeriodDays,
                        fileMask = item.FileMask,
                        localFilePath = filePath,
                        remoteFolder = item.RemoteFolder
                    };
                        
                        _upload.UploadFile(dto.localFilePath, dto.remoteFolder);
                        _clean.CleanOutdatedFilesOnDays(dto.archiveFolder,dto.fileMask, dto.cleanUpDays);
                        _archive.SaveFileToArchiveFolder(dto.localFilePath,  Path.Combine(dto.archiveFolder, Path.GetFileName(dto.localFilePath)));
                    
                }
                
            }    
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
    
             //_PeriodicalStrategy.Upload();
             //_TimeStrategy.Upload();
             _OnChangeStrategy.Upload();
            // _OnCreateStrategy.Upload();
             

        }
    }
}