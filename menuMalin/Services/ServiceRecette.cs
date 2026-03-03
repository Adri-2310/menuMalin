namespace menuMalin.Services;

using System.Net.Http.Json;
using menuMalin.DTOs;
using menuMalin.Modeles;
using menuMalin.Shared.Modeles.DTOs;
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
            // Appelle le backend proxy: GET /api/recipes/random?count={count}
            // Le backend retourne directement un tableau JSON d'objets {IdMeal, StrMeal, StrMealThumb, ...}
            var recipes = await _http.GetFromJsonAsync<List<Recette>>($"recipes/random?count={count}");
            return recipes ?? new List<Recette>();
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
            // Le backend retourne directement un tableau JSON d'objets {IdMeal, StrMeal, StrMealThumb, ...}
            var recipes = await _http.GetFromJsonAsync<List<Recette>>($"recipes/search?query={Uri.EscapeDataString(searchTerm)}");
            return recipes ?? new List<Recette>();
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
            // Le backend retourne un RecetteDTO unique (pas une liste)
            var dto = await _http.GetFromJsonAsync<RecetteDTO>($"recipes/{id}");
            return dto?.ToLocalRecette();
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
            // Le backend retourne une liste de RecetteDTO (recipeId, title, imageUrl, ...)
            var dtos = await _http.GetFromJsonAsync<List<RecetteDTO>>($"recipes/filter/category?category={Uri.EscapeDataString(category)}");
            return dtos.ToLocalRecettes();
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
            // Le backend retourne une liste de RecetteDTO (recipeId, title, imageUrl, ...)
            var dtos = await _http.GetFromJsonAsync<List<RecetteDTO>>($"recipes/filter/area?area={Uri.EscapeDataString(area)}");
            return dtos.ToLocalRecettes();
        }
        catch
        {
            return new List<Recette>();
        }
    }
}