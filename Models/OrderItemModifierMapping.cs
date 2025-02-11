using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class OrderItemModifierMapping
{
    public int Id { get; set; }

    public int? OrderItemMappingId { get; set; }

    public int? ModifierId { get; set; }

    public int Quantity { get; set; }

    public decimal? ModifierRate { get; set; }

    public decimal? TotalAmount { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Modifier? Modifier { get; set; }

    public virtual OrderItemMapping? OrderItemMapping { get; set; }
}
