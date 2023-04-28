using System.ComponentModel.DataAnnotations;

namespace WatermarkApi.Models
{
    public class StoredImage
    {
        [Key]
        public string StoredName { get; set; }
        public DateTime ExpireDateTime { get; set; }
    }
}
