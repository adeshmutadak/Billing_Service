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

        /// <summary>The previous bill's TotalPayable minus its PaidAmount. Only
        /// the immediately preceding bill is read, because its payable already
        /// rolls up every balance before it. Not editable by the caller: the same
        /// figure is recalculated when the bill is saved.</summary>
        public decimal PreviousBalance { get; set; }

        /// <summary>TotalAmount plus PreviousBalance. What is owed.</summary>
        public decimal TotalPayable { get; set; }

        /// <summary>Which bill the balance was carried from, so the UI can say
        /// "carried from February 2026".</summary>
        public int? PreviousBillId { get; set; }
        public int? PreviousBillMonth { get; set; }
        public int? PreviousBillYear { get; set; }

        // ----- The bill already saved for this period, when one exists -----

        /// <summary>True when a bill for this customer, year and month already
        /// exists. Posting recalculates it rather than creating a second one,
        /// unless it is already settled, which is refused with 409.</summary>
        public bool BillExists { get; set; }

        public int? BillId { get; set; }
        public decimal? ExistingPaidAmount { get; set; }
        public decimal? ExistingTotalPayable { get; set; }
        public decimal? ExistingRemainingAmount { get; set; }
        public string ExistingPaymentType { get; set; }
        public bool ExistingIsPaymentDone { get; set; }

        /// <summary>How many bills exist after this period. Saving this one
        /// recalculates their previous balances, so the UI can warn first.</summary>
        public int LaterBillCount { get; set; }
    }
}
