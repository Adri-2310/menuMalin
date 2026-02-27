using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using menuMalin.Server.Services;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur d'authentification BFF (Backend for Frontend)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Endpoint pour lancer le login (redirige vers Auth0)
    /// </summary>
    [HttpGet("login")]
    [AllowAnonymous]
    public async Task Login(string returnUrl = "/")
    {
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        await HttpContext.ChallengeAsync("Auth0", authenticationProperties);
    }

    /// <summary>
    /// Callback depuis Auth0 (géré automatiquement par le middleware)
    /// </summary>
    [HttpGet("callback")]
    [AllowAnonymous]
    public IActionResult Callback(string? returnUrl = null)
    {
        // Ce endpoint est appelé par Auth0 après l'authentification
        // Le middleware OAuth gère automatiquement l'échange du code et crée un cookie
        // NE PAS faire un deuxième SignInAsync - le middleware l'a déjà fait !
        _logger.LogDebug("Auth callback - IsAuthenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);
        _logger.LogDebug("Auth callback - User: {UserName}", User.Identity?.Name ?? "(anonymous)");

        // Rediriger vers le frontend après l'authentification
        // Ajouter un délai pour que le navigateur établisse le cookie avant de charger le frontend
        var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>Redirection...</title>
</head>
<body>
    <p>Redirection en cours...</p>
    <script>
        setTimeout(function() {
            window.location.href = 'https://localhost:7777/';
        }, 1000);
    </script>
</body>
</html>";
        return Content(html, "text/html");
    }

    /// <summary>
    /// Endpoint pour se déconnecter
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Vérification basique anti-CSRF : vérifier que la requête vient du même domaine
        var origin = Request.Headers["Origin"].ToString();
        var host = Request.Host.Host;

        // Rejeter les requêtes cross-origin sans validation supplémentaire
        if (!string.IsNullOrEmpty(origin))
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var originUri))
            {
                return BadRequest(new { error = "Origin invalide" });
            }

            // Vérifier que l'origine correspond au host actuel
            if (!originUri.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid("CORS policy violation - logout from different origin");
            }
        }

        _logger.LogDebug("Auth logout - IsAuthenticated before: {IsAuthenticated}", User.Identity?.IsAuthenticated);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogDebug("Auth logout - IsAuthenticated after: {IsAuthenticated}", User.Identity?.IsAuthenticated);
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Récupère les informations de l'utilisateur connecté (ou retourne isAuthenticated:false si non authentifié)
    /// Utilise [AllowAnonymous] pour éviter les redirects OAuth cross-origin qui causent des erreurs CORS
    /// </summary>
    [HttpGet("me")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCurrentUser()
    {
        // Si pas authentifié, retourner une réponse vide sans erreur
        // Cela évite les redirects OAuth qui causent des erreurs CORS avec le frontend WASM
        if (User.Identity?.IsAuthenticated != true)
        {
            _logger.LogDebug("Auth me - Utilisateur non authentifié");
            return Ok(new
            {
                userId = (string?)null,
                email = (string?)null,
                name = (string?)null,
                picture = (string?)null,
                isAuthenticated = false
            });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("name")?.Value
            ?? User.FindFirst("nickname")?.Value;
        var picture = User.FindFirst("picture")?.Value;

        _logger.LogDebug("Auth me - IsAuthenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);
        _logger.LogDebug("Auth me - UserId: {UserId}", userId);

        // Créer ou récupérer l'utilisateur dans la base de données
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await _userService.GetOrCreateUserAsync(userId, email, name);
                _logger.LogInformation("Utilisateur synchronisé avec la base de données");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création/synchronisation de l'utilisateur");
            }
        }

        return Ok(new
        {
            userId,
            email,
            name,
            picture,
            isAuthenticated = true
        });
    }

    /// <summary>
    /// Endpoint public pour vérifier que l'API est accessible
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
}
