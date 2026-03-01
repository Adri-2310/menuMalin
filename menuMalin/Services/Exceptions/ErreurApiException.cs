using System.Net;

namespace menuMalin.Services.Exceptions;

public class ErreurApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ContenuErreur { get; }

    public ErreurApiException(HttpStatusCode statusCode, string? contenuErreur = null)
        : base($"Erreur API: HTTP {(int)statusCode}")
    {
        StatusCode = statusCode;
        ContenuErreur = contenuErreur;
    }
}
