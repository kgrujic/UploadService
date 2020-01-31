using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UploadService.Utilities.ArchiveFiles;
using UploadService.Utilities.Clients;
using UploadService.Utilities.HashHelpers;
using UploadService.Utilities.IO_Helpers;
using UploadService.Utilities.UploadFiles;
using UploadServiceDatabase.DTOs;
using UploadServiceDatabase.Repositories;

namespace UploadServiceTest.UtilitiesTests.UploadFilesTests
{
    public class UploadFilesTest
    {
        [Test]
        public void UploadFiles_FileExistsOnRemoteOverwriteTrueHashDoNotMatch_UploadOnRemoterUpdateRepo()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var dummySourcePath = "/path/source/my.txt";
                var dummyRemoteFolder = "/path/dest/";
                var dummyRemoteFilePath = "/path/dest/my.txt";
                byte[] hashDLocal = new byte[5] {3, 10, 15, 12, 14};
                byte[] hashDDb = new byte[5] {3, 10, 15, 12, 14};
                var overwrite = true;
           
                var dummyDto = new FileDTO()
                {
                    FilePath = dummySourcePath, HashedContent = hashDLocal
                };
             

                mock.Mock<IHashHelper>()
                    .Setup(h => h.GenerateHash(dummySourcePath))
                    .Returns(hashDLocal);

                mock.Mock<IServerClient>()
                    .Setup(c => c.checkIfFileExists(dummyRemoteFilePath))
                    .Returns(true);

                mock.Mock<IUploadServiceRepository>()
                    .Setup(r => r.GetFileByPath(dummySourcePath).HashedContent)
                    .Returns(hashDDb);

                mock.Mock<IHashHelper>()
                    .Setup(h => h.HashMatching(hashDLocal, hashDDb))
                    .Returns(false);

                mock.Mock<IServerClient>()
                    .Setup(c => c.UploadFile(dummyRemoteFilePath, dummySourcePath, overwrite));

                mock.Mock<IUploadServiceRepository>()
                    .Setup(r => r.UpdateFile(dummyDto));


                var cls = mock.Create<UploadFiles>();
                cls.UploadFile(dummySourcePath, dummyRemoteFolder);
                

                mock.Mock<IServerClient>()
                    .Verify(X => X.checkIfFileExists(dummyRemoteFilePath), Times.Exactly(1));

                mock.Mock<IHashHelper>()
                    .Verify(X => X.GenerateHash(dummySourcePath), Times.Exactly(1));

                mock.Mock<IUploadServiceRepository>()
                    .Verify(X => X.GetFileByPath(dummySourcePath).HashedContent, Times.Exactly(1));

                mock.Mock<IHashHelper>()
                    .Verify(X => X.HashMatching(hashDLocal, hashDDb), Times.Exactly(1));

                mock.Mock<IUploadServiceRepository>()
                    .Verify(r => r.UpdateFile(It.Is<FileDTO>(x => x.FilePath == dummySourcePath && x.HashedContent == hashDLocal)));

