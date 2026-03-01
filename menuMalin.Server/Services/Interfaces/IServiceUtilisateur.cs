using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Services.Interfaces;

/// <summary>
/// Interface pour la gestion des utilisateurs
/// </summary>
public interface IServiceUtilisateur
{
    /// <summary>
    /// Récupère ou crée un utilisateur basé sur son email
    /// </summary>
    /// <param name="emailOrUserId">L'email ou identifiant de l'utilisateur</param>
    /// <param name="email">L'email de l'utilisateur (optionnel)</param>
    /// <param name="name">Le nom de l'utilisateur (optionnel)</param>
    /// <returns>L'utilisateur créé ou récupéré</returns>
    Task<Utilisateur> GetOrCreateUserAsync(string emailOrUserId, string? email = null, string? name = null);

    /// <summary>
    /// Récupère un utilisateur par son email
    /// </summary>
    Task<Utilisateur?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Crée un nouvel utilisateur
    /// </summary>
    Task<Utilisateur> CreateUserAsync(string email, string password, string name);

    /// <summary>
    /// Récupère un utilisateur par son UserId (backward compatibility)
    /// </summary>
    Task<Utilisateur?> GetUserByAuth0IdAsync(string userIdOrAuth0Id);
}
