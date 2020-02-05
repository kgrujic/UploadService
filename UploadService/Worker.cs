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
using Timer = System.Timers.Timer;

namespace UploadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private IEnumerable<PeriodicalUpload> _periodicalUploads;
        private IEnumerable<TimeSpecificUpload> _timeSpecificUploads;
        private IEnumerable<UploadOnChange> _onChangeUploads;
        private IEnumerable<UploadOnCreate> _onCreateUploads;


        private IServerConfiguration _ftpServerConfiguration;
   
        
        private IUploadStrategy<PeriodicalUpload> _periodicalStrategy;
        private IUploadStrategy<TimeSpecificUpload> _timeStrategy;
        private IUploadStrategy<UploadOnChange> _onChangeStrategy;
        private IUploadStrategy<UploadOnCreate> _onCreateStrategy;


        public Worker(ILogger<Worker> logger, IOptions<AppSettings> settings, IIoHelper ioHelper,IUploadServiceRepository repository, IHashHelper hashHelper,IUpload upload, IArchive archive, IClineable clean,
            IUploadStrategy<PeriodicalUpload> periodicalStrategy, IUploadStrategy<TimeSpecificUpload> timeStrategy, IUploadStrategy<UploadOnChange> onChangeStrategy, IUploadStrategy<UploadOnCreate> onCreateStrategy)
        {
            _periodicalUploads = settings.Value.PeriodicalUploads;
            _timeSpecificUploads = settings.Value.TimeSpecificUploads;
            _onChangeUploads = settings.Value.OnChangeUploads;
            _onCreateUploads = settings.Value.OnCreateUploads;
            
            _periodicalStrategy = periodicalStrategy;
            _timeStrategy = timeStrategy;
            _onChangeStrategy = onChangeStrategy;
            _onCreateStrategy = onCreateStrategy;
            
            _logger = logger;
        }
        
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
         // _periodicalStrategy.Upload(_periodicalUploads);
          // _timeStrategy.Upload(_timeSpecificUploads);
          // _onChangeStrategy.Upload(_onChangeUploads);
          // _onCreateStrategy.Upload(_onCreateUploads);
        }
    }
}