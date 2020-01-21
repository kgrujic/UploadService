using System.Collections.Generic;
using UploadService.Configurations.ServerConfiguration.Implementations;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;

namespace UploadService
{
    public class AppSettings
    {
        public List<PeriodicalUpload> PeriodicalUploads { get; set; }
        public List<TimeSpecificUpload> TimeSpecificUploads { get; set; }
        public FTPServerConfiguration ftpServerConfiguration { get; set; }
    }
}