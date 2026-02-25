using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Data;
using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Repository pour la gestion des données utilisateur
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByAuth0IdAsync(string auth0Id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);
    }

    public async Task<User?> GetByUserIdAsync(string userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
