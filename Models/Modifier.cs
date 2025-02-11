using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Modifier
{
    public int Id { get; set; }

    public string ModifierName { get; set; } = null!;

    public string? ModifierDescription { get; set; }

    public decimal Rate { get; set; }

    public int Quantity { get; set; }

    public int UnitId { get; set; }

    public int? ModifiergroupId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ModifierGroup? Modifiergroup { get; set; }

    public virtual ICollection<OrderItemModifierMapping> OrderItemModifierMappings { get; set; } = new List<OrderItemModifierMapping>();

    public virtual Unit Unit { get; set; } = null!;
}
