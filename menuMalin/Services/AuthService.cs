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
    Task<AuthUser?> LoginAsync(string email, string password);
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

    public async Task<AuthUser?> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new { email, password };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/login");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Content = JsonContent.Create(loginRequest);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthUser>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Erreur lors du login: {ex.Message}");
            return null;
        }
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
