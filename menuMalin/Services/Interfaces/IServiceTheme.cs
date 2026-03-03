namespace menuMalin.Services;

public interface IServiceTheme
{
    Task<string> GetThemeAsync();
    Task SetThemeAsync(string theme);
    Task<bool> IsDarkModeAsync();
    Task SetDarkModeAsync(bool isDark);
    Task InitializeAsync();
    event Action<string>? OnThemeChanged;
}
