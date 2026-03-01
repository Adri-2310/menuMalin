namespace menuMalin.Services;

using System.Net.Http.Json;
using menuMalin.DTOs;
using menuMalin.Modeles;
using System.Reflection;

public class ServiceRecette : IServiceRecette
{
    private readonly HttpClient _http;

    public ServiceRecette(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Recette>> GetRandomRecipesAsync(int count = 6)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/random
            var response = await _http.GetFromJsonAsync<ReponseRecette>("recipes/random");
            return response?.Meals ?? new List<Recette>();
        }
        catch
        {
            return new List<Recette>();
        }
    }

    public async Task<List<Recette>> SearchRecipesAsync(string searchTerm)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/search?query={searchTerm}
            var response = await _http.GetFromJsonAsync<ReponseRecette>($"recipes/search?query={Uri.EscapeDataString(searchTerm)}");
            return response?.Meals ?? new List<Recette>();
        }
        catch
        {
            return new List<Recette>();
        }
    }


    public async Task<Recette?> GetRecipeByIdAsync(string id)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/{id}
            var response = await _http.GetFromJsonAsync<ReponseRecette>($"recipes/{id}");
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

    public async Task<List<Recette>> SearchByCategoryAsync(string category)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/filter/category?category={category}
            var response = await _http.GetFromJsonAsync<ReponseRecette>($"recipes/filter/category?category={Uri.EscapeDataString(category)}");
            return response?.Meals ?? new List<Recette>();
        }
        catch
        {
            return new List<Recette>();
        }
    }

    public async Task<List<Recette>> SearchByAreaAsync(string area)
    {
        try
        {
            // Appelle le backend proxy: GET /api/recipes/filter/area?area={area}
            var response = await _http.GetFromJsonAsync<ReponseRecette>($"recipes/filter/area?area={Uri.EscapeDataString(area)}");
            return response?.Meals ?? new List<Recette>();
        }
        catch
        {
            return new List<Recette>();
        }
    }
}