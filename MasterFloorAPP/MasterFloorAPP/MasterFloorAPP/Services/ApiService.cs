using System.Net.Http.Json;
using MasterFloorAPP.Models;

namespace MasterFloorAPP.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5150";

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<PartnerListItem>?> GetPartnersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/partners");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<PartnerListItem>>();
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return null;
        }
    }


    public async Task<bool> CreatePartnerAsync(Partner partner)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/partners", partner);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка создания: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdatePartnerAsync(int id, Partner partner)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/api/partners/{id}", partner);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обновления: {ex.Message}");
            return false;
        }
    }

    public async Task<Partner?> GetPartnerDetailAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/partners/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Partner>();
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения деталей: {ex.Message}");
            return null;
        }
    }

    public async Task<List<PartnerType>?> GetPartnerTypesAsync()
    {
        try
        {
            // Правильный путь — /api/partners/types
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/partners/types");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<PartnerType>>();
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка получения типов: {ex.Message}");
            return null;
        }
    }


}