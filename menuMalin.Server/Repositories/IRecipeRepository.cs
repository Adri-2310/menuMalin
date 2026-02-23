using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Interface pour la couche d'accès aux recettes
/// </summary>
public interface IRecipeRepository
{
    /// <summary>
    /// Récupère une recette par son ID
    /// </summary>
    Task<Recipe?> GetByIdAsync(string recipeId);

    /// <summary>
    /// Récupère toutes les recettes
    /// </summary>
    Task<IEnumerable<Recipe>> GetAllAsync();

    /// <summary>
    /// Récupère les recettes par catégorie
    /// </summary>
    Task<IEnumerable<Recipe>> GetByCategoryAsync(string category);

    /// <summary>
    /// Ajoute une nouvelle recette
    /// </summary>
    Task<Recipe> AddAsync(Recipe recipe);

    /// <summary>
    /// Met à jour une recette
    /// </summary>
    Task<Recipe> UpdateAsync(Recipe recipe);

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
    Task<Recipe?> GetByMealDbIdAsync(string mealDbId);
}
