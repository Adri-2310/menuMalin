using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Services;

/// <summary>
/// Interface pour la gestion des utilisateurs
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Récupère ou crée un utilisateur basé sur son email
    /// </summary>
    /// <param name="emailOrUserId">L'email ou identifiant de l'utilisateur</param>
    /// <param name="email">L'email de l'utilisateur (optionnel)</param>
    /// <param name="name">Le nom de l'utilisateur (optionnel)</param>
    /// <returns>L'utilisateur créé ou récupéré</returns>
    Task<User> GetOrCreateUserAsync(string emailOrUserId, string? email = null, string? name = null);

    /// <summary>
    /// Récupère un utilisateur par son UserId (backward compatibility)
    /// </summary>
    Task<User?> GetUserByAuth0IdAsync(string userIdOrAuth0Id);
}
