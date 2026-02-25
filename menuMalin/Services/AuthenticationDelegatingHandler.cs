using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace menuMalin.Services;

/// <summary>
/// Handler pour ajouter automatiquement le token Bearer à chaque requête HTTP
/// </summary>
public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;

    public AuthenticationDelegatingHandler(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Récupérer le token d'accès
            var tokenResult = await _accessTokenProvider.RequestAccessToken();

            // Si le token est disponible, l'ajouter au header Authorization
            if (tokenResult.TryGetToken(out var token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
            }
        }
        catch (Exception ex)
        {
            // Silently fail - le token n'a pas pu être récupéré, la requête sera rejetée avec 401
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
