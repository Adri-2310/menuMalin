namespace menuMalin.Server.DTOs;

/// <summary>
/// Requête pour modifier le nom de l'utilisateur
/// </summary>
public class RequeteModificationNom
{
    public string? NouveauNom { get; set; }
}

/// <summary>
/// Requête pour modifier le mot de passe de l'utilisateur
/// </summary>
public class RequeteModificationMotDePasse
{
    public string? MotDePasseActuel { get; set; }
    public string? NouveauMotDePasse { get; set; }
    public string? ConfirmationMotDePasse { get; set; }
}
