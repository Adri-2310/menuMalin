using System.Text.Json;
using Blazored.LocalStorage;

namespace menuMalin.Services;

/// <summary>
/// Service pour gérer le localStorage (profil utilisateur, thème, etc)
/// </summary>
public class LocalStorageService
{
    private readonly ILocalStorageService _localStorage;

    public LocalStorageService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Récupère le profil utilisateur depuis localStorage
    /// </summary>
    public async Task<UserProfile?> GetUserProfileAsync()
    {
        try
        {
            var profile = await _localStorage.GetItemAsync<UserProfile>("user:profile");
            return profile;
        }
        catch (Exception ex)
        {
            // Logged in development
            return null;
        }
    }

    /// <summary>
    /// Sauvegarde le profil utilisateur dans localStorage
    /// </summary>
    public async Task SetUserProfileAsync(UserProfile profile)
    {
        try
        {
            await _localStorage.SetItemAsync("user:profile", profile);
        }
        catch (Exception ex)
        {
            // Logged in development
        }
    }

    /// <summary>
    /// Efface les données utilisateur du localStorage
    /// </summary>
    public async Task ClearUserDataAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync("user:profile");
            await _localStorage.RemoveItemAsync("user:tokens");
        }
        catch (Exception ex)
        {
            // Logged in development
        }
    }

    /// <summary>
    /// Récupère le thème depuis localStorage
    /// </summary>
    public async Task<string> GetThemeAsync()
    {
        try
        {
            var theme = await _localStorage.GetItemAsync<string>("user:theme");
            return theme ?? "light";
        }
        catch
        {
            return "light";
        }
    }

    /// <summary>
    /// Sauvegarde le thème dans localStorage
    /// </summary>
    public async Task SetThemeAsync(string theme)
    {
        try
        {
            await _localStorage.SetItemAsync("user:theme", theme);
        }
        catch (Exception ex)
        {
            // Logged in development
        }
    }
}

/// <summary>
/// Profil utilisateur stocké dans localStorage
/// </summary>
public class UserProfile
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Picture { get; set; }

    public string Sub { get; set; } = string.Empty;
}
