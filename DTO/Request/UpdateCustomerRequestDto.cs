using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class UpdateCustomerRequestDto
    {
        public int CustomerId { get; set; }   // Required for identification

        public string? Name { get; set; }
        public string? Address { get; set; }

        public string? Base64Photo { get; set; } // optional photo update

        public string? WhatsappNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public decimal? CowRate { get; set; }
        public decimal? BuffaloRate { get; set; }
    }

}
