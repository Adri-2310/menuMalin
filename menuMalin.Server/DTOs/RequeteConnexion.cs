namespace menuMalin.Server.DTOs;

/// <summary>
/// DTO pour la requête de login
/// </summary>
public class RequeteConnexion
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
