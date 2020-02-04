using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UploadService.Configurations.UploadStrategies;
using UploadService.Configurations.UploadStrategies.Implementations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
using UploadServiceDatabase.Repositories;

namespace UploadService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
                    // TODO refactor
                    services.AddSingleton<IIoHelper, IoHelper>();
                    services.AddSingleton<IHashHelper, HashHelper>();
                    services.AddSingleton<IUploadServiceRepository, UploadServiceRepository>();
                    
                    services.AddSingleton<IServerClient, FtpClient>(u =>
                    {
                        var host = hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection("HostAddress").Value;
                        var username = hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection( "Username" ).Value;
                        var pass = hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection("Password").Value;   
                        var port = Convert.ToInt32(hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection("PortNumber").Value);
                        
                        return new FtpClient(host,username,pass,port);
                        
                    });

                    services.AddSingleton<IUpload>(u => new UploadFiles(u.GetRequiredService<IServerClient>(),
                        u.GetRequiredService<IUploadServiceRepository>(),
                        u.GetRequiredService<IHashHelper>()
                    ));
                    services.AddSingleton<IArchive>(u => new ArchiveFiles(u.GetRequiredService<IIoHelper>()
                    ));

                    services.AddSingleton<IUploadStrategy<PeriodicalUpload>, PeriodicalStrategy>();   
                    
                    services.AddSingleton<IUploadStrategy<TimeSpecificUpload>, TimeSpecificStrategy>();  
                    
                    services.AddSingleton<IUploadStrategy<UploadOnCreate>, OnCreateStrategy>();
                    
                    services.AddSingleton<IUploadStrategy<UploadOnChange>,OnChangeStrategy>();
                    
                    services.AddSingleton<IClineable, CleanOudatedFiles>();
                
                    
                }).UseSystemd();
    }
}