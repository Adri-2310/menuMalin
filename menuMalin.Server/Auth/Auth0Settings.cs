namespace menuMalin.Server.Auth;

/// <summary>
/// Paramètres de configuration Auth0
/// </summary>
public class Auth0Settings
{
    /// <summary>
    /// Domaine Auth0 (ex: dev-xxx.eu.auth0.com)
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// ID du client Auth0
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Secret du client Auth0
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Audience API Auth0 (identifiant de l'API)
    /// </summary>
    public string Audience { get; set; } = string.Empty;
}
