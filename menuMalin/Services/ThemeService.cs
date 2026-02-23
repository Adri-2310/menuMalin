using Microsoft.JSInterop;

namespace menuMalin.Services;

/// <summary>
/// Service pour gérer le thème (dark/light)
/// </summary>
public class ThemeService : IThemeService
{
    private readonly LocalStorageService _storageService;
    private readonly IJSRuntime _jsRuntime;
    private string _currentTheme = "light";

    public event Action<string>? OnThemeChanged;

    public ThemeService(LocalStorageService storageService, IJSRuntime jsRuntime)
    {
        _storageService = storageService;
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetThemeAsync()
    {
        _currentTheme = await _storageService.GetThemeAsync();
        return _currentTheme;
    }

    public async Task SetThemeAsync(string theme)
    {
        _currentTheme = theme;
        await _storageService.SetThemeAsync(theme);

        // Appliquer le thème au DOM
        await ApplyThemeAsync(theme);

        // Notifier les abonnés
        OnThemeChanged?.Invoke(theme);
    }

    public async Task<bool> IsDarkModeAsync()
    {
        var theme = await GetThemeAsync();
        return theme == "dark";
    }

    public async Task SetDarkModeAsync(bool isDark)
    {
        var theme = isDark ? "dark" : "light";
        await SetThemeAsync(theme);
    }

    private async Task ApplyThemeAsync(string theme)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("applyTheme", theme);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur application thème: {ex.Message}");
        }
    }

    /// <summary>
    /// Initialise le thème au démarrage
    /// </summary>
    public async Task InitializeAsync()
    {
        var theme = await GetThemeAsync();
        await ApplyThemeAsync(theme);
    }
}
