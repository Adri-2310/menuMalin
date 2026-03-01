namespace menuMalin.DTOs;

/// <summary>
/// Requête pour créer une recette utilisateur
/// </summary>
public class RequeteCreationRecetteUtilisateur
{
    public string Title { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public string? Area { get; set; }
}
