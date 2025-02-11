using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Table
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;

    public int SectionId { get; set; }

    public bool? IsAvailable { get; set; }

    public string? Status { get; set; }

    public int? Capacity { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<TableOrderMapping> TableOrderMappings { get; set; } = new List<TableOrderMapping>();

    public virtual ICollection<WaitingToken> WaitingTokens { get; set; } = new List<WaitingToken>();
}
