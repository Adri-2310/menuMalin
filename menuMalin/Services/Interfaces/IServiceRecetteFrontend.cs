using menuMalin.DTOs;
using menuMalin.Shared.Modeles.DTOs;

namespace menuMalin.Services;

public interface IServiceRecetteFrontend
{
    Task<List<RecetteDTO>> GetRandomRecipesAsync();
    Task<List<RecetteDTO>> SearchRecipesAsync(string query);
    Task<RecetteDTO?> GetRecipeDetailsAsync(string mealId);
    Task<List<string>> GetCategoriesAsync();
    Task<List<string>> GetAreasAsync();
    Task<List<RecetteDTO>> FilterByCategoryAsync(string category);
    Task<List<RecetteDTO>> FilterByAreaAsync(string area);
}
