using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Depots.Interfaces;

/// <summary>
/// Interface pour la couche d'accès aux recettes utilisateur
/// </summary>
public interface IDepotRecetteUtilisateur
{
    /// <summary>
    /// Ajoute une nouvelle recette utilisateur
    /// </summary>
    Task<RecetteUtilisateur> AddAsync(RecetteUtilisateur userRecipe);

    /// <summary>
    /// Récupère une recette par son ID
    /// </summary>
    Task<RecetteUtilisateur?> GetByIdAsync(string userRecipeId);

    /// <summary>
    /// Récupère toutes les recettes créées par un utilisateur
    /// </summary>
    Task<IEnumerable<RecetteUtilisateur>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Récupère toutes les recettes publiques
    /// </summary>
    Task<IEnumerable<RecetteUtilisateur>> GetPublicAsync();

    /// <summary>
    /// Met à jour une recette existante
    /// </summary>
    Task<RecetteUtilisateur?> UpdateAsync(RecetteUtilisateur userRecipe);

    /// <summary>
    /// Supprime une recette par son ID
    /// </summary>
    Task<bool> DeleteAsync(string userRecipeId);

    /// <summary>
    /// Met à jour la visibilité (public/privé) d'une recette
    /// </summary>
    Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic);
}
