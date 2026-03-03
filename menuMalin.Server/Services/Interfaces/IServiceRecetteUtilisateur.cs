using menuMalin.Shared.Modeles.DTOs;
using menuMalin.Shared.Modeles.Requetes;

namespace menuMalin.Server.Services.Interfaces;

/// <summary>
/// Interface pour le service des recettes utilisateur
/// </summary>
public interface IServiceRecetteUtilisateur
{
    /// <summary>
    /// Crée une nouvelle recette utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur qui crée la recette</param>
    /// <param name="request">Les données de la recette à créer</param>
    /// <returns>Le RecetteUtilisateurDTO créé</returns>
    Task<RecetteUtilisateurDTO> CreateAsync(string userId, RequeteCreationRecetteUtilisateur request);

    /// <summary>
    /// Récupère toutes les recettes créées par un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <returns>Une collection de RecetteUtilisateurDTO créés par cet utilisateur</returns>
    Task<IEnumerable<RecetteUtilisateurDTO>> GetMyRecipesAsync(string userId);

    /// <summary>
    /// Récupère toutes les recettes publiques
    /// </summary>
    /// <returns>Une collection de RecetteUtilisateurDTO marquées comme publiques</returns>
    Task<IEnumerable<RecetteUtilisateurDTO>> GetPublicRecipesAsync();

    /// <summary>
    /// Récupère les détails d'une recette par son ID
    /// </summary>
    /// <param name="userRecipeId">L'ID de la recette</param>
    /// <returns>Le RecetteUtilisateurDTO ou null si non trouvée</returns>
    Task<RecetteUtilisateurDTO?> GetByIdAsync(string userRecipeId);

    /// <summary>
    /// Supprime une recette (seul le propriétaire peut la supprimer)
    /// </summary>
    /// <param name="userRecipeId">L'ID de la recette à supprimer</param>
    /// <param name="userId">L'ID de l'utilisateur qui demande la suppression</param>
    /// <returns>true si la suppression a réussi, false sinon</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si l'utilisateur n'est pas propriétaire</exception>
    Task<bool> DeleteAsync(string userRecipeId, string userId);

    /// <summary>
    /// Met à jour tous les champs modifiables d'une recette
    /// </summary>
    /// <param name="userRecipeId">L'ID de la recette</param>
    /// <param name="userId">L'ID de l'utilisateur qui demande le changement</param>
    /// <param name="request">Les nouvelles données de la recette</param>
    /// <returns>Le RecetteUtilisateurDTO mis à jour ou null si non trouvée</returns>
    /// <exception cref="UnauthorizedAccessException">Levée si l'utilisateur n'est pas propriétaire</exception>
    Task<RecetteUtilisateurDTO?> UpdateAsync(string userRecipeId, string userId, RequeteCreationRecetteUtilisateur request);

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
