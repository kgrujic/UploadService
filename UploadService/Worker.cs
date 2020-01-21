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
using UploadService.Utilities;
using UploadService.Utilities.IO_Helpers;

namespace UploadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public IEnumerable<IUploadTypeConfiguration> PeriodicalUploads;
        public IEnumerable<IUploadTypeConfiguration> TimeSpecificUploads;
        public IServerConfiguration ftpServerConfiguration;
        public IServerClient client;
        public IIOHelper IoHelper;
      
        
       // private static List<IUploadTypeConfiguration> timeSpec = new List<IUploadTypeConfiguration>();
        
        private IUploadStrategy _PeriodicalStrategy;
        private IUploadStrategy _TimeStrategy;


        public Worker(ILogger<Worker> logger,IOptions<AppSettings> settings)
        {
            
            //timeSpec.Add(new TimeSpecificUpload {LocalFolderPath = "/home/katarina/Desktop/testfolder2", RemoteFolder = "ftptestfolder", FileMask = ".txt"});

           PeriodicalUploads = settings.Value.PeriodicalUploads;
           TimeSpecificUploads = settings.Value.TimeSpecificUploads;
           
           ftpServerConfiguration = settings.Value.ftpServerConfiguration;
           client = new FTPClient(ftpServerConfiguration.HostAddress,ftpServerConfiguration.Username,ftpServerConfiguration.Password);
           
           IoHelper = new IOHelper();
           
           _PeriodicalStrategy = new PeriodicalStrategy(PeriodicalUploads,client,IoHelper);
           _TimeStrategy = new TimeSpecificStrategy(TimeSpecificUploads,client, IoHelper);
             
            // _TimeStrategy = new TimeSpecificStrategy(timeSpec,client);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*foreach (var VARIABLE in PeriodicalUpload.Cast<PeriodicalUpload>())
            {
                Console.WriteLine(VARIABLE.Interval);
                
            }*/
             //_PeriodicalStrategy.Upload();
             _TimeStrategy.Upload();
            // _TimeStrategy.Upload();
            
            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }*/
        }
    }
}