namespace menuMalin.Server.Models.Entities;

/// <summary>
/// Représente une recette marquée comme favorite par un utilisateur
/// </summary>
public class Favorite
{
    /// <summary>
    /// Identifiant unique du favori (UUID)
    /// </summary>
    public string FavoriteId { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant de l'utilisateur (clé étrangère)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant de la recette (clé étrangère)
    /// </summary>
    public string RecipeId { get; set; } = string.Empty;

    /// <summary>
    /// Date d'ajout aux favoris
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Utilisateur qui a ajouté ce favori
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Recette favorite
    /// </summary>
    public Recipe Recipe { get; set; } = null!;
}
