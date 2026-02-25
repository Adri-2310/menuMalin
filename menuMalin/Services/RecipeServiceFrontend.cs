using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Services;

/// <summary>
/// Service pour les recettes (Frontend) - communique avec le backend
/// </summary>
public class RecipeServiceFrontend : IRecipeServiceFrontend
{
    private readonly IHttpApiService _httpApiService;
    private const string BaseUrl = "recipes";

    public RecipeServiceFrontend(IHttpApiService httpApiService)
    {
        _httpApiService = httpApiService;
    }

    public async Task<List<RecipeDto>> GetRandomRecipesAsync()
    {
        try
        {
            var recipes = await _httpApiService.GetAsync<List<RecipeDto>>($"{BaseUrl}/random");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<RecipeDto>> SearchRecipesAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            var recipes = await _httpApiService.GetAsync<List<RecipeDto>>($"{BaseUrl}/search?query={Uri.EscapeDataString(query)}");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<RecipeDto?> GetRecipeDetailsAsync(string mealId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mealId))
                return null;

            return await _httpApiService.GetAsync<RecipeDto>($"{BaseUrl}/{mealId}");
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

    public async Task<List<RecipeDto>> FilterByCategoryAsync(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return new();

            var recipes = await _httpApiService.GetAsync<List<RecipeDto>>($"{BaseUrl}/filter/category?category={Uri.EscapeDataString(category)}");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<RecipeDto>> FilterByAreaAsync(string area)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(area))
                return new();

            var recipes = await _httpApiService.GetAsync<List<RecipeDto>>($"{BaseUrl}/filter/area?area={Uri.EscapeDataString(area)}");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }
}
