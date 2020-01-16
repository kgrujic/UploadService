using System;
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

        private IEnumerable<PeriodicalUpload> _foldersToUpload;
        private IServerClient _client;

        public PeriodicalStrategy(List<IUploadTypeConfiguration> foldersToUpload, IServerClient client, int interval)
        {
            _foldersToUpload = foldersToUpload.Cast<PeriodicalUpload>();
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
                var remoteFolder = _foldersToUpload.Where(rf => rf.LocalFolderPath == folder).Select(rm => rm.RemoteFolder).FirstOrDefault();
                var fileMask = _foldersToUpload.Where(rf => rf.LocalFolderPath == folder).Select(r => r.FileMask).FirstOrDefault();
               // TODO Add other parameters
                foreach (string filePath in Directory.GetFiles(folder))
                {
                    string[] arraytmp  = filePath.Split('/');
                    var fileName = arraytmp[arraytmp.Length - 1];
                    if (fileName.Contains(fileMask))
                    {
                        //TODO Ask async
                        _client.delete($"{"home/katarina/" + remoteFolder + "/"}{fileName}");
                        _client.UploadFile($"{"home/katarina/" + remoteFolder + "/"}{fileName}", $"{folder + "/"}{fileName}");
                    }
             
                   
                }


            }
        }
    }
}
