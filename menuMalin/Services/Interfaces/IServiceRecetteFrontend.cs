using menuMalin.DTOs;

namespace menuMalin.Services;

public interface IServiceRecetteFrontend
{
    Task<List<ReponseRecette>> GetRandomRecipesAsync();
    Task<List<ReponseRecette>> SearchRecipesAsync(string query);
    Task<ReponseRecette?> GetRecipeDetailsAsync(string mealId);
    Task<List<string>> GetCategoriesAsync();
    Task<List<string>> GetAreasAsync();
    Task<List<ReponseRecette>> FilterByCategoryAsync(string category);
    Task<List<ReponseRecette>> FilterByAreaAsync(string area);
}
