using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class MenuItem
{
    public int Id { get; set; }

    public string ItemName { get; set; } = null!;

    public string? ItemDescription { get; set; }

    public int CategoryId { get; set; }

    public string ItemType { get; set; } = null!;

    public decimal Rate { get; set; }

    public int Quantity { get; set; }

    public int UnitId { get; set; }

    public bool? IsAvailable { get; set; }

    public bool IsDefaultTax { get; set; }

    public decimal? TaxPercentage { get; set; }

    public string? ShortCode { get; set; }

    public bool? IsFavourite { get; set; }

    public string? ItemImage { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ItemCategory Category { get; set; } = null!;

    public virtual ICollection<ItemModifiergroupMapping> ItemModifiergroupMappings { get; set; } = new List<ItemModifiergroupMapping>();

    public virtual ICollection<OrderItemMapping> OrderItemMappings { get; set; } = new List<OrderItemMapping>();

    public virtual Unit Unit { get; set; } = null!;
}
