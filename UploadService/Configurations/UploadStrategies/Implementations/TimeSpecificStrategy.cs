using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class TimeSpecificStrategy : IUploadStrategy
    {
        //private static System.Timers.Timer aTimer;

        private IEnumerable<TimeSpecificUpload> _foldersToUpload;
        private IServerClient _client;
        private IIOHelper _ioHelper;
        //private DateTime scheduledTime;

        public TimeSpecificStrategy(IEnumerable<IUploadTypeConfiguration> foldersToUpload, IServerClient client, IIOHelper ioHelper)
        {
            _foldersToUpload = foldersToUpload.Cast<TimeSpecificUpload>();
            _client = client;
            _ioHelper = ioHelper;
        }

        public void Upload()
        {
          
            List<Timer> timerMatrix = new List<Timer>();

            foreach (var item in _foldersToUpload)
            { 
                DateTime dt = item.Time.ToUniversalTime();
                
               var scheduledTime = DateTime.Today.AddDays(0).AddHours(dt.Hour).AddMinutes(dt.Minute);
               
                
                var timer = new Timer();
                timer.Enabled = true;
                
                if (DateTime.Now > scheduledTime)
                    scheduledTime = scheduledTime.AddDays(1);
                
                timer.Interval = scheduledTime.Subtract(DateTime.Now).TotalSeconds * 1000;
                timer.AutoReset = true;
                 
                timerMatrix.Add(timer);

                timer.Elapsed += (sender, e) =>
                {

                    var path = item.LocalFolderPath;
                    OnTimedEvent(path, scheduledTime, timer);
                    
                };
            }
        }

        private void OnTimedEvent(string localFolderPath, DateTime scheduledTime, Timer timer)
        {
            double tillNextInterval = scheduledTime.Subtract(DateTime.Now).TotalSeconds * 1000;
            if (tillNextInterval < 0) tillNextInterval += new TimeSpan(24, 0, 0).TotalSeconds * 1000;
                timer.Interval = tillNextInterval;
            
            
                var remoteFolder = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(rm => rm.RemoteFolder).FirstOrDefault();
                var fileMask = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(r => r.FileMask).FirstOrDefault();
                var archiveFolder = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(r => r.ArchiveFolder).FirstOrDefault();
                var cleanUpDays = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(r => r.CleanUpPeriodDays).FirstOrDefault();
                _ioHelper.CreateDirectoryIfNotExist(archiveFolder);
                
                Console.WriteLine(localFolderPath);
                foreach (string filePath in Directory.EnumerateFiles(localFolderPath,"*"+fileMask,SearchOption.AllDirectories))
                {
                    Console.WriteLine(filePath);
                    string[] arraytmp  = filePath.Split('/');
                    var fileName = arraytmp[arraytmp.Length - 1];
                  
                    var remoteFilePath = $"{"home/katarina/" + remoteFolder + "/"}{fileName}";
                    var localFilePath = $"{localFolderPath + "/"}{fileName}";
                    //TODO Ask async
                    if (_client.checkIfFileExists($"{"home/katarina/" + remoteFolder + "/"}{fileName}"))
                    {
                        _client.delete(remoteFilePath);
                        _client.UploadFile(remoteFilePath,localFilePath);
                        _ioHelper.CleanOutdatedFiles(archiveFolder, fileMask, cleanUpDays);
                        _ioHelper.SaveFileToArchiveFolder(localFilePath, $"{archiveFolder + "/"}{fileName}");
                    }
                    else 
                    {
                        _client.UploadFile(remoteFilePath,localFilePath);
                        _ioHelper.CleanOutdatedFiles(archiveFolder, fileMask, cleanUpDays);
                        _ioHelper.SaveFileToArchiveFolder(localFilePath, $"{archiveFolder + "/"}{fileName}");
                    }
                        
                }
                
            

            
        }
    }
}