using Microsoft.AspNetCore.Components;

namespace menuMalin.Services;

/// <summary>
/// Service pour gérer l'authentification des pages Blazor
/// Centralise la logique de vérification d'authentification et redirection
/// </summary>
public interface IPageAuthService
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

public class PageAuthService : IPageAuthService
{
    private readonly IAuthService _authService;

    public PageAuthService(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> CheckAuthenticationAsync()
    {
        return await _authService.IsAuthenticatedAsync();
    }

    public async Task InitializeProtectedPageAsync(NavigationManager nav)
    {
        var isAuthenticated = await CheckAuthenticationAsync();
        if (!isAuthenticated)
        {
            nav.NavigateTo("/");
        }
    }
}
