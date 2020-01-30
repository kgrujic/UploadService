using System.ComponentModel.DataAnnotations;

namespace UploadServiceDatabase.DTOs
{
    public class FileDTO
    {
        [Key]
        public virtual string FilePath { get; set; }
        //TODO remember
        public virtual byte[] HashedContent { get; set; }
    }
}