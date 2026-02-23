using menuMalin.Models;
namespace menuMalin.Services;

public interface IRecipeService
{
    Task<List<Recipe>> GetRandomRecipesAsync(int count = 6);
    Task<List<Recipe>> SearchRecipesAsync(string searchTerm);
    Task<Recipe?> GetRecipeByIdAsync(string id);
    Task<List<string>> GetCategoriesAsync();
    Task<List<string>> GetAreasAsync();
    Task<List<Recipe>> SearchByCategoryAsync(string category);
    Task<List<Recipe>> SearchByAreaAsync(string area);
}