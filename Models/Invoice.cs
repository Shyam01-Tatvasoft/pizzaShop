using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Invoice
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? ModifierId { get; set; }

    public int? QuantityOfModifier { get; set; }

    public decimal? RateOfModifier { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Modifier? Modifier { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
