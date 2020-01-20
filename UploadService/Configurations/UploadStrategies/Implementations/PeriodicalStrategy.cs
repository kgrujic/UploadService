using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
       
        private static System.Timers.Timer aTimer;

        private IEnumerable<PeriodicalUpload> _foldersToUpload;
        private IServerClient _client;

        public PeriodicalStrategy(IEnumerable<IUploadTypeConfiguration> foldersToUpload, IServerClient client)
        {
            _foldersToUpload = foldersToUpload.Cast<PeriodicalUpload>();
            _client = client;
          

        }
        
        public void Upload()
        {
            
            List<Timer> timerMatrix = new List<Timer>();

            foreach (var VARIABLE in _foldersToUpload)
            {
                var timer = new System.Timers.Timer() {
                    Enabled = true,
                    Interval = VARIABLE.Interval,
                    AutoReset = true
                };
                 
                timerMatrix.Add(timer);

                timer.Elapsed += (sender, e) =>
                {

                    var path = VARIABLE.LocalFolderPath;
                    OnTimedEvent(path);
                    
                };
            }
            
        }
        
        
        private void OnTimedEvent(string localFolderPath)
        {
                var remoteFolder = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(rm => rm.RemoteFolder).FirstOrDefault();
                var fileMask = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(r => r.FileMask).FirstOrDefault();
                // TODO Add other parameters
                Console.WriteLine(localFolderPath);
                foreach (string filePath in Directory.EnumerateFiles(localFolderPath,"*"+fileMask,SearchOption.AllDirectories))
                {
                    Console.WriteLine(filePath);
                    string[] arraytmp  = filePath.Split('/');
                    var fileName = arraytmp[arraytmp.Length - 1];
                  
                    
                    //TODO Ask async
                    if (_client.checkIfFileExists($"{"home/katarina/" + remoteFolder + "/"}{fileName}"))
                    {
                        _client.delete($"{"home/katarina/" + remoteFolder + "/"}{fileName}");
                        _client.UploadFile($"{"home/katarina/" + remoteFolder + "/"}{fileName}",
                            $"{localFolderPath + "/"}{fileName}");
                    }
                    else 
                    {
                        _client.UploadFile($"{"home/katarina/" + remoteFolder + "/"}{fileName}", $"{localFolderPath + "/"}{fileName}");
                    }
                        
                }
                
        }
        
    }
}


