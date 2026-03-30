using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    public class PaymentResponseDto
    {
      //  public int PaymentId { get; set; }
        public string? PaymentType { get; set; } = null!;

        public bool? IsPaymentDone { get; set; }
        public DateOnly? Date { get; set; }
        public decimal Amount { get; set; }
        public decimal Remaning { get; set; }
    }
}
