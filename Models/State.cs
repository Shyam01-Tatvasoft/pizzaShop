﻿using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class State
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Countryid { get; set; }

    public DateTime? Createddate { get; set; }

    public string? Createdby { get; set; }

    public DateTime? Updateddate { get; set; }

    public string? Updatedby { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country? Country { get; set; }
}
