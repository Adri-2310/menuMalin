using menuMalin.Modeles;

namespace menuMalin.Services;

public interface IServiceRecette
{
    Task<List<Recette>> GetRandomRecipesAsync(int count = 6);
    Task<List<Recette>> SearchRecipesAsync(string searchTerm);
    Task<Recette?> GetRecipeByIdAsync(string id);
    Task<List<string>> GetCategoriesAsync();
    Task<List<string>> GetAreasAsync();
    Task<List<Recette>> SearchByCategoryAsync(string category);
    Task<List<Recette>> SearchByAreaAsync(string area);
}
