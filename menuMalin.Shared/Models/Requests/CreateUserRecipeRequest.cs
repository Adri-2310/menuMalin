namespace menuMalin.Shared.Models.Requests;

/// <summary>
/// Requête pour créer une recette utilisateur
/// </summary>
public class CreateUserRecipeRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string? Area { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public List<string> Ingredients { get; set; } = new();

    public bool IsPublic { get; set; } = false;
}
