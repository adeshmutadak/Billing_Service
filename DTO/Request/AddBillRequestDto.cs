using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class AddBillRequestDto
    {
        public int CustomerId { get; set; }

        /// <summary>Accepted for backward compatibility but ignored. The user is
        /// taken from the JWT so a caller cannot bill on another user's behalf.</summary>
        public int UserId { get; set; }

        /// <summary>1 to 12. The month being billed, which need not be the
        /// current one: September can bill August.</summary>
        public int Month { get; set; }

        public int Year { get; set; }

        /// <summary>What the customer is paying now. May be less than the total
        /// payable; the shortfall carries into the next month automatically.
        /// Null is treated as zero.</summary>
        public decimal? PaidAmount { get; set; }

        /// <summary>Cash, UPI, and so on.</summary>
        public string? PaymentType { get; set; }
    }

    /// <summary>The generated bill, with the figures the server calculated.</summary>
    public class BillResponseDto
    {
        public int BillId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Year { get; set; }

        /// <summary>Milk entries that fed the calculation.</summary>
        public int EntryCount { get; set; }

        public decimal TotalCowLitre { get; set; }
        public decimal TotalBuffaloLitre { get; set; }

        /// <summary>Sum of milkentries.TotalAmount for the month.</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Carried in from the previous bill's unpaid balance.</summary>
        public decimal PreviousBalance { get; set; }

        /// <summary>TotalAmount plus PreviousBalance.</summary>
        public decimal TotalPayable { get; set; }

        /// <summary>What was paid against this bill.</summary>
        public decimal PaidAmount { get; set; }

        /// <summary>TotalPayable minus PaidAmount. Carries into the next month.</summary>
        public decimal RemainingAmount { get; set; }

        public string PaymentType { get; set; }
        public bool IsPaymentDone { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>True when an existing bill for this period was recalculated
        /// rather than a new one created.</summary>
        public bool Regenerated { get; set; }

        /// <summary>How many later bills had their previous balance recomputed
        /// as a result of this save.</summary>
        public int ForwardBillsUpdated { get; set; }
    }
}
