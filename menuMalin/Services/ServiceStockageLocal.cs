using System.Text.Json;
using Blazored.LocalStorage;

namespace menuMalin.Services;

using menuMalin.Modeles;

/// <summary>
/// Service pour gérer le localStorage (profil utilisateur, thème, etc)
/// </summary>
public class ServiceStockageLocal
{
    private readonly ILocalStorageService _localStorage;

    public ServiceStockageLocal(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Récupère le profil utilisateur depuis localStorage
    /// </summary>
    public async Task<ProfilUtilisateur?> GetUserProfileAsync()
    {
        try
        {
            var profile = await _localStorage.GetItemAsync<ProfilUtilisateur>("user:profile");
            return profile;
        }
        catch
        {
            // Logged in development
            return null;
        }
    }

    /// <summary>
    /// Sauvegarde le profil utilisateur dans localStorage
    /// </summary>
    public async Task SetUserProfileAsync(ProfilUtilisateur profile)
    {
        try
        {
            await _localStorage.SetItemAsync("user:profile", profile);
        }
        catch
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
        catch
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
        catch
        {
            // Logged in development
        }
    }
}

