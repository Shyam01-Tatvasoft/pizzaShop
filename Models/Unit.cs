using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Unit
{
    public int Id { get; set; }

    public string UnitName { get; set; } = null!;

    public string? ShortName { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
