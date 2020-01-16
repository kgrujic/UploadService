using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Timers;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class PeriodicalStrategy : IUploadStrategy
    {
        public int _Interval { get; set; }
        private static System.Timers.Timer aTimer;

        private List<IUploadTypeConfiguration> _foldersToUpload;
        private IServerClient _client;

        public PeriodicalStrategy(List<IUploadTypeConfiguration> foldersToUpload, IServerClient client, int interval)
        {
            _foldersToUpload = foldersToUpload;
            _client = client;
            _Interval = interval;
            
        }


        public void Upload()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(_Interval);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            foreach (var folder in _foldersToUpload.Select(f => f.LocalFolderPath))
            {
                foreach (string fileName in Directory.GetFiles(folder))
                {
                    var remoteFolder = _foldersToUpload.Select(rf => rf.RemoteFolder);
                    _client.UploadFile($"{remoteFolder}{fileName}", folder);
                }
                
             
            }
        }
    }
}