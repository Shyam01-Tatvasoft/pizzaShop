using System;
using System.Collections.Generic;

namespace PizzaShop.Models;

public partial class RolePermissionMapping
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public bool? CanView { get; set; }

    public bool? CanEdit { get; set; }

    public bool? CanDelete { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
