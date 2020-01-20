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

namespace UploadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public IEnumerable<IUploadTypeConfiguration> PeriodicalUpload;
        public IServerConfiguration ftpServerConfiguration;
      
        
       // private static List<IUploadTypeConfiguration> timeSpec = new List<IUploadTypeConfiguration>();
        
        private IUploadStrategy _PeriodicalStrategy;
       // private IUploadStrategy _TimeStrategy;


        public Worker(ILogger<Worker> logger,IOptions<AppSettings> settings)
        {
            
            //timeSpec.Add(new TimeSpecificUpload {LocalFolderPath = "/home/katarina/Desktop/testfolder2", RemoteFolder = "ftptestfolder", FileMask = ".txt", Time = new TimeSpan(10,15,0)});

           PeriodicalUpload = settings.Value.PeriodicalUpload;
           ftpServerConfiguration = settings.Value.ftpServerConfiguration;
           IServerClient client = new FTPClient(ftpServerConfiguration.HostAddress,ftpServerConfiguration.Username,ftpServerConfiguration.Password);
           

           
             _PeriodicalStrategy = new PeriodicalStrategy(PeriodicalUpload,client);
            // _TimeStrategy = new TimeSpecificStrategy(timeSpec,client);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*foreach (var VARIABLE in PeriodicalUpload.Cast<PeriodicalUpload>())
            {
                Console.WriteLine(VARIABLE.Interval);
                
            }*/
             _PeriodicalStrategy.Upload();
            // _TimeStrategy.Upload();
            
            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }*/
        }
    }
}