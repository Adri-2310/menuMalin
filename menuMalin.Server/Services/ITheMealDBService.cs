using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services;

/// <summary>
/// Interface pour le service TheMealDB
/// </summary>
public interface ITheMealDBService
{
    /// <summary>
    /// Récupère une recette aléatoire
    /// </summary>
    Task<MealDto?> GetRandomAsync();

    /// <summary>
    /// Recherche des recettes par nom
    /// </summary>
    Task<List<MealDto>> SearchByNameAsync(string query);

    /// <summary>
    /// Récupère les détails d'une recette par ID
    /// </summary>
    Task<MealDto?> GetByIdAsync(string mealId);

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
    Task<List<MealDto>> FilterByCategoryAsync(string category);

    /// <summary>
    /// Filtre les recettes par zone/cuisine
    /// </summary>
    Task<List<MealDto>> FilterByAreaAsync(string area);
}
