using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UploadService.Configurations.UploadStrategies;
using UploadService.Configurations.UploadStrategies.Implementations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.CleaningOutdatedFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
using UploadServiceDatabase.Context;
using UploadServiceDatabase.Context.ContextFactory;
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

                    services.AddSingleton<IServerClient, FtpClient>(u =>
                    {
                        var host = hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection("HostAddress").Value;
                        var username = hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection("Username").Value;
                        var pass = hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection("Password").Value;
                        var port = Convert.ToInt32(hostContext.Configuration.GetSection("AppSettings")
                            .GetSection("ftpServerConfiguration").GetSection("PortNumber").Value);
                        var worker = u.GetRequiredService<ILogger<Worker>>();
                        return new FtpClient(host, username, pass, port, worker);
                    });


                    var connString = hostContext.Configuration.GetSection("AppSettings")
                        .GetSection("DefaultConnection").Value;

                    services.AddDbContext<UploadServiceContext>(options => options.UseSqlite(connString),
                        ServiceLifetime.Singleton);

                    services.AddSingleton<IContextFactory, ContextFactory>();


                    services.AddSingleton<IIoHelper, IoHelper>();
                    services.AddSingleton<IHashHelper, HashHelper>();
                    services.AddSingleton<IUploadServiceRepository, UploadServiceRepository>();

                    services.AddSingleton<IUpload, UploadFiles>();
                    services.AddSingleton<IArchive, ArchiveFiles>();
                    services.AddSingleton<IClineable, CleanOutdatedFiles>();

                    services.AddSingleton<IUploadStrategy<PeriodicalUpload>, PeriodicalStrategy>();
                    services.AddSingleton<IUploadStrategy<TimeSpecificUpload>, TimeSpecificStrategy>();
                    services.AddSingleton<IUploadStrategy<UploadOnCreate>, OnCreateStrategy>();
                    services.AddSingleton<IUploadStrategy<UploadOnChange>, OnChangeStrategy>();
                }).UseSystemd();
    }
}