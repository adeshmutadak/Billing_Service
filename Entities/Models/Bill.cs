using System;
using System.Collections.Generic;

namespace MilkBilling.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal? TotalCowLitre { get; set; }

    public decimal? TotalBuffaloLitre { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? PreviousBalance { get; set; }

    public decimal? TotalPayable { get; set; }
    public decimal PaidAmount { get; set; }

    public bool? IsDeleted { get; set; }

    /// <summary>How the month's bill was settled. Mirrors payments.PaymentType.</summary>
    public string? PaymentType { get; set; }

    /// <summary>Whether this month's bill is settled. Authoritative for the month,
    /// where payments.IsPaymentDone is per transaction.</summary>
    public bool IsPaymentDone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
