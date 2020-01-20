using System.Collections.Generic;
using UploadService.Configurations.ServerConfiguration.Implementations;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;

namespace UploadService
{
    public class AppSettings
    {
        public List<PeriodicalUpload> PeriodicalUpload { get; set; }
        public FTPServerConfiguration ftpServerConfiguration { get; set; }
    }
}