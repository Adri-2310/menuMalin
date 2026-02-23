using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Data;
using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Implémentation du repository pour les recettes
/// </summary>
public class RecipeRepository : IRecipeRepository
{
    private readonly ApplicationDbContext _context;

    public RecipeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe?> GetByIdAsync(string recipeId)
    {
        return await _context.Recipes.FirstOrDefaultAsync(r => r.RecipeId == recipeId);
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        return await _context.Recipes.ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByCategoryAsync(string category)
    {
        return await _context.Recipes
            .Where(r => r.Category == category)
            .ToListAsync();
    }

    public async Task<Recipe> AddAsync(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recipe> UpdateAsync(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<bool> DeleteAsync(string recipeId)
    {
        var recipe = await GetByIdAsync(recipeId);
        if (recipe == null)
            return false;

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByMealDbIdAsync(string mealDbId)
    {
        return await _context.Recipes
            .AnyAsync(r => r.MealDBId == mealDbId);
    }

    public async Task<Recipe?> GetByMealDbIdAsync(string mealDbId)
    {
        return await _context.Recipes
            .FirstOrDefaultAsync(r => r.MealDBId == mealDbId);
    }
}
