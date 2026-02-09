namespace menuMalin.Services;

using System.Net.Http.Json;
using menuMalin.DTOs;
using menuMalin.Models;

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

    public async Task<List<Recipe>> SearchRecipesAsync(string term)
    {
        var response = await _http.GetFromJsonAsync<RecipeResponse>($"search.php?s={term}");
        return response?.Meals ?? new List<Recipe>();
    }

    public async Task<Recipe?> GetRecipeByIdAsync(string id)
    {
        var response = await _http.GetFromJsonAsync<RecipeResponse>($"lookup.php?i={id}");
        return response?.Meals?.FirstOrDefault();
    }
}