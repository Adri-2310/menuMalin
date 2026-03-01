using menuMalin.Shared.Modeles.DTOs;

namespace menuMalin.Server.Services.Interfaces;

/// <summary>
/// Interface pour le service TheMealDB
/// </summary>
public interface IServiceMealDB
{
    /// <summary>
    /// Récupère une recette aléatoire
    /// </summary>
    Task<RecetteMealDTO?> GetRandomAsync();

    /// <summary>
    /// Recherche des recettes par nom
    /// </summary>
    Task<List<RecetteMealDTO>> SearchByNameAsync(string query);

    /// <summary>
    /// Récupère les détails d'une recette par ID
    /// </summary>
    Task<RecetteMealDTO?> GetByIdAsync(string mealId);

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
    Task<List<RecetteMealDTO>> FilterByCategoryAsync(string category);

    /// <summary>
    /// Filtre les recettes par zone/cuisine
    /// </summary>
    Task<List<RecetteMealDTO>> FilterByAreaAsync(string area);
}
