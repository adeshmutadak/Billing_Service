using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AddCustomerRequestDto
    {
        public int UserId { get; set; }     // Required

        public string Name { get; set; } = null!; // Required

        public string? Address { get; set; }      // Optional

        public string? PhotoUrl { get; set; }     // Optional (as per requirement)

        public string? WhatsappNumber { get; set; } // Optional

        public string PhoneNumber { get; set; }    // Optional

        public string? Email { get; set; }          // Optional (as per requirement)

        public decimal CowRate { get; set; }        // Required

        public decimal BuffaloRate { get; set; }    // Required
    }

}
