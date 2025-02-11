using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class TaxesAndFee
{
    public int Id { get; set; }

    public string TaxName { get; set; } = null!;

    public decimal TaxPercentage { get; set; }

    public decimal? FlatAmount { get; set; }

    public bool? IsEnabled { get; set; }

    public bool? IsDefaultTax { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<OrderTaxMapping> OrderTaxMappings { get; set; } = new List<OrderTaxMapping>();
}
