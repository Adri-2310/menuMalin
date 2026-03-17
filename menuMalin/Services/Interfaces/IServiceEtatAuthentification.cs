using menuMalin.Modeles;

namespace menuMalin.Services.Interfaces;

/// <summary>
/// Interface pour le service d'état d'authentification
/// Gère l'état d'authentification avec persistence dans localStorage
/// </summary>
public interface IServiceEtatAuthentification
{
    /// <summary>Utilisateur actuellement authentifié</summary>
    UtilisateurAuth? CurrentUser { get; set; }

    /// <summary>Indique si un utilisateur est authentifié</summary>
    bool IsAuthenticated { get; }

    /// <summary>Événement déclenché lors d'un changement d'authentification</summary>
    event Action? AuthenticationChanged;

    /// <summary>Initialiser l'état d'authentification depuis localStorage</summary>
    Task InitializeAuthStateAsync();

    /// <summary>Nettoyer l'état d'authentification (déconnexion)</summary>
    Task ClearAuthStateAsync();

    /// <summary>Notifier que l'authentification a changé</summary>
    void NotifyAuthenticationChanged();
}
