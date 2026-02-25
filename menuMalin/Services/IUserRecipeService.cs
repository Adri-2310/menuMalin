using menuMalin.Shared.Models.Dtos;
using menuMalin.Shared.Models.Requests;

namespace menuMalin.Services;

/// <summary>
/// Interface pour le service des recettes utilisateur (Frontend)
/// </summary>
public interface IUserRecipeService
{
    /// <summary>
    /// Crée une nouvelle recette utilisateur
    /// </summary>
    Task<UserRecipeDto?> CreateAsync(CreateUserRecipeRequest request);

    /// <summary>
    /// Récupère toutes les recettes créées par l'utilisateur
    /// </summary>
    Task<List<UserRecipeDto>> GetMyRecipesAsync();

    /// <summary>
    /// Récupère toutes les recettes publiques
    /// </summary>
    Task<List<UserRecipeDto>> GetPublicRecipesAsync();

    /// <summary>
    /// Récupère les détails d'une recette
    /// </summary>
    Task<UserRecipeDto?> GetByIdAsync(string userRecipeId);

    /// <summary>
    /// Met à jour une recette
    /// </summary>
    Task<UserRecipeDto?> UpdateAsync(string userRecipeId, CreateUserRecipeRequest request);

    /// <summary>
    /// Supprime une recette
    /// </summary>
    Task<bool> DeleteAsync(string userRecipeId);

    /// <summary>
    /// Bascule la visibilité (public/privé) d'une recette
    /// </summary>
    Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic);
}
