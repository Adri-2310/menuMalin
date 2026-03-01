using Blazored.LocalStorage;
using menuMalin.Modeles;

namespace menuMalin.Services;

/// <summary>
/// Service partagé pour gérer l'état d'authentification avec événements
/// Persiste l'authentification dans localStorage pour survivre aux F5
/// </summary>
public class ServiceEtatAuthentification
{
    private UtilisateurAuth? _currentUser;
    private readonly ILocalStorageService _localStorage;
    private const string AUTH_KEY = "user_auth_state";

    public event Action? AuthenticationChanged;

    public ServiceEtatAuthentification(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public UtilisateurAuth? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser?.Email != value?.Email || _currentUser?.IsAuthenticated != value?.IsAuthenticated)
            {
                _currentUser = value;
                // Sauvegarder dans localStorage
                _ = SaveAuthStateAsync();
                NotifyAuthenticationChanged();
            }
        }
    }

    public bool IsAuthenticated => _currentUser?.IsAuthenticated ?? false;

    /// <summary>
    /// Charger l'état d'authentification depuis localStorage
    /// À appeler au démarrage de l'app
    /// </summary>
    public async Task InitializeAuthStateAsync()
    {
        try
        {
            var savedUser = await _localStorage.GetItemAsync<UtilisateurAuth>(AUTH_KEY);
            if (savedUser?.IsAuthenticated == true)
            {
                _currentUser = savedUser;
                // Ne pas déclencher l'événement pendant l'init
            }
        }
        catch
        {
            // Si localStorage est vide ou erreur, c'est normal
        }
    }

    /// <summary>
    /// Sauvegarder l'état d'authentification dans localStorage
    /// </summary>
    private async Task SaveAuthStateAsync()
    {
        try
        {
            if (_currentUser?.IsAuthenticated == true)
            {
                await _localStorage.SetItemAsync(AUTH_KEY, _currentUser);
            }
            else
            {
                await _localStorage.RemoveItemAsync(AUTH_KEY);
            }
        }
        catch
        {
            // Si localStorage échoue, continuer sans persister
        }
    }

    /// <summary>
    /// Nettoyer l'authentification (déconnexion)
    /// </summary>
    public async Task ClearAuthStateAsync()
    {
        _currentUser = null;
        await _localStorage.RemoveItemAsync(AUTH_KEY);
        NotifyAuthenticationChanged();
    }

    public void NotifyAuthenticationChanged()
    {
        AuthenticationChanged?.Invoke();
    }
}
