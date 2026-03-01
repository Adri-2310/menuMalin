namespace menuMalin.Server.Modeles.Entites;

/// <summary>
/// Représente une recette marquée comme favorite par un utilisateur
/// </summary>
public class Favori
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
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Utilisateur qui a ajouté ce favori
    /// </summary>
    public Utilisateur? Utilisateur { get; set; }

    /// <summary>
    /// Recette favorite
    /// </summary>
    public Recette? Recette { get; set; }
}
