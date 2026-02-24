using menuMalin.Server.Models.Entities;
using menuMalin.Server.Repositories;
using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour les recettes
/// </summary>
/// <summary>
/// Service pour la gestion des recettes
/// </summary>
public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    /// <summary>
    /// Initialise une nouvelle instance de RecipeService
    /// </summary>
    /// <param name="recipeRepository">Le repository des recettes</param>
    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    /// <summary>
    /// Récupère une recette par son ID
    /// </summary>
    /// <param name="recipeId">L'ID de la recette</param>
    /// <returns>La recette avec les détails mappés en DTO, ou null si non trouvée</returns>
    public async Task<RecipeDto?> GetRecipeByIdAsync(string recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        return recipe == null ? null : MapToDto(recipe);
    }

    /// <summary>
    /// Récupère toutes les recettes
    /// </summary>
    /// <returns>Une collection de tous les RecipeDto</returns>
    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        return recipes.Select(MapToDto);
    }

    /// <summary>
    /// Récupère toutes les recettes d'une catégorie
    /// </summary>
    /// <param name="category">Le nom de la catégorie</param>
    /// <returns>Une collection de RecipeDto pour la catégorie spécifiée</returns>
    public async Task<IEnumerable<RecipeDto>> GetRecipesByCategoryAsync(string category)
    {
        var recipes = await _recipeRepository.GetByCategoryAsync(category);
        return recipes.Select(MapToDto);
    }

    /// <summary>
    /// Crée une nouvelle recette ou met à jour une existante
    /// </summary>
    /// <param name="mealDto">Les données de la recette à créer ou mettre à jour</param>
    /// <returns>Le RecipeDto créé ou mis à jour</returns>
    public async Task<RecipeDto> CreateOrUpdateRecipeAsync(MealDto mealDto)
    {
        // Vérifier si la recette existe déjà
        var existingRecipe = await _recipeRepository.GetByMealDbIdAsync(mealDto.IdMeal);

        if (existingRecipe != null)
        {
            // Mettre à jour
            existingRecipe.Title = mealDto.StrMeal;
            existingRecipe.Instructions = mealDto.StrInstructions ?? string.Empty;
            existingRecipe.ImageUrl = mealDto.StrMealThumb;
            existingRecipe.Category = mealDto.StrCategory;
            existingRecipe.Area = mealDto.StrArea;
            existingRecipe.Tags = mealDto.StrTags;
            existingRecipe.UpdatedAt = DateTime.UtcNow;

            await _recipeRepository.UpdateAsync(existingRecipe);
            return MapToDto(existingRecipe);
        }

        // Créer une nouvelle recette
        var newRecipe = new Recipe
        {
            RecipeId = Guid.NewGuid().ToString("N"),
            MealDBId = mealDto.IdMeal,
            Title = mealDto.StrMeal,
            Instructions = mealDto.StrInstructions ?? string.Empty,
            ImageUrl = mealDto.StrMealThumb,
            Category = mealDto.StrCategory,
            Area = mealDto.StrArea,
            Tags = mealDto.StrTags,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _recipeRepository.AddAsync(newRecipe);
        return MapToDto(newRecipe);
    }

    /// <summary>
    /// Vérifie si une recette existe par son ID MealDB
    /// </summary>
    /// <param name="mealDbId">L'ID TheMealDB de la recette</param>
    /// <returns>true si la recette existe, false sinon</returns>
    public async Task<bool> RecipeExistsAsync(string mealDbId)
    {
        return await _recipeRepository.ExistsByMealDbIdAsync(mealDbId);
    }

    /// <summary>
    /// Mappe une entité Recipe en RecipeDto
    /// </summary>
    /// <param name="recipe">L'entité Recipe à mapper</param>
    /// <returns>Le RecipeDto mappé</returns>
    private static RecipeDto MapToDto(Recipe recipe)
    {
        return new RecipeDto
        {
            RecipeId = recipe.RecipeId,
            Title = recipe.Title,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            ImageUrl = recipe.ImageUrl,
            MealDBId = recipe.MealDBId,
            Category = recipe.Category,
            Area = recipe.Area,
            Tags = recipe.Tags,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt,
            Ingredients = new()
        };
    }
}