                mock.Mock<IServerClient>()
                    .Verify(X => X.UploadFile(dummyRemoteFilePath, dummySourcePath, overwrite), Times.Exactly(1));
            }
        } 
        [Test]
        public void UploadFiles_FileExistsOnRemoteOverwriteTrueHashMatch_DoNotUploadOnRemoterDoNotUpdateRepo()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var dummySourcePath = "/path/source/my.txt";
                var dummyRemoteFolder = "/path/dest/";
                var dummyRemoteFilePath = "/path/dest/my.txt";
                byte[] hashDLocal = new byte[5] {3, 10, 15, 12, 14};
                byte[] hashDDb = new byte[5] {3, 10, 15, 12, 14};
                var overwrite = true;
           
                var dummyDto = new FileDTO()
                {
                    FilePath = dummySourcePath, HashedContent = hashDLocal
                };
             

                mock.Mock<IHashHelper>()
                    .Setup(h => h.GenerateHash(dummySourcePath))
                    .Returns(hashDLocal);

                mock.Mock<IServerClient>()
                    .Setup(c => c.checkIfFileExists(dummyRemoteFilePath))
                    .Returns(true);

                mock.Mock<IUploadServiceRepository>()
                    .Setup(r => r.GetFileByPath(dummySourcePath).HashedContent)
                    .Returns(hashDDb);

                mock.Mock<IHashHelper>()
                    .Setup(h => h.HashMatching(hashDLocal, hashDDb))
                    .Returns(true);

                mock.Mock<IServerClient>()
                    .Setup(c => c.UploadFile(dummyRemoteFilePath, dummySourcePath, overwrite));

                mock.Mock<IUploadServiceRepository>()
                    .Setup(r => r.UpdateFile(dummyDto));


                var cls = mock.Create<UploadFiles>();
                cls.UploadFile(dummySourcePath, dummyRemoteFolder);
                

                mock.Mock<IServerClient>()
                    .Verify(X => X.checkIfFileExists(dummyRemoteFilePath), Times.Exactly(1));

                mock.Mock<IHashHelper>()
                    .Verify(X => X.GenerateHash(dummySourcePath), Times.Exactly(1));

                mock.Mock<IUploadServiceRepository>()
                    .Verify(X => X.GetFileByPath(dummySourcePath).HashedContent, Times.Exactly(1));

                mock.Mock<IHashHelper>()
                    .Verify(X => X.HashMatching(hashDLocal, hashDDb), Times.Exactly(1));

                mock.Mock<IUploadServiceRepository>()
                    .Verify(r => r.UpdateFile(It.Is<FileDTO>(x => x.FilePath == dummySourcePath && x.HashedContent == hashDLocal)),Times.Never);

                mock.Mock<IServerClient>()
                    .Verify(X => X.UploadFile(dummyRemoteFilePath, dummySourcePath, overwrite), Times.Never);
            }
        }

        
        [Test]
        public void UploadFiles_FileNotExistsOnRemoteOverwriteFalse_UploadOnRemoteInsertInRepo()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var dummySourcePath = "/path/source/my.txt";
                var dummyRemoteFolder = "/path/dest/";
                var dummyRemoteFilePath = "/path/dest/my.txt";
                byte[] hashDLocal = new byte[5] {3, 10, 15, 12, 14};
                byte[] hashDDb = new byte[5] {3, 10, 15, 12, 14};
                var overwrite = false;
                var dummyDto = new FileDTO()
                {
                    FilePath = dummySourcePath, HashedContent = hashDLocal
                };

                mock.Mock<IHashHelper>()
                    .Setup(h => h.GenerateHash(dummySourcePath))
                    .Returns(hashDLocal);

                mock.Mock<IServerClient>()
                    .Setup(c => c.checkIfFileExists(dummyRemoteFilePath))
                    .Returns(false);
                

                mock.Mock<IServerClient>()
                    .Setup(c => c.UploadFile(dummyRemoteFilePath, dummySourcePath, overwrite));

                mock.Mock<IUploadServiceRepository>()
                    .Setup(r => r.InsertFile(dummyDto));


                var cls = mock.Create<UploadFiles>();
                cls.UploadFile(dummySourcePath, dummyRemoteFolder);
                

                mock.Mock<IServerClient>()
                    .Verify(X => X.checkIfFileExists(dummyRemoteFilePath), Times.Exactly(1));

                mock.Mock<IHashHelper>()
                    .Verify(X => X.GenerateHash(dummySourcePath), Times.Exactly(1));
                
                
                mock.Mock<IUploadServiceRepository>()
                    .Verify(r => r.InsertFile(It.Is<FileDTO>(x => x.FilePath == dummySourcePath && x.HashedContent == hashDLocal)));

                mock.Mock<IServerClient>()
                    .Verify(X => X.UploadFile(dummyRemoteFilePath, dummySourcePath, overwrite), Times.Exactly(1));
            }
        }
    }
}