namespace menuMalin.Shared.Models.Dtos;

/// <summary>
/// DTO pour les recettes
/// </summary>
public class RecipeDto
{
    public string RecipeId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string MealDBId { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string? Area { get; set; }

    public string? Tags { get; set; }

    public List<IngredientDto> Ingredients { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
