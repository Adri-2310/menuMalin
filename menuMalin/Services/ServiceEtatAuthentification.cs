using menuMalin.Modeles;

namespace menuMalin.Services;

/// <summary>
/// Service partagé pour gérer l'état d'authentification avec événements
/// Permet au layout de réagir immédiatement aux changements de connexion
/// </summary>
public class ServiceEtatAuthentification
{
    private UtilisateurAuth? _currentUser;

    public event Action? AuthenticationChanged;

    public UtilisateurAuth? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser?.Email != value?.Email || _currentUser?.IsAuthenticated != value?.IsAuthenticated)
            {
                _currentUser = value;
                NotifyAuthenticationChanged();
            }
        }
    }

    public bool IsAuthenticated => _currentUser?.IsAuthenticated ?? false;

    public void NotifyAuthenticationChanged()
    {
        AuthenticationChanged?.Invoke();
    }
}
