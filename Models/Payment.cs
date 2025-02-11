using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string Method { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public int? InvoiceId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
