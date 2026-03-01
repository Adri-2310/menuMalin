using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Donnees;
using menuMalin.Server.Modeles.Entites;

using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Depots.Interfaces;

namespace menuMalin.Server.Depots;

/// <summary>
/// Implémentation du repository pour les recettes utilisateur
/// </summary>
public class DepotRecetteUtilisateur : IDepotRecetteUtilisateur
{
    private readonly ApplicationDbContext _context;

    public DepotRecetteUtilisateur(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RecetteUtilisateur> AddAsync(RecetteUtilisateur userRecipe)
    {
        _context.RecettesUtilisateur.Add(userRecipe);
        await _context.SaveChangesAsync();
        return userRecipe;
    }

    public async Task<RecetteUtilisateur?> GetByIdAsync(string userRecipeId)
    {
        return await _context.RecettesUtilisateur
            .FirstOrDefaultAsync(r => r.UserRecipeId == userRecipeId);
    }

    public async Task<IEnumerable<RecetteUtilisateur>> GetByUserIdAsync(string userId)
    {
        return await _context.RecettesUtilisateur
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.DateCreation)
            .ToListAsync();
    }

    public async Task<IEnumerable<RecetteUtilisateur>> GetPublicAsync()
    {
        return await _context.RecettesUtilisateur
            .Where(r => r.IsPublic)
            .OrderByDescending(r => r.DateCreation)
            .ToListAsync();
    }

    public async Task<RecetteUtilisateur?> UpdateAsync(RecetteUtilisateur userRecipe)
    {
        // Récupérer l'entité existante
        var existing = await _context.RecettesUtilisateur.FirstOrDefaultAsync(r => r.UserRecipeId == userRecipe.UserRecipeId);
        if (existing == null)
            return null;

        // Mettre à jour les propriétés
        existing.Title = userRecipe.Title;
        existing.Category = userRecipe.Category;
        existing.Area = userRecipe.Area;
        existing.Instructions = userRecipe.Instructions;
        existing.ImageUrl = userRecipe.ImageUrl;
        existing.IngredientsJson = userRecipe.IngredientsJson;
        existing.IsPublic = userRecipe.IsPublic;
        existing.DateMaj = DateTime.UtcNow;

        // L'entité est déjà trackée, pas besoin d'appeler Update()
        // EF Core détecte automatiquement les changements
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(string userRecipeId)
    {
        var userRecipe = await GetByIdAsync(userRecipeId);
        if (userRecipe == null)
            return false;

        _context.RecettesUtilisateur.Remove(userRecipe);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic)
    {
        var userRecipe = await GetByIdAsync(userRecipeId);
        if (userRecipe == null)
            return false;

        userRecipe.IsPublic = isPublic;
        userRecipe.DateMaj = DateTime.UtcNow;

        _context.RecettesUtilisateur.Update(userRecipe);
        await _context.SaveChangesAsync();
        return true;
    }
}
