using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class ItemModifiergroupMapping
{
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public int? ModifiergroupId { get; set; }

    public int? MinSelection { get; set; }

    public int? MaxSelection { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual MenuItem? Item { get; set; }

    public virtual ModifierGroup? Modifiergroup { get; set; }
}
