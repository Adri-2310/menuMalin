namespace menuMalin.Server.DTOs;

/// <summary>
/// DTO pour la requête de registration
/// </summary>
public class RequeteInscription
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
}
