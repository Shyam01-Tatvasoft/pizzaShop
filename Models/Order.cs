using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Order
{
    public int Id { get; set; }

    public decimal SubTotalAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Comment { get; set; }

    public string Status { get; set; } = null!;

    public int? CustomerId { get; set; }

    public int? NoOfPerson { get; set; }

    public decimal? Discount { get; set; }

    public decimal? Tax { get; set; }

    public bool IsGstselected { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public int? PaymentId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<OrderItemMapping> OrderItemMappings { get; set; } = new List<OrderItemMapping>();

    public virtual ICollection<OrderTaxMapping> OrderTaxMappings { get; set; } = new List<OrderTaxMapping>();

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<TableOrderMapping> TableOrderMappings { get; set; } = new List<TableOrderMapping>();
}
