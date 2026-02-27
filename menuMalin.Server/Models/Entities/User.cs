namespace menuMalin.Server.Models.Entities;

/// <summary>
/// Représente un utilisateur de l'application
/// Architecture simple: Authentification par email + password (stockés en base)
/// </summary>
public class User
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
    /// Date de création du compte
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Recettes créées par l'utilisateur
    /// </summary>
    public ICollection<UserRecipe> UserRecipes { get; set; } = new List<UserRecipe>();

    /// <summary>
    /// Recettes favorites de l'utilisateur
    /// </summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    /// <summary>
    /// Messages de contact envoyés par l'utilisateur
    /// </summary>
    public ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
}
