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
                Console.WriteLine($"✅ Token ajouté à la requête vers: {request.RequestUri}");
            }
            else
            {
                Console.WriteLine($"⚠️ Impossible de récupérer le token pour: {request.RequestUri}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur dans AuthenticationDelegatingHandler: {ex.Message}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
