using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Interface pour la couche d'accès aux recettes utilisateur
/// </summary>
public interface IUserRecipeRepository
{
    /// <summary>
    /// Ajoute une nouvelle recette utilisateur
    /// </summary>
    Task<UserRecipe> AddAsync(UserRecipe userRecipe);

    /// <summary>
    /// Récupère une recette par son ID
    /// </summary>
    Task<UserRecipe?> GetByIdAsync(string userRecipeId);

    /// <summary>
    /// Récupère toutes les recettes créées par un utilisateur
    /// </summary>
    Task<IEnumerable<UserRecipe>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Récupère toutes les recettes publiques
    /// </summary>
    Task<IEnumerable<UserRecipe>> GetPublicAsync();

    /// <summary>
    /// Met à jour une recette existante
    /// </summary>
    Task<UserRecipe?> UpdateAsync(UserRecipe userRecipe);

    /// <summary>
    /// Supprime une recette par son ID
    /// </summary>
    Task<bool> DeleteAsync(string userRecipeId);

    /// <summary>
    /// Met à jour la visibilité (public/privé) d'une recette
    /// </summary>
    Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic);
}
