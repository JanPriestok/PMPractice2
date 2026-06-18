using System;
using System.Collections.Generic;

namespace MasterFloorAPI.Models;

public partial class Partner
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public string? DirectorLastName { get; set; }

    public string? DirectorFirstName { get; set; }

    public string? DirectorMiddleName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int LegalAddressId { get; set; }

    public string Inn { get; set; } = null!;

    public int? Rating { get; set; }

    public virtual Address LegalAddress { get; set; } = null!;

    public virtual ICollection<PartnerRatingHistory> PartnerRatingHistories { get; set; } = new List<PartnerRatingHistory>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual PartnerType Type { get; set; } = null!;
}
