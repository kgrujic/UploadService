using System.ComponentModel.DataAnnotations;

namespace UploadService.DTOs
{
    public class FileDTO
    {
        [Key]
        public string FilePath { get; set; }
        public byte[] HashedContent { get; set; }
    }
}