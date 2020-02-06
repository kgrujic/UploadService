using System;
using System.IO;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using UploadService;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.IO_Helpers;

namespace UploadServiceTest.UtilitiesTests.ArchiveFilesTests
{
    public class ArchiveFilesTests
    {
        private IArchive _archive;
        private Mock<IIoHelper> _ioHelperMock;
        private Mock<ILogger<Worker>> _loggerMock;

        [SetUp]
        public void Setup()
        {
           
            var mocks = new MockRepository(MockBehavior.Default);
            _ioHelperMock = mocks.Create<IIoHelper>();
            _loggerMock = mocks.Create<ILogger<Worker>>();
            _archive = new ArchiveFiles(_ioHelperMock.Object, _loggerMock.Object);
        }

        [Test]
        public void SaveFileToArchiveFolder_ValidFilePaths_Successful()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var dummySourcePath = "/path/source/my.txt";
                var dummyDestinationPath = "/path/dest/my.txt";

                mock.Mock<IIoHelper>()
                    .Setup(x => x.CopyFile(dummySourcePath, dummyDestinationPath));
                
                var cls = mock.Create<ArchiveFiles>();
                
                cls.MoveFileToArchiveFolder(dummySourcePath,dummyDestinationPath);
                
                mock.Mock<IIoHelper>()
                    .Verify(X => X.CopyFile(dummySourcePath,dummyDestinationPath), Times.Exactly(1));

            }
        }

        
        [Test]
        public void SaveFileToArchiveFolder_InvalidFilePaths_ThrowException()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var dummySourcePath = "/path/source/my.txt";
                string dummyDestinationPath = null;

                mock.Mock<IIoHelper>()
                    .Setup(x => x.CopyFile(dummySourcePath, dummyDestinationPath)).Throws<DirectoryNotFoundException>();
                
                
                var cls = mock.Create<ArchiveFiles>();
                cls.MoveFileToArchiveFolder(dummySourcePath,dummyDestinationPath);
              
               //Assert.Catch<DirectoryNotFoundException>(() =>  cls.SaveFileToArchiveFolder(dummySourcePath,dummyDestinationPath));
               //mock.Mock<IIOHelper>()
                   //.Verify(X => X.CopyFile(dummySourcePath,dummyDestinationPath));
               

            }
        }
    }
}