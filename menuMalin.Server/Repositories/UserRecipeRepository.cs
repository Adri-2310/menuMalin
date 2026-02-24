using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Data;
using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Implémentation du repository pour les recettes utilisateur
/// </summary>
public class UserRecipeRepository : IUserRecipeRepository
{
    private readonly ApplicationDbContext _context;

    public UserRecipeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserRecipe> AddAsync(UserRecipe userRecipe)
    {
        _context.UserRecipes.Add(userRecipe);
        await _context.SaveChangesAsync();
        return userRecipe;
    }

    public async Task<UserRecipe?> GetByIdAsync(string userRecipeId)
    {
        return await _context.UserRecipes
            .FirstOrDefaultAsync(r => r.UserRecipeId == userRecipeId);
    }

    public async Task<IEnumerable<UserRecipe>> GetByUserIdAsync(string userId)
    {
        return await _context.UserRecipes
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserRecipe>> GetPublicAsync()
    {
        return await _context.UserRecipes
            .Where(r => r.IsPublic)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<UserRecipe?> UpdateAsync(UserRecipe userRecipe)
    {
        var existing = await GetByIdAsync(userRecipe.UserRecipeId);
        if (existing == null)
            return null;

        existing.Title = userRecipe.Title;
        existing.Category = userRecipe.Category;
        existing.Area = userRecipe.Area;
        existing.Instructions = userRecipe.Instructions;
        existing.ImageUrl = userRecipe.ImageUrl;
        existing.IngredientsJson = userRecipe.IngredientsJson;
        existing.IsPublic = userRecipe.IsPublic;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.UserRecipes.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(string userRecipeId)
    {
        var userRecipe = await GetByIdAsync(userRecipeId);
        if (userRecipe == null)
            return false;

        _context.UserRecipes.Remove(userRecipe);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic)
    {
        var userRecipe = await GetByIdAsync(userRecipeId);
        if (userRecipe == null)
            return false;

        userRecipe.IsPublic = isPublic;
        userRecipe.UpdatedAt = DateTime.UtcNow;

        _context.UserRecipes.Update(userRecipe);
        await _context.SaveChangesAsync();
        return true;
    }
}
