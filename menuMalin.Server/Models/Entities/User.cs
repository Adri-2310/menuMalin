namespace menuMalin.Server.Models.Entities;

/// <summary>
/// Représente un utilisateur mappé à Auth0
/// Architecture Hybrid: Le profil complet (Name, Picture) est stocké dans localStorage client
/// Cette entité stocke seulement les données critiques pour les relations (Auth0Id, Email)
/// </summary>
public class User
{
    /// <summary>
    /// Identifiant unique de l'utilisateur (UUID)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant unique Auth0 (ex: "auth0|123456789")
    /// </summary>
    public string Auth0Id { get; set; } = string.Empty;

    /// <summary>
    /// Email de l'utilisateur (pour référence et récupération de compte)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Date de création du compte
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Recettes favorites de l'utilisateur
    /// </summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    /// <summary>
    /// Messages de contact envoyés par l'utilisateur
    /// </summary>
    public ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
}
