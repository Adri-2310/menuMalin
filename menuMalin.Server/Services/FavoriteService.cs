using menuMalin.Server.Models.Entities;
using menuMalin.Server.Repositories;
using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour les favoris
/// </summary>
/// <summary>
/// Service pour la gestion des favoris utilisateur
/// </summary>
public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IRecipeService _recipeService;
    private readonly IRecipeRepository _recipeRepository;

    /// <summary>
    /// Initialise une nouvelle instance de FavoriteService
    /// </summary>
    /// <param name="favoriteRepository">Le repository des favoris</param>
    /// <param name="recipeService">Le service des recettes</param>
    /// <param name="recipeRepository">Le repository des recettes</param>
    public FavoriteService(IFavoriteRepository favoriteRepository, IRecipeService recipeService, IRecipeRepository recipeRepository)
    {
        _favoriteRepository = favoriteRepository;
        _recipeService = recipeService;
        _recipeRepository = recipeRepository;
    }

    /// <summary>
    /// Ajoute une recette aux favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <param name="recipeId">L'ID de la recette à ajouter</param>
    /// <returns>Le RecipeDto ajouté aux favoris</returns>
    /// <exception cref="InvalidOperationException">Levée si la recette n'existe pas</exception>
    public async Task<RecipeDto> AddFavoriteAsync(string userId, string recipeId)
    {
        // Vérifier si la recette existe déjà
        var recipeExists = await _recipeRepository.GetByIdAsync(recipeId);

        if (recipeExists == null)
        {
            // Créer une recette placeholder pour les recettes externes (TheMealDB, etc.)
            var placeholderRecipe = new Recipe
            {
                RecipeId = recipeId,
                MealDBId = recipeId,  // Stocker le MealDBId comme identifiant externe
                Title = string.Empty,
                Instructions = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _recipeRepository.AddAsync(placeholderRecipe);
            recipeExists = placeholderRecipe;
        }

        // Créer un nouveau favori
        var favorite = new Favorite
        {
            FavoriteId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            RecipeId = recipeId,
            CreatedAt = DateTime.UtcNow
        };

        await _favoriteRepository.AddAsync(favorite);

        // Retourner la recette (placeholder ou réelle)
        return new RecipeDto
        {
            RecipeId = recipeExists.RecipeId,
            Title = recipeExists.Title ?? string.Empty,
            MealDBId = recipeExists.MealDBId,
            Instructions = recipeExists.Instructions ?? string.Empty
        };
    }

    /// <summary>
    /// Supprime une recette des favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <param name="recipeId">L'ID de la recette à supprimer</param>
    /// <returns>true si la suppression a réussi, false sinon</returns>
    public async Task<bool> RemoveFavoriteAsync(string userId, string recipeId)
    {
        return await _favoriteRepository.RemoveByUserAndRecipeAsync(userId, recipeId);
    }

    /// <summary>
    /// Récupère tous les favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <returns>Une collection de RecipeDto correspondant aux favoris de l'utilisateur</returns>
    public async Task<IEnumerable<RecipeDto>> GetUserFavoritesAsync(string userId)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId);
        var recipes = new List<RecipeDto>();

        foreach (var favorite in favorites)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(favorite.RecipeId);
            if (recipe != null)
            {
                recipes.Add(recipe);
            }
            else
            {
                // Si la recette n'existe pas localement (ex: TheMealDB), créer un placeholder
                // La recette sera chargée depuis TheMealDB côté frontend
                recipes.Add(new RecipeDto { RecipeId = favorite.RecipeId });
            }
        }

        return recipes;
    }

    /// <summary>
    /// Vérifie si une recette est dans les favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <param name="recipeId">L'ID de la recette</param>
    /// <returns>true si la recette est dans les favoris, false sinon</returns>
    public async Task<bool> IsFavoriteAsync(string userId, string recipeId)
    {
        return await _favoriteRepository.IsFavoriteAsync(userId, recipeId);
    }
}
