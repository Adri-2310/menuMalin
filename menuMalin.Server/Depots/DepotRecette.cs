using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Donnees;
using menuMalin.Server.Modeles.Entites;
using menuMalin.Server.Depots.Interfaces;

namespace menuMalin.Server.Depots;

/// <summary>
/// Implémentation du repository pour les recettes
/// </summary>
public class DepotRecette : IDepotRecette
{
    private readonly ApplicationDbContext _context;

    public DepotRecette(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Recette?> GetByIdAsync(string recipeId)
    {
        return await _context.Recettes.FirstOrDefaultAsync(r => r.RecipeId == recipeId);
    }

    public async Task<IEnumerable<Recette>> GetAllAsync()
    {
        return await _context.Recettes.ToListAsync();
    }

    public async Task<IEnumerable<Recette>> GetByCategoryAsync(string category)
    {
        return await _context.Recettes
            .Where(r => r.Category == category)
            .ToListAsync();
    }

    public async Task<Recette> AddAsync(Recette recipe)
    {
        _context.Recettes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recette> UpdateAsync(Recette recipe)
    {
        _context.Recettes.Update(recipe);
        await _context.SaveChangesAsync();
        return recipe;
    }

    public async Task<bool> DeleteAsync(string recipeId)
    {
        var recipe = await GetByIdAsync(recipeId);
        if (recipe == null)
            return false;

        _context.Recettes.Remove(recipe);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByMealDbIdAsync(string mealDbId)
    {
        return await _context.Recettes
            .AnyAsync(r => r.MealDBId == mealDbId);
    }

    public async Task<Recette?> GetByMealDbIdAsync(string mealDbId)
    {
        return await _context.Recettes
            .FirstOrDefaultAsync(r => r.MealDBId == mealDbId);
    }
}
