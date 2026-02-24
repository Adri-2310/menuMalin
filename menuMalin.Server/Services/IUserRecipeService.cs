using menuMalin.Shared.Models.Dtos;
using menuMalin.Shared.Models.Requests;

namespace menuMalin.Server.Services;

/// <summary>
/// Interface pour le service des recettes utilisateur
/// </summary>
public interface IUserRecipeService
{
    /// <summary>
    /// Crée une nouvelle recette utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur qui crée la recette</param>
    /// <param name="request">Les données de la recette à créer</param>
    /// <returns>Le UserRecipeDto créé</returns>
    Task<UserRecipeDto> CreateAsync(string userId, CreateUserRecipeRequest request);

    /// <summary>
    /// Récupère toutes les recettes créées par un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <returns>Une collection de UserRecipeDto créés par cet utilisateur</returns>
    Task<IEnumerable<UserRecipeDto>> GetMyRecipesAsync(string userId);

    /// <summary>
    /// Récupère toutes les recettes publiques
    /// </summary>
    /// <returns>Une collection de UserRecipeDto marquées comme publiques</returns>
    Task<IEnumerable<UserRecipeDto>> GetPublicRecipesAsync();

    /// <summary>
    /// Récupère les détails d'une recette par son ID
    /// </summary>
    /// <param name="userRecipeId">L'ID de la recette</param>
    /// <returns>Le UserRecipeDto ou null si non trouvée</returns>
    Task<UserRecipeDto?> GetByIdAsync(string userRecipeId);

    /// <summary>
    /// Supprime une recette (seul le propriétaire peut la supprimer)
    /// </summary>
    /// <param name="userRecipeId">L'ID de la recette à supprimer</param>
    /// <param name="userId">L'ID de l'utilisateur qui demande la suppression</param>
    /// <returns>true si la suppression a réussi, false sinon</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si l'utilisateur n'est pas propriétaire</exception>
    Task<bool> DeleteAsync(string userRecipeId, string userId);

    /// <summary>
    /// Bascule la visibilité (public/privé) d'une recette
    /// </summary>
    /// <param name="userRecipeId">L'ID de la recette</param>
    /// <param name="userId">L'ID de l'utilisateur qui demande le changement</param>
    /// <param name="isPublic">La nouvelle valeur de visibilité</param>
    /// <returns>true si le changement a réussi, false sinon</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si l'utilisateur n'est pas propriétaire</exception>
    Task<bool> UpdateVisibilityAsync(string userRecipeId, string userId, bool isPublic);
}
