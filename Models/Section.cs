using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Section
{
    public int Id { get; set; }

    public string SectionName { get; set; } = null!;

    public string? SectionDescription { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual ICollection<WaitingToken> WaitingTokens { get; set; } = new List<WaitingToken>();
}
