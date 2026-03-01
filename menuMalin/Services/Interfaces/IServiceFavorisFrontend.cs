using menuMalin.DTOs;

namespace menuMalin.Services;

public interface IServiceFavorisFrontend
{
    Task<List<ReponseRecette>> GetUserFavoritesAsync();
    Task<bool> AddFavoriteAsync(string recipeId);
    Task<bool> RemoveFavoriteAsync(string recipeId);
    Task<bool> IsFavoriteAsync(string recipeId);
}
