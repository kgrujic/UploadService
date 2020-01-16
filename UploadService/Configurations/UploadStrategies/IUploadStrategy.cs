using System.Collections.Generic;
using UploadService.Configurations.UploadTypeConfgurations;
using UploadService.Utilities;

namespace UploadService.Configurations.UploadStrategies
{
    public interface IUploadStrategy
    {
         public void Upload();
    }
}