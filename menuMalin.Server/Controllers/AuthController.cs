using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using menuMalin.Server.Services;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur d'authentification BFF (Backend for Frontend)
/// Authentification simple: Email + Password → Cookie HttpOnly
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
    /// POST /api/auth/login - Authentifier avec email + password
    /// Retourne un cookie HttpOnly si authentification réussie
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validation des entrées
        if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Password))
        {
            _logger.LogWarning("Login attempt with missing email or password");
            return BadRequest(new { error = "Email et password requis" });
        }

        // Validation du format email
        if (!request.Email.Contains("@"))
        {
            return BadRequest(new { error = "Format email invalide" });
        }

        try
        {
            // Pour DEV: Accepter n'importe quel email/password (validation simple)
            // En PROD: Faire bcrypt.VerifyHashedPassword(hashedPassword, request.Password)

            // Créer ou récupérer l'utilisateur
            var user = await _userService.GetOrCreateUserAsync(request.Email, request.Email, request.Email.Split("@")[0]);

            if (user == null)
            {
                _logger.LogWarning("Failed to create/retrieve user: {Email}", request.Email);
                return StatusCode(500, new { error = "Erreur lors de la création du compte" });
            }

            // Créer les claims pour le cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name ?? user.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
            };

            // Créer le cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return Ok(new
            {
                userId = user.UserId,
                email = user.Email,
                name = user.Name,
                isAuthenticated = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, new { error = "Erreur serveur lors de la connexion" });
        }
    }

    /// <summary>
    /// POST /api/auth/logout - Déconnecter l'utilisateur
    /// Supprime le cookie de session
    /// </summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        try
        {
            _logger.LogDebug("User logout - IsAuthenticated before: {IsAuthenticated}", User.Identity?.IsAuthenticated);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogDebug("User logout - IsAuthenticated after: {IsAuthenticated}", User.Identity?.IsAuthenticated);
            _logger.LogInformation("User logged out successfully");

            return Ok(new { message = "Déconnexion réussie" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { error = "Erreur lors de la déconnexion" });
        }
    }

    /// <summary>
    /// GET /api/auth/me - Récupérer les infos de l'utilisateur connecté
    /// Retourne isAuthenticated=false si pas connecté (sans erreur)
    /// </summary>
    [HttpGet("me")]
    [AllowAnonymous]
    public IActionResult GetCurrentUser()
    {
        // Si pas authentifié, retourner 200 avec isAuthenticated=false
        if (User.Identity?.IsAuthenticated != true)
        {
            _logger.LogDebug("GetCurrentUser - User not authenticated");
            return Ok(new
            {
                userId = (string?)null,
                email = (string?)null,
                name = (string?)null,
                isAuthenticated = false
            });
        }

        // Récupérer les claims du cookie
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;

        _logger.LogDebug("GetCurrentUser - UserId: {UserId}, Email: {Email}", userId, email);

        return Ok(new
        {
            userId,
            email,
            name,
            isAuthenticated = true
        });
    }

    /// <summary>
    /// GET /api/auth/health - Health check public
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
}

/// <summary>
/// DTO pour la requête de login
/// </summary>
public class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
