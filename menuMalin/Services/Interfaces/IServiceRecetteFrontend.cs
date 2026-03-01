using menuMalin.Modeles;

namespace menuMalin.Services;

public interface IServiceRecetteFrontend
{
    Task<List<Recette>> GetRandomRecipesAsync();
    Task<List<Recette>> SearchRecipesAsync(string query);
    Task<Recette?> GetRecipeDetailsAsync(string mealId);
    Task<List<string>> GetCategoriesAsync();
    Task<List<string>> GetAreasAsync();
    Task<List<Recette>> FilterByCategoryAsync(string category);
    Task<List<Recette>> FilterByAreaAsync(string area);
}
