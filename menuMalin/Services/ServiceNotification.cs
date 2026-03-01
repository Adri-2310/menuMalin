using Blazored.Toast.Services;
using System.Net;

namespace menuMalin.Services;

public class ServiceNotification : IServiceNotification
{
    private readonly IToastService _toastService;

    public ServiceNotification(IToastService toastService)
    {
        _toastService = toastService;
    }

    public void AfficherSucces(string message, string? titre = null)
        => _toastService.ShowSuccess(message);

    public void AfficherErreur(string message, string? titre = null)
        => _toastService.ShowError(message);

    public void AfficherAvertissement(string message, string? titre = null)
        => _toastService.ShowWarning(message);

    public void AfficherInfo(string message, string? titre = null)
        => _toastService.ShowInfo(message);

    public void AfficherErreurHttp(HttpStatusCode statusCode, string? contexte = null)
    {
        var msg = statusCode switch
        {
            HttpStatusCode.Unauthorized => "Vous devez être connecté pour effectuer cette action.",
            HttpStatusCode.Forbidden => "Vous n'avez pas les droits pour effectuer cette action.",
            HttpStatusCode.NotFound => "La ressource demandée est introuvable.",
            HttpStatusCode.RequestTimeout or HttpStatusCode.GatewayTimeout =>
                "Le serveur met trop de temps à répondre. Réessayez dans quelques instants.",
            HttpStatusCode.InternalServerError => "Une erreur interne s'est produite. Réessayez plus tard.",
            HttpStatusCode.ServiceUnavailable => "Le service est temporairement indisponible.",
            _ => $"Erreur de communication (code {(int)statusCode})."
        };

        _toastService.ShowError(msg);
    }

    public void AfficherErreurReseau()
        => _toastService.ShowError(
            "Impossible de contacter le serveur. Vérifiez votre connexion internet.");
}
