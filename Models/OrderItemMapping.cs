using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class OrderItemMapping
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? ItemId { get; set; }

    public int Quantity { get; set; }

    public string? Instruction { get; set; }

    public string Status { get; set; } = null!;

    public string? ItemName { get; set; }

    public decimal? ItemRate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? TotalModifierAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public decimal? Tax { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual MenuItem? Item { get; set; }

    public virtual Order? Order { get; set; }

    public virtual ICollection<OrderItemModifierMapping> OrderItemModifierMappings { get; set; } = new List<OrderItemModifierMapping>();
}
