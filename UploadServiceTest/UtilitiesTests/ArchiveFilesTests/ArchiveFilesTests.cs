using System;
using System.IO;
using Moq;
using NUnit.Framework;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.IO_Helpers;

namespace UploadServiceTest.UtilitiesTests.ArchiveFilesTests
{
    public class ArchiveFilesTests
    {
        private IArchive _archive;
        private Mock<IIOHelper> ioHelperMock;

        [SetUp]
        public void Setup()
        {
           
            var mocks = new MockRepository(MockBehavior.Default);
            ioHelperMock = mocks.Create<IIOHelper>();
            _archive = new ArchiveFiles(ioHelperMock.Object);
        }

        [Test]
        public void SaveFileToArchiveFolder_ValidFilePaths_Successful()
        {
            var dummySourcePath = "/path/source/my.txt";
            var dummyDestinationPath = "/path/dest/my.txt";
           _archive.SaveFileToArchiveFolder(dummySourcePath, dummyDestinationPath);
            ioHelperMock.Verify(x => x.CopyFile(dummySourcePath,dummyDestinationPath));
        }

        
        [Test]
        public void SaveFileToArchiveFolder_InvalidFilePaths_ThrowException()
        {
            var dummySourcePath = "/path/source/my.txt";
            var dummyDestinationPath = "/path/dest/my.txt";
            /*ioHelperMock.Setup(x => x.CopyFile(dummySourcePath, dummyDestinationPath))
                .Throws<DirectoryNotFoundException>();*/
            
            Assert.Throws<DirectoryNotFoundException>(() =>_archive.SaveFileToArchiveFolder(dummySourcePath, dummyDestinationPath));
            
            /*ioHelperMock.Verify(x => x.CopyFile(dummySourcePath,dummyDestinationPath));*/
        }
    }
}