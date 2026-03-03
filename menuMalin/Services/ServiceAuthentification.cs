using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;

using menuMalin.Modeles;

namespace menuMalin.Services;

/// <summary>
/// Service d'authentification BFF (Backend for Frontend)
/// </summary>
public class ServiceAuthentification : IServiceAuthentification
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;

    public ServiceAuthentification(HttpClient httpClient, NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }

    public async Task<UtilisateurAuth?> GetCurrentUserAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/me");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UtilisateurAuth>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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

    public async Task<UtilisateurAuth?> LoginAsync(string email, string password)
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
                return JsonSerializer.Deserialize<UtilisateurAuth>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Erreur lors du login: {ex.Message}");
            return null;
        }
    }

    public async Task<UtilisateurAuth?> RegisterAsync(string email, string password, string name)
    {
        try
        {
            var registerRequest = new { email, password, name };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/register");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Content = JsonContent.Create(registerRequest);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UtilisateurAuth>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                // Lire le message d'erreur du serveur
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(errorJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    string errorMessage = "Erreur lors de la création du compte";

                    if (errorObj.TryGetProperty("message", out var messageProp))
                    {
                        errorMessage = messageProp.GetString() ?? errorMessage;
                    }
                    else if (errorObj.TryGetProperty("error", out var errorProp))
                    {
                        errorMessage = errorProp.GetString() ?? errorMessage;
                    }

                    return new UtilisateurAuth { Error = errorMessage };
                }
                catch
                {
                    return new UtilisateurAuth { Error = $"Erreur serveur (HTTP {response.StatusCode})" };
                }
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Erreur lors de l'enregistrement: {ex.Message}");
            return new UtilisateurAuth { Error = ex.Message };
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
        }
        catch
        {
            // Silencieusement ignorer les erreurs - le nettoyage local sera fait par DispositionPrincipale
        }
    }
}

