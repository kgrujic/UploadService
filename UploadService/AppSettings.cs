using System.Collections.Generic;
using UploadService.Configurations.ServerConfigurations.Implementations;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;

namespace UploadService
{
    public class AppSettings
    {
        public List<PeriodicalUpload> PeriodicalUploads { get; set; }
        public List<TimeSpecificUpload> TimeSpecificUploads { get; set; }
        
        public List<UploadOnChange> OnChangeUploads { get; set; }
        
        public List<UploadOnCreate> OnCreateUploads { get; set; }
        public FtpServerConfiguration FtpServerConfiguration { get; set; }
    }
}