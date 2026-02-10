using menuMalin.Models;
namespace menuMalin.Services;

public interface IRecipeService
{
    Task<List<Recipe>> GetRandomRecipesAsync(int count = 6);
    Task<List<Recipe>> SearchRecipesAsync(string searchTerm);
    Task<Recipe?> GetRecipeByIdAsync(string id);
}