using menuMalin.Modeles;

namespace menuMalin.Services;

public interface IServiceFavorisFrontend
{
    Task<List<Recette>> GetUserFavoritesAsync();
    Task<bool> AddFavoriteAsync(string recipeId);
    Task<bool> RemoveFavoriteAsync(string recipeId);
    Task<bool> IsFavoriteAsync(string recipeId);
}
