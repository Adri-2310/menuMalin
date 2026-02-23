using menuMalin.Server.Models.Entities;
using menuMalin.Server.Repositories;
using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour les favoris
/// </summary>
public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IRecipeService _recipeService;

    public FavoriteService(IFavoriteRepository favoriteRepository, IRecipeService recipeService)
    {
        _favoriteRepository = favoriteRepository;
        _recipeService = recipeService;
    }

    public async Task<RecipeDto> AddFavoriteAsync(string userId, string recipeId)
    {
        // Créer un nouveau favori
        var favorite = new Favorite
        {
            FavoriteId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            RecipeId = recipeId,
            CreatedAt = DateTime.UtcNow
        };

        await _favoriteRepository.AddAsync(favorite);

        // Retourner la recette
        var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
        return recipe ?? throw new InvalidOperationException($"Recette {recipeId} non trouvée");
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, string recipeId)
    {
        return await _favoriteRepository.RemoveByUserAndRecipeAsync(userId, recipeId);
    }

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
        }

        return recipes;
    }

    public async Task<bool> IsFavoriteAsync(string userId, string recipeId)
    {
        return await _favoriteRepository.IsFavoriteAsync(userId, recipeId);
    }
}
