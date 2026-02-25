using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Services;

/// <summary>
/// Interface pour la gestion des utilisateurs
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Récupère ou crée un utilisateur basé sur ses informations Auth0
    /// </summary>
    /// <param name="auth0Id">L'identifiant Auth0 (ex: "auth0|123456")</param>
    /// <param name="email">L'email de l'utilisateur</param>
    /// <param name="name">Le nom de l'utilisateur (optionnel)</param>
    /// <returns>L'utilisateur créé ou récupéré</returns>
    Task<User> GetOrCreateUserAsync(string auth0Id, string? email = null, string? name = null);

    /// <summary>
    /// Récupère un utilisateur par son Auth0Id
    /// </summary>
    Task<User?> GetUserByAuth0IdAsync(string auth0Id);
}
