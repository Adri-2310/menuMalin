using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Depots.Interfaces;

/// <summary>
/// Interface pour accéder aux données utilisateur
/// </summary>
public interface IDepotUtilisateur
{
    /// <summary>
    /// Récupère un utilisateur par son email
    /// </summary>
    Task<Utilisateur?> GetByEmailAsync(string email);

    /// <summary>
    /// Récupère un utilisateur par son UserId (UUID)
    /// </summary>
    Task<Utilisateur?> GetByUserIdAsync(string userId);

    /// <summary>
    /// Ajoute un nouvel utilisateur
    /// </summary>
    Task<Utilisateur> AddAsync(Utilisateur user);

    /// <summary>
    /// Met à jour un utilisateur existant
    /// </summary>
    Task<Utilisateur> UpdateAsync(Utilisateur user);
}
