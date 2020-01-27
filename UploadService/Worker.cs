using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UploadService.Configurations.ServerConfiguration;
using UploadService.Configurations.ServerConfiguration.Implementations;
using UploadService.Configurations.UploadStrategies;
using UploadService.Configurations.UploadStrategies.Implementations;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Context;
using UploadService.Utilities;
using UploadService.Utilities.Clients;
using UploadService.Utilities.IO_Helpers;

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
       
           
           //TODO add context to other strategies
           _PeriodicalStrategy = new PeriodicalStrategy(PeriodicalUploads, client, IoHelper);
           _TimeStrategy = new TimeSpecificStrategy(TimeSpecificUploads, client, IoHelper);
           _OnChangeStrategy = new OnChangeStrategy(client,IoHelper,OnChangeUploads);
           
            _logger = logger;
        }
        
        /*public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Worker started at: {DateTime.Now}");
 
            return base.StartAsync(cancellationToken);
        }*/

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
    
             //_PeriodicalStrategy.Upload();
             //_TimeStrategy.Upload();
             _OnChangeStrategy.Upload();
             
        }
    }
}