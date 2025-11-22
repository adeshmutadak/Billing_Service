using System;
using System.Collections.Generic;

namespace MilkBilling.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public string PaymentType { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? Reference { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
