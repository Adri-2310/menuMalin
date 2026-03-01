using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Donnees;
using menuMalin.Server.Modeles.Entites;

using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Depots.Interfaces;

namespace menuMalin.Server.Depots;

/// <summary>
/// Repository pour la gestion des données utilisateur
/// </summary>
public class DepotUtilisateur : IDepotUtilisateur
{
    private readonly ApplicationDbContext _context;

    public DepotUtilisateur(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Utilisateur?> GetByEmailAsync(string email)
    {
        return await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    // Backward compatibility alias
    public async Task<Utilisateur?> GetByAuth0IdAsync(string auth0IdOrUserId)
    {
        // Traiter comme UserId (maintenant que Auth0Id n'existe plus)
        return await GetByUserIdAsync(auth0IdOrUserId);
    }

    public async Task<Utilisateur?> GetByUserIdAsync(string userId)
    {
        return await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<Utilisateur> AddAsync(Utilisateur user)
    {
        _context.Utilisateurs.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<Utilisateur> UpdateAsync(Utilisateur user)
    {
        _context.Utilisateurs.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
