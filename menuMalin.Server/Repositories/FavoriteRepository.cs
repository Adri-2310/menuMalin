using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Data;
using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Implémentation du repository pour les favoris
/// </summary>
public class FavoriteRepository : IFavoriteRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Favorite> AddAsync(Favorite favorite)
    {
        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();
        return favorite;
    }

    public async Task<bool> RemoveAsync(string favoriteId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.FavoriteId == favoriteId);

        if (favorite == null)
            return false;

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Recipe)
            .ToListAsync();
    }

    public async Task<bool> IsFavoriteAsync(string userId, string recipeId)
    {
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId);
    }

    public async Task<Favorite?> GetByUserAndRecipeAsync(string userId, string recipeId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
    }

    public async Task<bool> RemoveByUserAndRecipeAsync(string userId, string recipeId)
    {
        var favorite = await GetByUserAndRecipeAsync(userId, recipeId);

        if (favorite == null)
            return false;

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }
}
