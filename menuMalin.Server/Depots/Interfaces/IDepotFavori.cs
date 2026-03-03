using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Depots.Interfaces;

/// <summary>
/// Interface pour la couche d'accès aux favoris
/// </summary>
public interface IDepotFavori
{
    /// <summary>
    /// Ajoute un favori
    /// </summary>
    Task<Favori> AddAsync(Favori favorite);

    /// <summary>
    /// Supprime un favori par son ID
    /// </summary>
    Task<bool> RemoveAsync(string favoriteId);

    /// <summary>
    /// Récupère tous les favoris d'un utilisateur
    /// </summary>
    Task<IEnumerable<Favori>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Vérifie si une recette est favorite pour un utilisateur
    /// </summary>
    Task<bool> IsFavoriteAsync(string userId, string recipeId);

    /// <summary>
    /// Récupère un favori par userId et recipeId
    /// </summary>
    Task<Favori?> GetByUserAndRecipeAsync(string userId, string recipeId);

    /// <summary>
    /// Supprime un favori par userId et recipeId
    /// </summary>
    Task<bool> RemoveByUserAndRecipeAsync(string userId, string recipeId);
}
