using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class Feedback
{
    public int Id { get; set; }

    public int? Food { get; set; }

    public int? Serivce { get; set; }

    public int? Ambience { get; set; }

    public string? Comment { get; set; }

    public int? OrderId { get; set; }

    public decimal? AvgRating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Order? Order { get; set; }
}
