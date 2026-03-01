using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Depots.Interfaces;

/// <summary>
/// Interface pour la couche d'accès aux recettes
/// </summary>
public interface IDepotRecette
{
    /// <summary>
    /// Récupère une recette par son ID
    /// </summary>
    Task<Recette?> GetByIdAsync(string recipeId);

    /// <summary>
    /// Récupère toutes les recettes
    /// </summary>
    Task<IEnumerable<Recette>> GetAllAsync();

    /// <summary>
    /// Récupère les recettes par catégorie
    /// </summary>
    Task<IEnumerable<Recette>> GetByCategoryAsync(string category);

    /// <summary>
    /// Ajoute une nouvelle recette
    /// </summary>
    Task<Recette> AddAsync(Recette recipe);

    /// <summary>
    /// Met à jour une recette
    /// </summary>
    Task<Recette> UpdateAsync(Recette recipe);

    /// <summary>
    /// Supprime une recette
    /// </summary>
    Task<bool> DeleteAsync(string recipeId);

    /// <summary>
    /// Vérifie si une recette existe par MealDBId
    /// </summary>
    Task<bool> ExistsByMealDbIdAsync(string mealDbId);

    /// <summary>
    /// Récupère une recette par MealDBId
    /// </summary>
    Task<Recette?> GetByMealDbIdAsync(string mealDbId);
}
