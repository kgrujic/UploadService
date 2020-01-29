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
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
using UploadServiceDatabase.Repositories;

namespace UploadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private IEnumerable<IUploadTypeConfiguration> PeriodicalUploads;
        private IEnumerable<IUploadTypeConfiguration> TimeSpecificUploads;
        private IEnumerable<IUploadTypeConfiguration> OnChangeUploads;
        private IEnumerable<IUploadTypeConfiguration> OnCreateUploads;


        private IServerConfiguration _ftpServerConfiguration;
        private IServerClient _client;
        private IIOHelper _ioHelper;
        private IHashHelper _hashHelper;
        private IUploadServiceRepository _repository;

        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;


        private IUploadStrategy _PeriodicalStrategy;
        private IUploadStrategy _TimeStrategy;
        private IUploadStrategy _OnChangeStrategy;
        private IUploadStrategy _OnCreateStrategy;


        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings)
        {
            PeriodicalUploads = settings.Value.PeriodicalUploads;
            TimeSpecificUploads = settings.Value.TimeSpecificUploads;
            OnChangeUploads = settings.Value.OnChangeUploads;
            OnCreateUploads = settings.Value.OnCreateUploads;

            _ftpServerConfiguration = settings.Value.ftpServerConfiguration;
            _client = new FTPClient(_ftpServerConfiguration.HostAddress, _ftpServerConfiguration.Username,
                _ftpServerConfiguration.Password);

            _ioHelper = new IOHelper();
            _repository = new UploadServiceRepository();
            _hashHelper = new HashHelper();

            _upload = new UploadFiles(_client, _repository, _hashHelper);
            _archive = new ArchiveFiles(_ioHelper);
            _clean = new CleanOudatedFiles();

            
            _PeriodicalStrategy = new PeriodicalStrategy(PeriodicalUploads, _upload, _archive, _clean);
            _TimeStrategy = new TimeSpecificStrategy(TimeSpecificUploads, _upload, _archive, _clean);
            _OnChangeStrategy = new OnChangeStrategy(OnChangeUploads, _upload);
            _OnCreateStrategy = new OnCreateStrategy(OnCreateUploads, _upload, _archive, _clean);
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker started at: {DateTime.Now}");

           
            /*HandleOnStartOnChange(OnChangeUploads); 
            HandleOnStart(PeriodicalUploads);
            HandleOnStart(TimeSpecificUploads);
            HandleOnStart(OnCreateUploads);*/

            return base.StartAsync(cancellationToken);
        }

        private void HandleOnStart(IEnumerable<IUploadTypeConfiguration> list)
        {
            foreach (var item in list.ToList())
            {
                foreach (string filePath in Directory.EnumerateFiles(item.LocalFolderPath, item.FileMask,
                    SearchOption.AllDirectories))
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
                    _clean.CleanOutdatedFilesOnDays(dto.archiveFolder, dto.fileMask, dto.cleanUpDays);
                    _archive.SaveFileToArchiveFolder(dto.localFilePath,
                        Path.Combine(dto.archiveFolder, Path.GetFileName(dto.localFilePath)));
                }
            }
        }

        private void HandleOnStartOnChange(IEnumerable<IUploadTypeConfiguration> list)
        {
            foreach (var file in list.ToList().Cast<UploadOnChange>())
            {
                _upload.UploadFile(file.LocalFilePath, file.RemoteFolder);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          //  _PeriodicalStrategy.Upload();
           // _TimeStrategy.Upload();
           // _OnChangeStrategy.Upload();
           // _OnCreateStrategy.Upload();
        }
    }
}