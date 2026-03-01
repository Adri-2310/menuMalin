using Microsoft.AspNetCore.Components;

namespace menuMalin.Services;

/// <summary>
/// Interface pour gérer l'authentification des pages Blazor
/// Centralise la logique de vérification d'authentification et redirection
/// </summary>
public interface IServiceAuthPage
{
    /// <summary>
    /// Vérifie l'authentification de l'utilisateur
    /// </summary>
    /// <returns>true si authentifié, false sinon</returns>
    Task<bool> CheckAuthenticationAsync();

    /// <summary>
    /// Initialise une page protégée (vérification + redirection si nécessaire)
    /// </summary>
    Task InitializeProtectedPageAsync(NavigationManager nav);
}
