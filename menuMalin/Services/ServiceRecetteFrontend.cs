using menuMalin.Modeles;
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

    public async Task<List<Recette>> GetRandomRecipesAsync()
    {
        try
        {
            // Le backend retourne directement un tableau [{IdMeal, StrMeal, StrMealThumb, ...}]
            var recipes = await _httpApiService.GetAsync<List<Recette>>($"{BaseUrl}/random");
            return recipes ?? new();
        }
        catch
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<Recette>> SearchRecipesAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return new();

            // Le backend retourne directement un tableau [{IdMeal, StrMeal, StrMealThumb, ...}]
            var recipes = await _httpApiService.GetAsync<List<Recette>>($"{BaseUrl}/search?query={Uri.EscapeDataString(query)}");
            return recipes ?? new();
        }
        catch
        {
            // Logged in development
            return new();
        }
    }

    public async Task<Recette?> GetRecipeDetailsAsync(string mealId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mealId))
                return null;

            // Le backend retourne un RecetteDTO unique (recipeId, title, imageUrl, ...)
            var dto = await _httpApiService.GetAsync<RecetteDTO>($"{BaseUrl}/{mealId}");
            return dto?.ToLocalRecette();
        }
        catch
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
        catch
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
        catch
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<Recette>> FilterByCategoryAsync(string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return new();

            // Le backend retourne une liste de RecetteDTO (recipeId, title, imageUrl, ...)
            var dtos = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/filter/category?category={Uri.EscapeDataString(category)}");
            return dtos.ToLocalRecettes();
        }
        catch
        {
            // Logged in development
            return new();
        }
    }

    public async Task<List<Recette>> FilterByAreaAsync(string area)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(area))
                return new();

            // Le backend retourne une liste de RecetteDTO (recipeId, title, imageUrl, ...)
            var dtos = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/filter/area?area={Uri.EscapeDataString(area)}");
            return dtos.ToLocalRecettes();
        }
        catch
        {
            // Logged in development
            return new();
        }
    }
}
