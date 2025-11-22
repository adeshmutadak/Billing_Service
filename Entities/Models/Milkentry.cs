using System;
using System.Collections.Generic;

namespace MilkBilling.Models;

public partial class Milkentry
{
    public int EntryId { get; set; }

    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public DateOnly Date { get; set; }

    public decimal? CowLitre { get; set; }

    public decimal? BuffaloLitre { get; set; }

    public decimal? CowRate { get; set; }

    public decimal? BuffaloRate { get; set; }

    public decimal? TotalAmount { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
