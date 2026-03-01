namespace menuMalin.Shared.Modeles.Requetes;

/// <summary>
/// Requête pour créer une recette utilisateur
/// </summary>
public class RequeteCreationRecetteUtilisateur
{
    public string Title { get; set; } = string.Empty;

    public string? Category { get; set; }

    public string? Area { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public List<string> Ingredients { get; set; } = new();

    public bool IsPublic { get; set; } = false;
}
