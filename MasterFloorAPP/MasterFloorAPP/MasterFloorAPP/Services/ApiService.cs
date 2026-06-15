using System.Net.Http.Json;
using MasterFloorAPP.Models;

namespace MasterFloorAPP.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5142";

    public ApiService()
    {
        _httpClient = new HttpClient();
    }
    public async Task<List<Partner>?> GetPartnersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/partners");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Partner>>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return null;
        }
    }
}