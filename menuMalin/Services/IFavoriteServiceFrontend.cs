using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Services;

/// <summary>
/// Interface pour le service des favoris (Frontend)
/// </summary>
public interface IFavoriteServiceFrontend
{
    /// <summary>
    /// Récupère tous les favoris de l'utilisateur
    /// </summary>
    Task<List<RecipeDto>> GetUserFavoritesAsync();

    /// <summary>
    /// Ajoute un favori
    /// </summary>
    Task<bool> AddFavoriteAsync(string recipeId);

    /// <summary>
    /// Supprime un favori
    /// </summary>
    Task<bool> RemoveFavoriteAsync(string recipeId);

    /// <summary>
    /// Vérifie si une recette est favorite
    /// </summary>
    Task<bool> IsFavoriteAsync(string recipeId);
}
