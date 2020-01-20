using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class TimeSpecificStrategy : IUploadStrategy
    {
        private static System.Timers.Timer aTimer;

        private IEnumerable<TimeSpecificUpload> _foldersToUpload;
        private IServerClient _client;
        private DateTime scheduledTime;

        public TimeSpecificStrategy(List<IUploadTypeConfiguration> foldersToUpload, IServerClient client)
        {
            _foldersToUpload = foldersToUpload.Cast<TimeSpecificUpload>();
            _client = client;
            scheduledTime = DateTime.Today.AddDays(0).AddHours(10).AddMinutes(5);
        }

        public void Upload()
        {
          
            aTimer = new Timer();
            aTimer.Enabled = true;
            
            if (DateTime.Now > scheduledTime)
                scheduledTime = scheduledTime.AddDays(1);
            
            aTimer.Interval = scheduledTime.Subtract(DateTime.Now).TotalSeconds * 1000;
           
            //aTimer.Start();
            aTimer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            double tillNextInterval = scheduledTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            if (tillNextInterval < 0) tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
            aTimer.Interval = tillNextInterval;
            
            Console.WriteLine("I am here");
            foreach (var folder in _foldersToUpload.Select(f => f.LocalFolderPath))
            {
                var remoteFolder = _foldersToUpload.Where(rf => rf.LocalFolderPath == folder).Select(rm => rm.RemoteFolder).FirstOrDefault();
                var fileMask = _foldersToUpload.Where(rf => rf.LocalFolderPath == folder).Select(r => r.FileMask).FirstOrDefault();
                // TODO Add other parameters
                Console.WriteLine(folder);
                foreach (string filePath in Directory.EnumerateFiles(folder,"*"+fileMask,SearchOption.AllDirectories))
                {
                    Console.WriteLine(filePath);
                    string[] arraytmp  = filePath.Split('/');
                    var fileName = arraytmp[arraytmp.Length - 1];
                  
                    
                    //TODO Ask async
                    if (_client.checkIfFileExists($"{"home/katarina/" + remoteFolder + "/"}{fileName}"))
                    {
                        _client.delete($"{"home/katarina/" + remoteFolder + "/"}{fileName}");
                        _client.UploadFile($"{"home/katarina/" + remoteFolder + "/"}{fileName}",
                            $"{folder + "/"}{fileName}");
                    }
                    else 
                    {
                        _client.UploadFile($"{"home/katarina/" + remoteFolder + "/"}{fileName}", $"{folder + "/"}{fileName}");
                    }
                        
                }
                
            }

            
        }
    }
}