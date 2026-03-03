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
        // Le backend retourne directement un tableau [{IdMeal, StrMeal, StrMealThumb, ...}]
        var recipes = await _httpApiService.GetAsync<List<Recette>>($"{BaseUrl}/random");
        return recipes ?? new();
    }

    public async Task<List<Recette>> SearchRecipesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new();

        // Le backend retourne directement un tableau [{IdMeal, StrMeal, StrMealThumb, ...}]
        var recipes = await _httpApiService.GetAsync<List<Recette>>($"{BaseUrl}/search?query={Uri.EscapeDataString(query)}");
        return recipes ?? new();
    }

    public async Task<Recette?> GetRecipeDetailsAsync(string mealId)
    {
        if (string.IsNullOrWhiteSpace(mealId))
            return null;

        // Le backend retourne un RecetteDTO unique (recipeId, title, imageUrl, ...)
        var dto = await _httpApiService.GetAsync<RecetteDTO>($"{BaseUrl}/{mealId}");
        return dto?.ToLocalRecette();
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        var categories = await _httpApiService.GetAsync<List<string>>($"{BaseUrl}/categories/list");
        return categories ?? new();
    }

    public async Task<List<string>> GetAreasAsync()
    {
        var areas = await _httpApiService.GetAsync<List<string>>($"{BaseUrl}/areas/list");
        return areas ?? new();
    }

    public async Task<List<Recette>> FilterByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return new();

        // Le backend retourne une liste de RecetteDTO (recipeId, title, imageUrl, ...)
        var dtos = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/filter/category?category={Uri.EscapeDataString(category)}");
        return dtos.ToLocalRecettes();
    }

    public async Task<List<Recette>> FilterByAreaAsync(string area)
    {
        if (string.IsNullOrWhiteSpace(area))
            return new();

        // Le backend retourne une liste de RecetteDTO (recipeId, title, imageUrl, ...)
        var dtos = await _httpApiService.GetAsync<List<RecetteDTO>>($"{BaseUrl}/filter/area?area={Uri.EscapeDataString(area)}");
        return dtos.ToLocalRecettes();
    }
}
