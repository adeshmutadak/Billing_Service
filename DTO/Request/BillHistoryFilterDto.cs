using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public enum CustomerScope
    {
        /// <summary>Every customer of the user, whatever the year. Default.</summary>
        All = 0,

        /// <summary>Only customers whose customers.CreatedAt falls in Year.
        /// A long-standing customer you still deliver to is excluded, because
        /// they were created in an earlier year.</summary>
        CreatedInYear = 1,

        /// <summary>Customers who belong to that year: they have a milk entry or a
        /// bill in it, or they were created in it. Recommended for the home screen,
        /// since it keeps a newly added customer visible before their first
        /// delivery and drops customers who are no longer being served.</summary>
        CurrentInYear = 2
    }

    /// <summary>Filters for the shared customer-and-history query.
    /// The customer home screen passes the current year; the history screen
    /// passes whichever year the user picked. Nothing else differs.</summary>
    public class BillHistoryFilterDto
    {
        public int Year { get; set; }          // required
        public int? Month { get; set; }
        public bool? IsPaid { get; set; }
        public int? CustomerId { get; set; }
        public bool IncludeDetail { get; set; }
        public bool IncludeEmptyDays { get; set; }
    }
    /// <summary>One milk entry. EntryId is null on a padded day.</summary>
    public class BillHistoryDayDto
    {
        public int? EntryId { get; set; }
        public DateOnly Date { get; set; }
        public int DayOfMonth { get; set; }

        /// <summary>Monday, Tuesday, and so on.</summary>
        public string DayName { get; set; }

        public decimal CowLitre { get; set; }
        public decimal BuffaloLitre { get; set; }
        public decimal CowRate { get; set; }
        public decimal BuffaloRate { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>One month for one customer. Litres and TotalAmount come from
    /// milkentries; the payment figures come from the bills row for the same
    /// Year and Month.</summary>
    public class BillHistoryMonthDto
    {
        public int Month { get; set; }

        /// <summary>Short form: Jan, Feb, Mar.</summary>
        public string MonthName { get; set; }

        public decimal TotalCowLitre { get; set; }
        public decimal TotalBuffaloLitre { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        /// <summary>Paid, Unpaid or NoActivity.</summary>
        public string PaymentStatus { get; set; }
        public bool IsPaid { get; set; }
        public decimal PreviousBalance { get; set; }

        /// <summary>TotalAmount plus PreviousBalance. What was owed.</summary>
        public decimal PayableAmount { get; set; }

        /// <summary>Ascending by date, then by EntryId when a date holds several
        /// entries. Empty unless IncludeDetail or Month was supplied.</summary>
        public List<BillHistoryDayDto> Days { get; set; } = new();
    }

    /// <summary>One customer and their months, January through December.</summary>
    public class BillHistoryCustomerDto
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhotoUrl { get; set; }
        public string WhatsappNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal CowRate { get; set; }
        public decimal BuffaloRate { get; set; }

        /// <summary>Ascending by month number.</summary>
        public List<BillHistoryMonthDto> Months { get; set; } = new();
    }

    /// <summary>Rollup across the returned customers for the whole year, never
    /// the filtered subset, so a header does not move while filters change.</summary>
    public class BillHistoryTotalsDto
    {
        public int CustomerCount { get; set; }
        public int PaidCustomerCount { get; set; }
        public int UnpaidCustomerCount { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    public class BillHistoryResponseDto
    {
        public BillHistoryTotalsDto Totals { get; set; }
        public List<BillHistoryCustomerDto> Customers { get; set; } = new();
    }
}
