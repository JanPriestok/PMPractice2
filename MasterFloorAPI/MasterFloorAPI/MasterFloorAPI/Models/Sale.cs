using System;
using System.Collections.Generic;

namespace MasterFloorAPI.Models;
public class Sale
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public string ProductArticle { get; set; } = null!;
    public int Quantity { get; set; }
    public DateOnly SaleDate { get; set; }
    public virtual Partner Partner { get; set; } = null!;
    public virtual Product ProductArticleNavigation { get; set; } = null!;  
}
