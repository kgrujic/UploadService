using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Timers;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities;
using UploadService.Utilities.IO_Helpers;

namespace UploadService.Configurations.UploadStrategies.Implementations
{
    public class PeriodicalStrategy : IUploadStrategy
    {
        
        private IEnumerable<PeriodicalUpload> _foldersToUpload;
        private IServerClient _client;
        private IIOHelper _ioHelper;

        public PeriodicalStrategy(IEnumerable<IUploadTypeConfiguration> foldersToUpload, IServerClient client, IIOHelper ioHelper)
        {
            _foldersToUpload = foldersToUpload.Cast<PeriodicalUpload>();
            _client = client;
            _ioHelper = ioHelper;

        }
        
        public void Upload()
        {
            
            List<Timer> timerMatrix = new List<Timer>();

            foreach (var item in _foldersToUpload)
            {
                var timer = new System.Timers.Timer() {
                    Enabled = true,
                    Interval = item.Interval,
                    AutoReset = true
                };
                 
                timerMatrix.Add(timer);

                timer.Elapsed += (sender, e) =>
                {

                    var path = item.LocalFolderPath;
                    OnTimedEvent(path);
                    
                };
            }
            
        }
        
        
        private void OnTimedEvent(string localFolderPath)
        {
                var remoteFolder = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(rm => rm.RemoteFolder).FirstOrDefault();
                var fileMask = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(r => r.FileMask).FirstOrDefault();
                var archiveFolder = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(r => r.ArchiveFolder).FirstOrDefault();
                var cleanUpDays = _foldersToUpload.Where(rf => rf.LocalFolderPath == localFolderPath).Select(r => r.CleanUpPeriodDays).FirstOrDefault();
                _ioHelper.CreateDirectoryIfNotExist(archiveFolder);
                
                foreach (string filePath in Directory.EnumerateFiles(localFolderPath,"*"+ fileMask,SearchOption.AllDirectories))
                {
                   
                    string[] arraytmp  = filePath.Split('/');
                    var fileName = arraytmp[arraytmp.Length - 1];
                    
                    var remoteFilePath = $"{"home/katarina/" + remoteFolder + "/"}{fileName}";
                    var localFilePath = $"{localFolderPath + "/"}{fileName}";
                   
                    
                    //TODO Ask async
                    if (_client.checkIfFileExists(remoteFilePath))
                    {
                        _client.delete(remoteFilePath);
                        _client.UploadFile(remoteFilePath, localFilePath);
                        _ioHelper.CleanOutdatedFiles(archiveFolder, fileMask, cleanUpDays);
                        _ioHelper.SaveFileToArchiveFolder(localFilePath, $"{archiveFolder + "/"}{fileName}");
                    }
                    else 
                    {
                        _client.UploadFile(remoteFilePath, localFilePath);
                        _ioHelper.CleanOutdatedFiles(archiveFolder, fileMask, cleanUpDays);
                        _ioHelper.SaveFileToArchiveFolder(localFilePath, $"{archiveFolder + "/"}{fileName}");
                    }
                        
                }
                
        }

        

       

       
        
    }
}


