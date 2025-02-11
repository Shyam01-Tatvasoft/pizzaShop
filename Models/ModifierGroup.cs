using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class ModifierGroup
{
    public int Id { get; set; }

    public string GroupName { get; set; } = null!;

    public string? GroupDescription { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<ItemModifiergroupMapping> ItemModifiergroupMappings { get; set; } = new List<ItemModifiergroupMapping>();

    public virtual ICollection<Modifier> Modifiers { get; set; } = new List<Modifier>();
}
