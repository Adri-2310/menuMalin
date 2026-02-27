using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Interface pour accéder aux données utilisateur
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Récupère un utilisateur par son email
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Récupère un utilisateur par son UserId (UUID)
    /// </summary>
    Task<User?> GetByUserIdAsync(string userId);

    /// <summary>
    /// Ajoute un nouvel utilisateur
    /// </summary>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Met à jour un utilisateur existant
    /// </summary>
    Task<User> UpdateAsync(User user);
}
