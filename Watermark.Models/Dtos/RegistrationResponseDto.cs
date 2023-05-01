using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watermark.Models.Dtos
{
    public class RegistrationResponseDto
    {
        public bool IsSuccessfulRegistraion { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
