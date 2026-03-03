using menuMalin.Server.Depots.Interfaces;
using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Donnees;
using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Depots;

/// <summary>
/// Implémentation du repository pour les favoris
/// </summary>
public class DepotFavori : IDepotFavori
{
    private readonly ApplicationDbContext _context;

    public DepotFavori(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Favori> AddAsync(Favori favorite)
    {
        _context.Favoris.Add(favorite);
        await _context.SaveChangesAsync();
        return favorite;
    }

    public async Task<bool> RemoveAsync(string favoriteId)
    {
        var favorite = await _context.Favoris
            .FirstOrDefaultAsync(f => f.FavoriteId == favoriteId);

        if (favorite == null)
            return false;

        _context.Favoris.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Favori>> GetByUserIdAsync(string userId)
    {
        return await _context.Favoris
            .Where(f => f.UserId == userId)
            .Include(f => f.Recette)
            .ToListAsync();
    }

    public async Task<bool> IsFavoriteAsync(string userId, string recipeId)
    {
        // D'abord chercher par RecipeId interne (pour les appels du backend)
        var isFavorite = await _context.Favoris
            .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);

        if (isFavorite)
            return true;

        // Si pas trouvé, chercher par MealDBId (pour les appels du frontend avec TheMealDB ID)
        // En supposant que recipeId pourrait être un TheMealDB ID
        var recipeByMealDb = await _context.Recettes
            .FirstOrDefaultAsync(r => r.MealDBId == recipeId);

        if (recipeByMealDb != null)
        {
            return await _context.Favoris
                .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeByMealDb.RecipeId);
        }

        return false;
    }

    public async Task<Favori?> GetByUserAndRecipeAsync(string userId, string recipeId)
    {
        return await _context.Favoris
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
    }

    public async Task<bool> RemoveByUserAndRecipeAsync(string userId, string recipeId)
    {
        // D'abord chercher par RecipeId interne
        var favorite = await GetByUserAndRecipeAsync(userId, recipeId);

        // Si pas trouvé, chercher par MealDBId
        if (favorite == null)
        {
            var recipeByMealDb = await _context.Recettes
                .FirstOrDefaultAsync(r => r.MealDBId == recipeId);

            if (recipeByMealDb != null)
            {
                favorite = await GetByUserAndRecipeAsync(userId, recipeByMealDb.RecipeId);
            }
        }

        if (favorite == null)
            return false;

        _context.Favoris.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }
}
