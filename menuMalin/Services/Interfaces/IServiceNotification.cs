using System.Net;

namespace menuMalin.Services;

public interface IServiceNotification
{
    void AfficherSucces(string message, string? titre = null);
    void AfficherErreur(string message, string? titre = null);
    void AfficherAvertissement(string message, string? titre = null);
    void AfficherInfo(string message, string? titre = null);
    void AfficherErreurHttp(HttpStatusCode statusCode, string? contexte = null);
    void AfficherErreurReseau();
}
