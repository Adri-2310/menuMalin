using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services.Interfaces;

// DTOs importés depuis menuMalin.Shared

/// <summary>
/// Interface pour le service des recettes
/// </summary>
public interface IServiceRecette
{
    /// <summary>
    /// Récupère une recette par son ID
    /// </summary>
    Task<RecipeDto?> GetRecipeByIdAsync(string recipeId);

    /// <summary>
    /// Récupère toutes les recettes
    /// </summary>
    Task<IEnumerable<RecipeDto>> GetAllRecipesAsync();

    /// <summary>
    /// Récupère les recettes par catégorie
    /// </summary>
    Task<IEnumerable<RecipeDto>> GetRecipesByCategoryAsync(string category);

    /// <summary>
    /// Crée ou met en cache une recette depuis TheMealDB
    /// </summary>
    Task<RecipeDto> CreateOrUpdateRecipeAsync(MealDto mealDto);

    /// <summary>
    /// Vérifie si une recette existe en cache
    /// </summary>
    Task<bool> RecipeExistsAsync(string mealDbId);
}
