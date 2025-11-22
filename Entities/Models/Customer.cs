using System;
using System.Collections.Generic;

namespace MilkBilling.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhotoUrl { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public decimal CowRate { get; set; }

    public decimal BuffaloRate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Milkentry> Milkentries { get; set; } = new List<Milkentry>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
