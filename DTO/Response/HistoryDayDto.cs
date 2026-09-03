using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Response
{
    /// <summary>One day in the milk register. EntryId is null on a padded day.</summary>
    public class HistoryDayDto
    {
        public int? EntryId { get; set; }
        public DateOnly Date { get; set; }
        public int DayOfMonth { get; set; }
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
        public DateOnly? Date { get; set; }
        public decimal Amount { get; set; }

        /// <summary>Maps the misspelled payments.Remaning column.</summary>
        public decimal Remaining { get; set; }

        public string PaymentType { get; set; }
        public bool IsPaymentDone { get; set; }
    }

    public class HistoryMonthDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal TotalCowLitre { get; set; }
        public decimal TotalBuffaloLitre { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        /// <summary>Paid, Partial, Unpaid or NoActivity.</summary>
        public string PaymentStatus { get; set; }
        public bool IsPaid { get; set; }

        public List<HistoryDayDto> Days { get; set; } = new();
        public List<HistoryPaymentDto> Payments { get; set; } = new();
    }

    public class HistoryCustomerDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string WhatsappNumber { get; set; }
        public string Address { get; set; }
        public decimal CowRate { get; set; }
        public decimal BuffaloRate { get; set; }

        public List<HistoryMonthDto> Months { get; set; } = new();
    }

    public class HistoryTotalsDto
    {
        public int CustomerCount { get; set; }
        public int PaidCustomerCount { get; set; }
        public int UnpaidCustomerCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    public class HistoryResponseDto
    {
        public HistoryTotalsDto Totals { get; set; }
        public List<HistoryCustomerDto> Customers { get; set; } = new();
    }

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
    }

}
