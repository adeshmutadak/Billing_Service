using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Request
{
    public class HistoryFilterDto
    {
        /// <summary>Required. Scopes customers, milk entries, bills and payments.</summary>
        public int Year { get; set; }

        /// <summary>Limit to one customer.</summary>
        public int? CustomerId { get; set; }

        /// <summary>Keep only this month inside each customer. Omit for all twelve.</summary>
        public int? Month { get; set; }

        /// <summary>Filter on payments.IsPaymentDone for the month.
        /// true = settled, false = not settled, null = no filter.</summary>
        public bool? IsPaymentDone { get; set; }

        /// <summary>By default only customers whose customers.CreatedAt falls in
        /// Year are returned. Set true to return every customer of the user
        /// instead. Note the default hides a customer created in an earlier year
        /// even when they have deliveries in Year, which also lowers the totals.</summary>
        public bool IncludeAllCustomers { get; set; }

        /// <summary>Include day and payment lists inside every month. Off by
        /// default so a year query stays light. Always on when Month is given.</summary>
        public bool IncludeDetail { get; set; }

        /// <summary>Pad each month with zero rows for days that had no delivery.
        /// Intended for a single-month view.</summary>
        public bool IncludeEmptyDays { get; set; }
    }

    /// <summary>One day in the milk register, from milkentries.Date.
    /// EntryId is null on a padded day.</summary>
    public class HistoryDayDto
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

    public class HistoryPaymentDto
    {
        public int PaymentId { get; set; }

        /// <summary>The month this payment settles, from payments.Date. A payment
        /// recorded in September for the August bill carries an August date.</summary>
        public DateOnly? Date { get; set; }

        /// <summary>When the payment was actually recorded, from payments.CreatedAt.</summary>
        public DateTime? RecordedAt { get; set; }

        public decimal Amount { get; set; }

        /// <summary>Maps the misspelled payments.Remaning column.</summary>
        public decimal Remaining { get; set; }

        public string PaymentType { get; set; }
        public bool IsPaymentDone { get; set; }
    }

    /// <summary>One month for one customer.</summary>
    public class HistoryMonthDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; }

        // Rolled up from milkentries for this month.
        public decimal TotalCowLitre { get; set; }
        public decimal TotalBuffaloLitre { get; set; }
        public decimal TotalAmount { get; set; }

        // From the bills row for this customer, year and month, when one exists.
        public bool BillGenerated { get; set; }
        public int? BillId { get; set; }
        public decimal? BillTotalAmount { get; set; }
        public decimal? BillPreviousBalance { get; set; }
        public decimal? BillTotalPayable { get; set; }

        /// <summary>What is owed. BillTotalPayable when a bill exists, since that
        /// carries any previous balance; otherwise TotalAmount.</summary>
        public decimal PayableAmount { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        /// <summary>True when a payment for this month is flagged done.</summary>
        public bool IsPaymentDone { get; set; }

        /// <summary>Paid, Partial, Unpaid or NoActivity. Derived from money.</summary>
        public string PaymentStatus { get; set; }
        public bool IsPaid { get; set; }

        public List<HistoryDayDto> Days { get; set; } = new();
        public List<HistoryPaymentDto> Payments { get; set; } = new();
    }

    /// <summary>One customer and their months for the year.</summary>
    public class HistoryCustomerDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string WhatsappNumber { get; set; }
        public string Address { get; set; }
        public decimal CowRate { get; set; }
        public decimal BuffaloRate { get; set; }

        /// <summary>From customers.CreatedAt, the field the year filter uses.</summary>
        public DateTime? CreatedAt { get; set; }

        public List<HistoryMonthDto> Months { get; set; } = new();
    }

    /// <summary>Year rollup across the returned customers. Always describes the
    /// whole year, never the filtered subset.</summary>
    public class HistoryTotalsDto
    {
        public int CustomerCount { get; set; }
        public int PaidCustomerCount { get; set; }
        public int UnpaidCustomerCount { get; set; }

        /// <summary>Milk delivered, from milkentries.</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Owed, using bill payables where bills exist.</summary>
        public decimal PayableAmount { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    public class HistoryResponseDto
    {
        public HistoryTotalsDto Totals { get; set; }
        public List<HistoryCustomerDto> Customers { get; set; } = new();
    }

    /// <summary>Dropdown values for the filter screen.</summary>
    public class HistoryFilterOptionsDto
    {
        public List<int> Years { get; set; } = new();
        public List<HistoryCustomerOptionDto> Customers { get; set; } = new();
    }

    public class HistoryCustomerOptionDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
