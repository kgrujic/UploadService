using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
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
                
                DirectoryInfo startDir = new DirectoryInfo(folder);
                TraverseDirectory(startDir,fileMask, remoteFolder);
              

            }
        }
        
        public void TraverseDirectory(DirectoryInfo directoryInfo, string fileMask, string remoteFolder)
        {
            //var subdirectories = directoryInfo.EnumerateDirectories();

            /*foreach (var subdirectory in subdirectories)
            {
                TraverseDirectory(subdirectory, fileMask,remoteFolder);
            }*/

            var files = directoryInfo.EnumerateFiles();
          
            foreach (var file in files)
            {
                HandleFile(file,fileMask, remoteFolder, directoryInfo.ToString());
            }
        }

        void HandleFile(FileInfo file,string fileMask, string remoteFolder, string directory)
        {
            
           
            string[] arraytmp  = file.ToString().Split('/');
            var fileName = arraytmp[arraytmp.Length - 1];
            //TODO check before
            if (fileName.Contains(fileMask))
            {
                
                if (_client.checkIfFileExists($"{"home/katarina/" + remoteFolder + "/"}{fileName}"))
                {
                    _client.delete($"{"home/katarina/" + remoteFolder + "/"}{fileName}");
                    _client.UploadFile($"{"home/katarina/" + remoteFolder + "/"}{fileName}",
                        $"{directory.ToString() + "/"}{fileName}");
                }
                else 
                {
                    _client.UploadFile($"{"home/katarina/" + remoteFolder + "/"}{fileName}", $"{directory.ToString() + "/"}{fileName}");
                }

                
            }
        }
    }
}


