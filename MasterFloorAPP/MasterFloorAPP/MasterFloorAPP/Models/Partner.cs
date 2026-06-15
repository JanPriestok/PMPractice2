namespace MasterFloorAPP.Models;

public class Partner
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartnerType { get; set; } = string.Empty;
    public string DirectorFullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Address { get; set; } = string.Empty;
    public long TotalSalesQuantity { get; set; }
    public int DiscountPercent { get; set; }
}