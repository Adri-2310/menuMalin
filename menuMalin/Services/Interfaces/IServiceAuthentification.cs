using menuMalin.Modeles;

namespace menuMalin.Services;

/// <summary>
/// Interface pour le service d'authentification BFF (Backend for Frontend)
/// </summary>
public interface IServiceAuthentification
{
    Task<UtilisateurAuth?> GetCurrentUserAsync();
    Task<UtilisateurAuth?> LoginAsync(string email, string password);
    Task<UtilisateurAuth?> RegisterAsync(string email, string password, string name);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
}
