namespace menuMalin.Server.Modeles.Entites;

/// <summary>
/// Représente un utilisateur de l'application
/// Architecture simple: Authentification par email + password (stockés en base)
/// </summary>
public class Utilisateur
{
    /// <summary>
    /// Identifiant unique de l'utilisateur (UUID)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Email de l'utilisateur (identifiant de connexion unique)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nom de l'utilisateur (affiché dans l'interface)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Hash du mot de passe (BCrypt, workFactor 12)
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Date de création du compte
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Recettes créées par l'utilisateur
    /// </summary>
    public ICollection<RecetteUtilisateur> RecettesUtilisateur { get; set; } = new List<RecetteUtilisateur>();

    /// <summary>
    /// Recettes favorites de l'utilisateur
    /// </summary>
    public ICollection<Favori> Favoris { get; set; } = new List<Favori>();

    /// <summary>
    /// Messages de contact envoyés par l'utilisateur
    /// </summary>
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
