using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class State
{
    public int Id { get; set; }

    public string StateName { get; set; } = null!;

    public int? CountryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Country? Country { get; set; }
}
