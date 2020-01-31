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

        private IEnumerable<IUploadTypeConfiguration> _periodicalUploads;
        private IEnumerable<IUploadTypeConfiguration> _timeSpecificUploads;
        private IEnumerable<IUploadTypeConfiguration> _onChangeUploads;
        private IEnumerable<IUploadTypeConfiguration> _onCreateUploads;


        private IServerConfiguration _ftpServerConfiguration;
        //private IServerClient _client;
        /*private IIOHelper _ioHelper;
        private IHashHelper _hashHelper;
        private IUploadServiceRepository _repository;

        private IUpload _upload;
        private IArchive _archive;
        private IClineable _clean;*/


        private IUploadStrategy _periodicalStrategy;
        private IUploadStrategy _timeStrategy;
        private IUploadStrategy _onChangeStrategy;
        private IUploadStrategy _onCreateStrategy;


        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings, IIOHelper ioHelper,IUploadServiceRepository repository, IHashHelper hashHelper,IUpload upload, IArchive archive, IClineable clean)
        {
            _periodicalUploads = settings.Value.PeriodicalUploads;
            _timeSpecificUploads = settings.Value.TimeSpecificUploads;
            _onChangeUploads = settings.Value.OnChangeUploads;
            _onCreateUploads = settings.Value.OnCreateUploads;


            _periodicalStrategy = new PeriodicalStrategy(upload, archive, clean);
            _timeStrategy = new TimeSpecificStrategy(upload, archive, clean);
            _onChangeStrategy = new OnChangeStrategy(upload);
            _onCreateStrategy = new OnCreateStrategy(upload, archive, clean);
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

        /*private void HandleOnStart(IEnumerable<IUploadTypeConfiguration> list)
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
        }*/

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           _periodicalStrategy.Upload(_periodicalUploads);
           // _TimeStrategy.Upload(TimeSpecificUploads);
           // _OnChangeStrategy.Upload(_onChangeUploads);
           // _OnCreateStrategy.Upload(OnCreateUploads);
        }
    }
}