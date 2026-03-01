using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services.Interfaces;

/// <summary>
/// Interface pour le service des favoris
/// </summary>
public interface IServiceFavoris
{
    /// <summary>
    /// Ajoute un favori pour un utilisateur
    /// </summary>
    Task<RecipeDto> AddFavoriteAsync(string userId, string recipeId);

    /// <summary>
    /// Supprime un favori
    /// </summary>
    Task<bool> RemoveFavoriteAsync(string userId, string recipeId);

    /// <summary>
    /// Récupère tous les favoris d'un utilisateur
    /// </summary>
    Task<IEnumerable<RecipeDto>> GetUserFavorisAsync(string userId);

    /// <summary>
    /// Vérifie si une recette est favorite pour un utilisateur
    /// </summary>
    Task<bool> IsFavoriteAsync(string userId, string recipeId);
}
