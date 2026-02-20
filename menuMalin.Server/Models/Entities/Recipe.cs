namespace menuMalin.Server.Models.Entities;

/// <summary>
/// Représente une recette provenant de TheMealDB ou créée par un utilisateur
/// </summary>
public class Recipe
{
    /// <summary>
    /// Identifiant unique de la recette (UUID)
    /// </summary>
    public string RecipeId { get; set; } = string.Empty;

    /// <summary>
    /// Titre/Nom de la recette
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description de la recette
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Instructions de préparation
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// URL de l'image de la recette
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Identifiant TheMealDB (pour référence à l'API externe)
    /// </summary>
    public string MealDBId { get; set; } = string.Empty;

    /// <summary>
    /// Catégorie de la recette (ex: "Pasta", "Dessert", etc.)
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Cuisine/Région (ex: "Italian", "French", etc.)
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// Tags/Mots-clés de la recette
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière mise à jour
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Favoris contenant cette recette
    /// </summary>
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}