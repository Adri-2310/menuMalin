using System.Text.Json;
using menuMalin.Shared.Modeles.DTOs;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour intégrer l'API TheMealDB
/// </summary>
public class ServiceMealDB : IServiceMealDB
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceMealDB> _logger;
    private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1/";

    public ServiceMealDB(HttpClient httpClient, ILogger<ServiceMealDB> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<RecetteMealDTO?> GetRandomAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("random.php");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecetteMealResponse>(content);

            return result?.Meals?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération d'une recette aléatoire depuis TheMealDB");
            return null;
        }
    }

    public async Task<List<RecetteMealDTO>> SearchByNameAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            var response = await _httpClient.GetAsync($"search.php?s={query}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecetteMealResponse>(content);

            return result?.Meals ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche de recettes par nom dans TheMealDB");
            return new();
        }
    }

    public async Task<RecetteMealDTO?> GetByIdAsync(string mealId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mealId))
                return null;

            var response = await _httpClient.GetAsync($"lookup.php?i={mealId}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecetteMealResponse>(content);

            return result?.Meals?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération d'une recette depuis TheMealDB");
            return null;
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("categories.php");
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
            _logger.LogError(ex, "Erreur lors de la recherche de recettes par nom dans TheMealDB");
            return new();
        }
    }

    public async Task<List<string>> GetAreasAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("list.php?a=list");
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
            _logger.LogError(ex, "Erreur lors de la recherche de recettes par nom dans TheMealDB");
            return new();
        }
    }

    public async Task<List<RecetteMealDTO>> FilterByCategoryAsync(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return new();

            var response = await _httpClient.GetAsync($"filter.php?c={category}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecetteMealResponse>(content);

            return result?.Meals ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche de recettes par nom dans TheMealDB");
            return new();
        }
    }

    public async Task<List<RecetteMealDTO>> FilterByAreaAsync(string area)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(area))
                return new();

            var response = await _httpClient.GetAsync($"filter.php?a={area}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecetteMealResponse>(content);

            return result?.Meals ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche de recettes par nom dans TheMealDB");
            return new();
        }
    }
}
