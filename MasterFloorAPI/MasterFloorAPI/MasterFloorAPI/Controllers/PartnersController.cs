using MasterFloorAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

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
        var partners = await _context.Partners
            .Include(p => p.Type)
            .Include(p => p.LegalAddress)
            .ToListAsync();

        var result = new List<object>();
        foreach (var p in partners)
        {
            var totalQuantity = await _context.Sale
                .Where(sh => sh.PartnerId == p.Id)
                //.SelectMany(sh => sh.Sale)
                .SumAsync(si => si.Quantity);

            int discount = CalculateDiscount(totalQuantity);
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

    // GET: api/partners/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var partner = await _context.Partners
            .Include(p => p.Type)
            .Include(p => p.LegalAddress)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partner == null)
            return NotFound($"Партнёр с Id = {id} не найден.");

        // Возвращаем полный объект Partner (с вложенным Address)
        return Ok(partner);
    }
    [HttpGet("types")]
    public async Task<IActionResult> GetPartnerTypes()
    {
        var types = await _context.PartnerTypes.ToListAsync();
        return Ok(types);
    }

    // POST: api/partners
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Partner partner)
    {
        try
        {
            // Валидация рейтинга
            if (partner.Rating.HasValue && partner.Rating.Value < 0)
                return BadRequest("Рейтинг не может быть отрицательным.");

            // Проверка наличия адреса
            if (partner.LegalAddress == null)
                return BadRequest("Адрес обязателен.");

            // Создаём новый адрес (Id не задаём, он сгенерируется БД)
            var newAddress = new Address
            {
                PostalCode = partner.LegalAddress.PostalCode,
                Area = partner.LegalAddress.Area,
                City = partner.LegalAddress.City,
                Street = partner.LegalAddress.Street,
                House = partner.LegalAddress.House
            };
            _context.Addresses.Add(newAddress);
            await _context.SaveChangesAsync(); // Получаем Id адреса

            // Создаём партнёра
            var newPartner = new Partner
            {
                Name = partner.Name,
                TypeId = partner.TypeId,
                Rating = partner.Rating,
                DirectorLastName = partner.DirectorLastName,
                DirectorFirstName = partner.DirectorFirstName,
                DirectorMiddleName = partner.DirectorMiddleName,
                Phone = partner.Phone,
                Email = partner.Email,
                LegalAddressId = newAddress.Id,
                Inn = "0000000000" // Заглушка, т.к. INN не требуется в задании
            };
            _context.Partners.Add(newPartner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newPartner.Id }, newPartner);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }

    // PUT: api/partners/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Partner partner)
    {
        try
        {
            var existingPartner = await _context.Partners
                .Include(p => p.LegalAddress)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPartner == null)
                return NotFound($"Партнёр с Id = {id} не найден.");

            // Валидация рейтинга
            if (partner.Rating.HasValue && partner.Rating.Value < 0)
                return BadRequest("Рейтинг не может быть отрицательным.");

            // Обновляем поля партнёра
            existingPartner.Name = partner.Name;
            existingPartner.TypeId = partner.TypeId;
            existingPartner.Rating = partner.Rating;
            existingPartner.DirectorLastName = partner.DirectorLastName;
            existingPartner.DirectorFirstName = partner.DirectorFirstName;
            existingPartner.DirectorMiddleName = partner.DirectorMiddleName;
            existingPartner.Phone = partner.Phone;
            existingPartner.Email = partner.Email;

            // Обновляем адрес (если передан)
            if (partner.LegalAddress != null)
            {
                existingPartner.LegalAddress.PostalCode = partner.LegalAddress.PostalCode;
                existingPartner.LegalAddress.Area = partner.LegalAddress.Area;
                existingPartner.LegalAddress.City = partner.LegalAddress.City;
                existingPartner.LegalAddress.Street = partner.LegalAddress.Street;
                existingPartner.LegalAddress.House = partner.LegalAddress.House;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
        }
    }
    private int CalculateDiscount(int totalQuantity)
    {
        int discount;

        if (totalQuantity < 10000)
        {
            discount = 0;
        }
        else if (totalQuantity < 50000)
        {
            discount = 5;
        }
        else if (totalQuantity < 300000)
        {
            discount = 10;
        }
        else
        {
            discount = 15;
        }

        return discount;
    }
}