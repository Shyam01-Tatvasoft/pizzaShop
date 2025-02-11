using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class WaitingToken
{
    public int Id { get; set; }

    public int? SectionId { get; set; }

    public bool? IsAssigned { get; set; }

    public int? CustomerId { get; set; }

    public int? NoOfPerson { get; set; }

    public int? TableId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Section? Section { get; set; }

    public virtual Table? Table { get; set; }
}
