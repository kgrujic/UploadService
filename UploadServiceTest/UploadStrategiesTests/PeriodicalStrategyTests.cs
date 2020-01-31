using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using UploadService;
using UploadService.Configurations.UploadStrategies.Implementations;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities.UploadFiles;

namespace UploadServiceTest.UploadStrategiesTests
{
    public class PeriodicalStrategyTests
    {
        [Test]
        public void Upload_Success()
        {
            /*using (var mock = AutoMock.GetLoose())
            {
                IEnumerable<IUploadTypeConfiguration> dummyPeriodicalUploads = new List<PeriodicalUpload>();
                dummyPeriodicalUploads.ToList().Add(new PeriodicalUpload
                {
                    LocalFolderPath = "/path/dummy",
                    RemoteFolder = "/dummyRemote",
                    FileMask = ".tct",
                    Interval = 10000,
                    ArchiveFolder = "/archddumy",
                    CleanUpPeriodDays = 5
                });

                mock.Mock<Timer>()
                    .SetupAdd(t => t.Elapsed += (sender, args) => { });
                
                mock.Mock<Timer>()
                    .VerifyAdd(t => t.Elapsed += It.IsAny<ElapsedEventHandler>(), Times.Exactly(dummyPeriodicalUploads.Count()));

            }*/
        }

        [Test]
        public void OnTimedEvent_Success()
        {
            using (var mock = AutoMock.GetLoose())
            {
                PeriodicalUpload dummyItem = new PeriodicalUpload
                {
                    ArchiveFolder = "/dummyarch",
                    CleanUpPeriodDays = 5,
                    FileMask = ".txt",
                    Interval = 50000,
                    LocalFolderPath = "/dummypath",
                    RemoteFolder = "/dummyRemote"
                };
                
                IEnumerable<IUploadTypeConfiguration> dummyPeriodicalUploads = new List<PeriodicalUpload>();
                dummyPeriodicalUploads.ToList().Add(new PeriodicalUpload
                {
                    LocalFolderPath = "/path/dummy",
                    RemoteFolder = "/dummyRemote",
                    FileMask = ".tct",
                    Interval = 10000,
                    ArchiveFolder = "/archddumy",
                    CleanUpPeriodDays = 5
                });
                
                

                

            }
            
            
        }
    }
}