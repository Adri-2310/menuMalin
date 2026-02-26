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

    public AuthController(IUserService userService)
    {
        _userService = userService;
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
        System.Console.WriteLine($"📍 /api/auth/callback - IsAuthenticated: {User.Identity?.IsAuthenticated}");
        System.Console.WriteLine($"   User: {User.Identity?.Name ?? "(anonymous)"}");

        if (!string.IsNullOrEmpty(returnUrl) && Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute))
        {
            return Redirect(returnUrl);
        }
        return Redirect("https://localhost:7777/");
    }

    /// <summary>
    /// Endpoint pour se déconnecter
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        System.Console.WriteLine($"📍 /api/auth/logout called - IsAuthenticated before: {User.Identity?.IsAuthenticated}");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        System.Console.WriteLine($"   IsAuthenticated after SignOut: {User.Identity?.IsAuthenticated}");
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Récupère les informations de l'utilisateur connecté
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("name")?.Value
            ?? User.FindFirst("nickname")?.Value;
        var picture = User.FindFirst("picture")?.Value;

        System.Console.WriteLine($"📍 /api/auth/me called - IsAuthenticated: {User.Identity?.IsAuthenticated}");
        System.Console.WriteLine($"   UserId: {userId}, Email: {email}");

        // Créer ou récupérer l'utilisateur dans la base de données
        if (!string.IsNullOrEmpty(userId))
        {
            try
            {
                await _userService.GetOrCreateUserAsync(userId, email, name);
                System.Console.WriteLine($"✅ Utilisateur synchronisé avec la base de données");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"❌ Erreur lors de la création/synchronisation de l'utilisateur: {ex.Message}");
            }
        }

        return Ok(new
        {
            userId,
            email,
            name,
            picture,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
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
