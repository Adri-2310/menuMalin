namespace menuMalin.Server.Models.Entities;

/// <summary>
/// Représente une recette créée par un utilisateur
/// </summary>
public class UserRecipe
{
    /// <summary>
    /// Identifiant unique de la recette utilisateur (UUID)
    /// </summary>
    public string UserRecipeId { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant de l'utilisateur qui a créé la recette (clé étrangère)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Titre de la recette
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Catégorie de la recette
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Cuisine/zone géographique
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// Instructions de préparation
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// URL de l'image de la recette
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Ingrédients au format JSON
    /// </summary>
    public string IngredientsJson { get; set; } = "[]";

    /// <summary>
    /// Indique si la recette est publique (visible à tous)
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Utilisateur qui a créé cette recette
    /// </summary>
    public User User { get; set; } = null!;
}
