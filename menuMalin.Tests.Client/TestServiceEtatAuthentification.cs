using menuMalin.Modeles;
using menuMalin.Services.Interfaces;

namespace menuMalin.Tests.Client;

/// <summary>
/// Implémentation stub minimale de IServiceEtatAuthentification pour les tests
/// Par défaut, l'utilisateur est authentifié pour permettre les tests des pages protégées
/// </summary>
public class TestServiceEtatAuthentification : IServiceEtatAuthentification
{
    private UtilisateurAuth? _currentUser;

    /// <param name="isAuthenticated">Si false, le service simule un visiteur non-authentifié</param>
    public TestServiceEtatAuthentification(bool isAuthenticated = true)
    {
        if (isAuthenticated)
        {
            // Par défaut, l'utilisateur est authentifié pour les tests
            _currentUser = new UtilisateurAuth
            {
                UserId = "test-user-123",
                Email = "test@example.com",
                Name = "Test User",
                IsAuthenticated = true
            };
        }
        else
        {
            // Simuler un visiteur non-authentifié
            _currentUser = null;
        }
    }

    public UtilisateurAuth? CurrentUser
    {
        get => _currentUser;
        set => _currentUser = value;
    }

    public bool IsAuthenticated => _currentUser?.IsAuthenticated ?? false;

    public event Action? AuthenticationChanged;

    public Task InitializeAuthStateAsync() => Task.CompletedTask;

    public Task ClearAuthStateAsync()
    {
        _currentUser = null;
        NotifyAuthenticationChanged();
        return Task.CompletedTask;
    }

    public void NotifyAuthenticationChanged() => AuthenticationChanged?.Invoke();
}
