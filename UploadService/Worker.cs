using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UploadService.Configurations.ServerConfigurations;
using UploadService.Configurations.UploadStrategies;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
using UploadServiceDatabase.Repositories;

namespace UploadService
{
    /// <summary>
    /// Worker class contains methods for starting and execution of service
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private IEnumerable<PeriodicalUpload> _periodicalUploads;
        private IEnumerable<TimeSpecificUpload> _timeSpecificUploads;
        private IEnumerable<UploadOnChange> _onChangeUploads;
        private IEnumerable<UploadOnCreate> _onCreateUploads;
        
        
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
        /// <summary>
        /// StartAsync method is called when service is started
        /// StartUpUpload methods from strategies are called
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Task</returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker started at: {DateTime.Now}");
            
            _periodicalStrategy.StartUpUpload(_periodicalUploads);
            _timeStrategy.StartUpUpload(_timeSpecificUploads);
            _onCreateStrategy.StartUpUpload(_onCreateUploads);
            _onChangeStrategy.StartUpUpload(_onChangeUploads);

            return base.StartAsync(cancellationToken);
        }
        
        /// <summary>
        /// ExecuteAsync method is called when service is working
        /// Upload methods from strategies are called
        /// </summary>
        /// <returns>Task</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
                _periodicalStrategy.Upload(_periodicalUploads);
                _timeStrategy.Upload(_timeSpecificUploads);
                 _onChangeStrategy.Upload(_onChangeUploads);
                _onCreateStrategy.Upload(_onCreateUploads);
            
       
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker stoped at: {DateTime.Now}");
            return base.StopAsync(cancellationToken);
        }
    }
}