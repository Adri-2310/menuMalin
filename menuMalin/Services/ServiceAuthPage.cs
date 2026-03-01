using Microsoft.AspNetCore.Components;

namespace menuMalin.Services;

/// <summary>
/// Service pour gérer l'authentification des pages Blazor
/// Centralise la logique de vérification d'authentification et redirection
/// </summary>
public class ServiceAuthPage : IServiceAuthPage
{
    private readonly IServiceAuthentification _authService;

    public ServiceAuthPage(IServiceAuthentification authService)
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
