using System.Collections.Generic;
using UploadService.Configurations.UploadTypeConfgurations;

namespace UploadService.Configurations.UploadStrategies
{
    /// <summary>
    /// Generic Interface IUploadStrategy
    /// </summary>
    /// <typeparam name="T">Generic Type T must implement IUploadTypeConfiguration interface</typeparam>
    public interface IUploadStrategy<T> where T : IUploadTypeConfiguration
    {
        /// <summary>
        /// Perform upload
        /// </summary>
        /// <param name="list"></param>
        public void Upload(IEnumerable<T> list);

        /// <summary>
        /// Perform startup upload
        /// </summary>
        /// <param name="list"></param>
        public void StartUpUpload(IEnumerable<T> list);
    }
}