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
        var recipes = new List<Recipe>();
        // L'API ne permet d'en récupérer qu'une à la fois avec 'random.php'
        for (int i = 0; i < count; i++)
        {
            var response = await _http.GetFromJsonAsync<RecipeResponse>("random.php");
            if (response?.Meals != null && response.Meals.Any())
            {
                recipes.Add(response.Meals[0]);
            }
        }
        return recipes;
    }

    public async Task<List<Recipe>> SearchRecipesAsync(string searchTerm)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<RecipeResponse>($"search.php?s={searchTerm}");
            return response?.Meals ?? new List<Recipe>();
        }
        catch
        {
            return new List<Recipe>();
        }
        
    }
    

    public async Task<Recipe?> GetRecipeByIdAsync(string id)
    {
        var response = await _http.GetFromJsonAsync<RecipeResponse>($"lookup.php?i={id}");
        return response?.Meals?.FirstOrDefault();
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<CategoryResponse>("list.php?c=list");
            return response?.Meals?.Select(c => c.StrCategory).Where(c => !string.IsNullOrEmpty(c)).Cast<string>().ToList() ?? new List<string>();
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
            var response = await _http.GetFromJsonAsync<AreaResponse>("list.php?a=list");
            return response?.Meals?.Select(a => a.StrArea).Where(a => !string.IsNullOrEmpty(a)).Cast<string>().ToList() ?? new List<string>();
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
            var response = await _http.GetFromJsonAsync<RecipeResponse>($"filter.php?c={category}");
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
            var response = await _http.GetFromJsonAsync<RecipeResponse>($"filter.php?a={area}");
            return response?.Meals ?? new List<Recipe>();
        }
        catch
        {
            return new List<Recipe>();
        }
    }
}