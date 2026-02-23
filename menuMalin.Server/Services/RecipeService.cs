using menuMalin.Server.Models.Entities;
using menuMalin.Server.Repositories;
using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour les recettes
/// </summary>
public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeDto?> GetRecipeByIdAsync(string recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        return recipe == null ? null : MapToDto(recipe);
    }

    public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        return recipes.Select(MapToDto);
    }

    public async Task<IEnumerable<RecipeDto>> GetRecipesByCategoryAsync(string category)
    {
        var recipes = await _recipeRepository.GetByCategoryAsync(category);
        return recipes.Select(MapToDto);
    }

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

    public async Task<bool> RecipeExistsAsync(string mealDbId)
    {
        return await _recipeRepository.ExistsByMealDbIdAsync(mealDbId);
    }

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
