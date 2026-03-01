namespace menuMalin.Modeles;

/// <summary>
/// Profil utilisateur stocké dans localStorage
/// </summary>
public class ProfilUtilisateur
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Picture { get; set; }

    public string Sub { get; set; } = string.Empty;
}
