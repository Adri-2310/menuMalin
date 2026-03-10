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

    /// <summary>
    /// Modifie le nom de l'utilisateur connecté
    /// </summary>
    /// <returns>Tuple (Succès, MessageErreur, NouveauNom)</returns>
    Task<(bool Succes, string? Erreur, string? NouveauNom)> ModifierNomAsync(string nouveauNom);

    /// <summary>
    /// Modifie le mot de passe de l'utilisateur connecté
    /// </summary>
    /// <returns>Tuple (Succès, MessageErreur)</returns>
    Task<(bool Succes, string? Erreur)> ModifierMotDePasseAsync(
        string motDePasseActuel, string nouveauMotDePasse, string confirmationMotDePasse);
}
