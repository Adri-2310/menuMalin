namespace menuMalin.Shared.Models.Dtos;

/// <summary>
/// DTO pour les recettes créées par les utilisateurs
/// </summary>
public class UserRecipeDto
{
    public string UserRecipeId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string? Area { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public List<string> Ingredients { get; set; } = new();

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
