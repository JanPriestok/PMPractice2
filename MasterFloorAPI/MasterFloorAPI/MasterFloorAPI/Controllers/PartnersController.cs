using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasterFloorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartnersController : ControllerBase
{
    private readonly MasterFloorContext _context;

    public PartnersController(MasterFloorContext context)
    {
        _context = context;
    }

    // GET: api/partners
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        //Загрузка партнёров
        var partners = await _context.Partners
            .Include(p => p.Type)
            .Include(p => p.LegalAddress)
            .ToListAsync();

        //Подсчёт общего количества проданной продукции каждому партнёру
        var result = new List<object>();

        foreach (var p in partners)
        {
            var totalQuantity = await _context.SaleHeaders
                .Where(sh => sh.PartnerId == p.Id)
                .SelectMany(sh => sh.SaleItems)
                .SumAsync(si => si.Quantity);

            //Расчёт скидки
            int discount = 0;
            if (totalQuantity >= 300000)
                discount = 15;
            else if (totalQuantity >= 50000)
                discount = 10;
            else if (totalQuantity >= 10000)
                discount = 5;
            result.Add(new
            {
                p.Id,
                p.Name,
                PartnerType = p.Type?.Name ?? "",
                DirectorFullName = $"{p.DirectorLastName} {p.DirectorFirstName} {p.DirectorMiddleName}".Trim(),
                p.Phone,
                p.Rating,
                Address = p.LegalAddress == null ? "" :
                    $"{p.LegalAddress.PostalCode}, {p.LegalAddress.City}, {p.LegalAddress.Street}, {p.LegalAddress.House}",
                TotalSalesQuantity = totalQuantity,
                DiscountPercent = discount
            });
        }
        return Ok(result);
    }
}