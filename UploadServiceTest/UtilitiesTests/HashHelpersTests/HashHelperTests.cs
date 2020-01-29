using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.DotNet.PlatformAbstractions;
using NUnit.Framework;
using UploadService.Utilities.HashHelpers;

namespace UploadServiceTest.UtilitiesTests.HashHelpersTests
{
    public class HashHelperTests
    {
        private IHashHelper _hashHelper;

        [SetUp]
        public void Setup()
        {
            _hashHelper = new HashHelper();
        }

        [Test]
        public void GenerateHash_ValidFilePath_Successful()
        {
          var expectedValue = "E9-02-37-3C-E2-47-A8-EA-25-91-0F-12-D1-57-3D-54";
          var actualValue = BitConverter.ToString(_hashHelper.GenerateHash(@"././Stubs/MyStub.txt"));
          Assert.That(expectedValue,Is.EqualTo(actualValue)); 

        }
        
        [Test]
        public void GenerateHash_InvalidFilePath_ThrowException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _hashHelper.GenerateHash("./dummy/path"));
        }
        
        [Test]
        public void HashMatching_SameHashValues_ReturnsTrue()
        {
            var hashOne = "E9-02-37-3C-E2-47-A8-EA-25-91-0F-12-D1-57-3D-54";
            var hashTwo = "E9-02-37-3C-E2-47-A8-EA-25-91-0F-12-D1-57-3D-54";

            var result = _hashHelper.HashMatching(Encoding.ASCII.GetBytes(hashOne), Encoding.ASCII.GetBytes(hashTwo));
            Assert.That(result,Is.EqualTo(true)); 

        }   
        [Test]
        public void HashMatching_DifferentHashValues_ReturnsFalse()
        {
            var hashOne = "E9-02-37-3C-E2-47-A8-EA-25-91-0F-12-D1-57-3D-54";
            var hashTwo = "E9-02-37-3C-E2-47-A8-EA-25-91-0F-12-D1-57-3D-55";

            var result = _hashHelper.HashMatching(Encoding.ASCII.GetBytes(hashOne), Encoding.ASCII.GetBytes(hashTwo));
            Assert.That(result,Is.EqualTo(true)); 

        }
        

    }
}