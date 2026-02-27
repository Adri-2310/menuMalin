using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;

namespace menuMalin.Services;

/// <summary>
/// Service d'authentification BFF (Backend for Frontend)
/// </summary>
public interface IAuthService
{
    Task<AuthUser?> GetCurrentUserAsync();
    Task LoginAsync();
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(HttpClient httpClient, NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public async Task<AuthUser?> GetCurrentUserAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthUser>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.IsAuthenticated ?? false;
    }

    public Task LoginAsync()
    {
        // Redirige vers le backend pour le login Auth0 (URL relative au BaseAddress du HttpClient)
        // Le BaseAddress est défini dans Program.cs à partir de appsettings.json
        var loginUrl = _httpClient.BaseAddress != null
            ? $"{_httpClient.BaseAddress.AbsoluteUri}api/auth/login?returnUrl=/"
            : "/api/auth/login?returnUrl=/";
        _navigationManager.NavigateTo(loginUrl);
        return Task.CompletedTask;
    }

    public async Task LogoutAsync()
    {
        try
        {
            // Appeler le endpoint logout du backend
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Content = new StringContent("");
            await _httpClient.SendAsync(request);

            // Force un vrai rechargement complet du navigateur
            // window.location.reload() recharge la page depuis le serveur, réinitialisant complètement Blazor
            await _jsRuntime.InvokeVoidAsync("window.location.reload");
        }
        catch
        {
            // En cas d'erreur, forcer quand même le rechargement
            await _jsRuntime.InvokeVoidAsync("window.location.reload");
        }
    }
}

public class AuthUser
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public bool IsAuthenticated { get; set; }
}
