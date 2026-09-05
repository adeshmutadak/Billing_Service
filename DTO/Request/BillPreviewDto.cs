using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class BillPreviewDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public int Month { get; set; }

        /// <summary>Short form: Jan, Feb, Mar.</summary>
        public string MonthName { get; set; }

        public int Year { get; set; }

        /// <summary>Milk entries that fed the calculation. Zero means nothing was
        /// delivered in this month.</summary>
        public int EntryCount { get; set; }

        // Calculated from milkentries for the month, by Date.
        public decimal TotalCowLitre { get; set; }
        public decimal TotalBuffaloLitre { get; set; }
        public decimal TotalAmount { get; set; }

        /// <summary>Carried forward from the customer's most recent earlier bill
        /// when that bill is not settled. It is a suggestion: PreviousBalance is
        /// still supplied by the caller on POST, so the user can override it.</summary>
        public decimal SuggestedPreviousBalance { get; set; }

        /// <summary>TotalAmount plus SuggestedPreviousBalance. What would be owed
        /// if the suggestion is accepted unchanged.</summary>
        public decimal TotalPayable { get; set; }

        /// <summary>The bill this suggestion was carried from, if any.</summary>
        public int? PreviousBillId { get; set; }
        public int? PreviousBillMonth { get; set; }
        public int? PreviousBillYear { get; set; }

        // ----- The bill already saved for this period, when one exists -----

        /// <summary>True when a bill for this customer, year and month already
        /// exists. Posting will recalculate it rather than create a second one,
        /// unless it is already settled, which is refused with 409.</summary>
        public bool BillExists { get; set; }

        public int? BillId { get; set; }
        public decimal? ExistingPreviousBalance { get; set; }
        public decimal? ExistingTotalPayable { get; set; }
        public string ExistingPaymentType { get; set; }
        public bool ExistingIsPaymentDone { get; set; }
    }
}
