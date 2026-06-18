using System;
using System.Collections.Generic;

namespace MasterFloorAPI.Models;

public partial class Product
{
    public string Article { get; set; } = null!;

    public int TypeId { get; set; }

    public int? MaterialId { get; set; }

    public string Name { get; set; } = null!;

    public decimal MinPartnerPrice { get; set; }

    public virtual Material? Material { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ProductType Type { get; set; } = null!;
}
