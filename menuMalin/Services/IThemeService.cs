namespace menuMalin.Services;

/// <summary>
/// Interface pour le service de thème (dark/light)
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Récupère le thème actuel
    /// </summary>
    Task<string> GetThemeAsync();

    /// <summary>
    /// Change le thème et sauvegarde
    /// </summary>
    Task SetThemeAsync(string theme);

    /// <summary>
    /// Vérifie si le mode dark est activé
    /// </summary>
    Task<bool> IsDarkModeAsync();

    /// <summary>
    /// Active ou désactive le mode dark
    /// </summary>
    Task SetDarkModeAsync(bool isDark);

    /// <summary>
    /// Événement déclenché quand le thème change
    /// </summary>
    event Action<string>? OnThemeChanged;
}
