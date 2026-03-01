using menuMalin.DTOs;
using menuMalin.Shared.Modeles.DTOs;

namespace menuMalin.Services;

/// <summary>
/// Service pour les recettes (Frontend) - communique avec le backend
/// </summary>
public class ServiceRecetteFrontend : IServiceRecetteFrontend
{
    private readonly IServiceApiHttp _httpApiService;
    private const string BaseUrl = "recipes";

    public ServiceRecetteFrontend(IServiceApiHttp httpApiService)
    {
        _httpApiService = httpApiService;
    }

    public async Task<List<RecetteDTO>> GetRandomRecipesAsync()
    {
        try
        {
            var recipes = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/random");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<RecetteDTO>> SearchRecipesAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            var recipes = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/search?query={Uri.EscapeDataString(query)}");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<RecetteDTO?> GetRecipeDetailsAsync(string mealId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mealId))
                return null;

            return await _httpApiService.GetAsync<RecetteDTO>($"{BaseUrl}/{mealId}");
        }
        catch (Exception ex)
        {
            // Logged in development
            return null;
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            var categories = await _httpApiService.GetAsync<List<string>>($"{BaseUrl}/categories/list");
            return categories ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<string>> GetAreasAsync()
    {
        try
        {
            var areas = await _httpApiService.GetAsync<List<string>>($"{BaseUrl}/areas/list");
            return areas ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<RecetteDTO>> FilterByCategoryAsync(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return new();

            var recipes = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/filter/category?category={Uri.EscapeDataString(category)}");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<RecetteDTO>> FilterByAreaAsync(string area)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(area))
                return new();

            var recipes = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/filter/area?area={Uri.EscapeDataString(area)}");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }
}
