using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.Bot.Models
{
    public class UserData
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public int State { get; set; }
        public string? SourceImageStoredFileName { get; set; }
        public string? WatermarkImageStoredFileName { get; set; }
        public DateTime LastContact { get; set; }
    }
}
