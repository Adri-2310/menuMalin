using menuMalin.Models;

namespace menuMalin.Services;

public interface IFavoriteService
{
    Task AddFavoriteAsync(Recipe recipe);
    Task RemoveFavoriteAsync(string recipeId);
    Task<bool> IsFavoriteAsync(string recipeId);
    Task<List<Recipe>> GetFavoritesAsync();
}