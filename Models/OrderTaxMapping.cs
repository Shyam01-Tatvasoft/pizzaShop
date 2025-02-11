using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class OrderTaxMapping
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? TaxId { get; set; }

    public decimal? TaxValue { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Order? Order { get; set; }

    public virtual TaxesAndFee? Tax { get; set; }
}
