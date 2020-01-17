using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        private static List<IUploadTypeConfiguration> periodical = new List<IUploadTypeConfiguration>();
        //IServerConfiguration ftpServerConfiguration = new FTPServerConfiguration("ftp://10.251.65.37/","katarina","bajici");
        //private static IServerConfiguration ftpServerConfiguration = new FTPServerConfiguration("ftp://10.251.65.37/","katarina","bajici");
        //static IServerClient client = new FTPClient(ftpServerConfiguration.HostAddress,ftpServerConfiguration.Username,ftpServerConfiguration.Password);
        private IUploadStrategy _PeriodicalStrategy;


        public Worker(ILogger<Worker> logger)
        {
            //IServerConfiguration ftpServerConfiguration = new FTPServerConfiguration("ftp://10.251.65.37/","katarina","bajici");
            //Console.WriteLine(ftpServerConfiguration.HostAddress);
            IServerClient client = new FTPClient("ftp://192.168.8.103/","katarina","bajici");
            periodical.Add(new PeriodicalUpload {LocalFolderPath = "/home/katarina/Desktop/testfolder3", RemoteFolder = "ftptestfolder", FileMask = ".txt"}); 
            periodical.Add(new PeriodicalUpload {LocalFolderPath = "/home/katarina/Desktop/testfolder2", RemoteFolder = "ftptestfolder", FileMask = ".txt"});

             _PeriodicalStrategy = new PeriodicalStrategy(periodical,client,4000);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
             _PeriodicalStrategy.Upload();
            
            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }*/
        }
    }
}