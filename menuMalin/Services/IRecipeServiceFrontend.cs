using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Services;

/// <summary>
/// Interface pour le service des recettes (Frontend)
/// </summary>
public interface IRecipeServiceFrontend
{
    /// <summary>
    /// Récupère 6 recettes aléatoires
    /// </summary>
    Task<List<RecipeDto>> GetRandomRecipesAsync();

    /// <summary>
    /// Recherche des recettes par nom
    /// </summary>
    Task<List<RecipeDto>> SearchRecipesAsync(string query);

    /// <summary>
    /// Récupère les détails d'une recette
    /// </summary>
    Task<RecipeDto?> GetRecipeDetailsAsync(string mealId);

    /// <summary>
    /// Récupère toutes les catégories
    /// </summary>
    Task<List<string>> GetCategoriesAsync();

    /// <summary>
    /// Récupère toutes les zones/cuisines
    /// </summary>
    Task<List<string>> GetAreasAsync();

    /// <summary>
    /// Filtre les recettes par catégorie
    /// </summary>
    Task<List<RecipeDto>> FilterByCategoryAsync(string category);

    /// <summary>
    /// Filtre les recettes par zone/cuisine
    /// </summary>
    Task<List<RecipeDto>> FilterByAreaAsync(string area);
}
