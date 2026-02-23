using System.Text.Json;
using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour intégrer l'API TheMealDB
/// </summary>
public class TheMealDBService : ITheMealDBService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1";

    public TheMealDBService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<MealDto?> GetRandomAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/random.php");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MealResponse>(content);

            return result?.Meals?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            // Log l'erreur (à implémenter avec Serilog)
            Console.WriteLine($"Erreur GetRandom: {ex.Message}");
            return null;
        }
    }

    public async Task<List<MealDto>> SearchByNameAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            var response = await _httpClient.GetAsync($"/search.php?s={query}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MealResponse>(content);

            return result?.Meals ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur SearchByName: {ex.Message}");
            return new();
        }
    }

    public async Task<MealDto?> GetByIdAsync(string mealId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mealId))
                return null;

            var response = await _httpClient.GetAsync($"/lookup.php?i={mealId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MealResponse>(content);

            return result?.Meals?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur GetById: {ex.Message}");
            return null;
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/categories.php");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // Parser la réponse pour extraire les catégories
            using var doc = JsonDocument.Parse(content);
            var categories = new List<string>();

            if (doc.RootElement.TryGetProperty("categories", out var array))
            {
                foreach (var item in array.EnumerateArray())
                {
                    if (item.TryGetProperty("strCategory", out var categoryName))
                    {
                        categories.Add(categoryName.GetString() ?? string.Empty);
                    }
                }
            }

            return categories;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur GetCategories: {ex.Message}");
            return new();
        }
    }

    public async Task<List<string>> GetAreasAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/list.php?a=list");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var areas = new List<string>();

            if (doc.RootElement.TryGetProperty("meals", out var array))
            {
                foreach (var item in array.EnumerateArray())
                {
                    if (item.TryGetProperty("strArea", out var areaName))
                    {
                        areas.Add(areaName.GetString() ?? string.Empty);
                    }
                }
            }

            return areas;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur GetAreas: {ex.Message}");
            return new();
        }
    }

    public async Task<List<MealDto>> FilterByCategoryAsync(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return new();

            var response = await _httpClient.GetAsync($"/filter.php?c={category}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MealResponse>(content);

            return result?.Meals ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur FilterByCategory: {ex.Message}");
            return new();
        }
    }

    public async Task<List<MealDto>> FilterByAreaAsync(string area)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(area))
                return new();

            var response = await _httpClient.GetAsync($"/filter.php?a={area}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MealResponse>(content);

            return result?.Meals ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur FilterByArea: {ex.Message}");
            return new();
        }
    }
}
