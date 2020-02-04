using System.Collections.Generic;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Configurations.UploadTypeConfgurations.Implementations;
using UploadService.Utilities;

namespace UploadService.Configurations.UploadStrategies
{
    public interface IUploadStrategy<T> where T : IUploadTypeConfiguration
    {
        public void Upload(IEnumerable<T> list);

        public void StartUpUpload(IEnumerable<T> list);
    }
}