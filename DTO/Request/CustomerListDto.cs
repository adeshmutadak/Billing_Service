using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class CustomerListDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
        public string? WhatsappNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public decimal CowRate { get; set; }
        public decimal BuffaloRate { get; set; }
    }

}
