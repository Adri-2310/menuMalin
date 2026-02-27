namespace menuMalin.Services;

using System.Net.Http.Json;
using menuMalin.DTOs;
using menuMalin.Models;
using System.Reflection;

public class RecipeService : IRecipeService
{
    private readonly HttpClient _http;

    public RecipeService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Recipe>> GetRandomRecipesAsync(int count = 6)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/random
            var response = await _http.GetFromJsonAsync<RecipeResponse>("recipes/random");
            return response?.Meals ?? new List<Recipe>();
        }
        catch
        {
            return new List<Recipe>();
        }
    }

    public async Task<List<Recipe>> SearchRecipesAsync(string searchTerm)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/search?query={searchTerm}
            var response = await _http.GetFromJsonAsync<RecipeResponse>($"recipes/search?query={Uri.EscapeDataString(searchTerm)}");
            return response?.Meals ?? new List<Recipe>();
        }
        catch
        {
            return new List<Recipe>();
        }
    }
    

    public async Task<Recipe?> GetRecipeByIdAsync(string id)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/{id}
            var response = await _http.GetFromJsonAsync<RecipeResponse>($"recipes/{id}");
            return response?.Meals?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/categories/list
            var response = await _http.GetFromJsonAsync<List<string>>("recipes/categories/list");
            return response ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task<List<string>> GetAreasAsync()
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/areas/list
            var response = await _http.GetFromJsonAsync<List<string>>("recipes/areas/list");
            return response ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public async Task<List<Recipe>> SearchByCategoryAsync(string category)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/filter/category?category={category}
            var response = await _http.GetFromJsonAsync<RecipeResponse>($"recipes/filter/category?category={Uri.EscapeDataString(category)}");
            return response?.Meals ?? new List<Recipe>();
        }
        catch
        {
            return new List<Recipe>();
        }
    }

    public async Task<List<Recipe>> SearchByAreaAsync(string area)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/filter/area?area={area}
            var response = await _http.GetFromJsonAsync<RecipeResponse>($"recipes/filter/area?area={Uri.EscapeDataString(area)}");
            return response?.Meals ?? new List<Recipe>();
        }
        catch
        {
            return new List<Recipe>();
        }
    }
}