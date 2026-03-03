using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using menuMalin.Server.DTOs;
using menuMalin.Server.Depots;
using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Services;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Controleurs;

/// <summary>
/// Contrôleur d'authentification BFF (Backend for Frontend)
/// Authentification simple: Email + Password → Cookie HttpOnly
/// </summary>
[ApiController]
[Route("api/auth")]
public class ControleurAuthentification : ControllerBase
{
    private readonly IServiceUtilisateur _serviceUtilisateur;
    private readonly ILogger<ControleurAuthentification> _logger;

    public ControleurAuthentification(IServiceUtilisateur serviceUtilisateur, ILogger<ControleurAuthentification> logger)
    {
        _serviceUtilisateur = serviceUtilisateur;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/ControleurAuthentification/login - Authentifier avec email + password
    /// Retourne un cookie HttpOnly si authentification réussie
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] RequeteConnexion request)
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
            // Récupérer l'utilisateur par email
            var user = await _serviceUtilisateur.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
                return BadRequest(new { error = "Email ou mot de passe incorrect" });
            }

            // Vérifier le password avec BCrypt
            if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed - incorrect password for user: {Email}", request.Email);
                return BadRequest(new { error = "Email ou mot de passe incorrect" });
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
                IsPersistent = true
            };

            // Créer le cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Utilisateur logged in successfully: {Email}", request.Email);

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
    /// POST /api/ControleurAuthentification/register - Créer un nouveau compte
    /// Retourne un cookie HttpOnly si création réussie
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RequeteInscription request)
    {
        // Validation des entrées
        if (string.IsNullOrWhiteSpace(request?.Email) || string.IsNullOrWhiteSpace(request?.Password) || string.IsNullOrWhiteSpace(request?.Name))
        {
            _logger.LogWarning("Register attempt with missing required fields");
            return BadRequest(new { message = "Email, nom et mot de passe sont requis" });
        }

        // Validation du format email
        if (!request.Email.Contains("@"))
        {
            return BadRequest(new { message = "Format email invalide" });
        }

        // Validation de la longueur du mot de passe
        if (request.Password.Length < 8)
        {
            return BadRequest(new { message = "Le mot de passe doit contenir au moins 8 caractères" });
        }

        try
        {
            _logger.LogInformation("Register attempt for email: {Email}", request.Email);

            // Vérifier si l'utilisateur existe déjà
            var existingUser = await _serviceUtilisateur.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
                return BadRequest(new { message = "Cet email est déjà utilisé" });
            }

            // Créer le nouvel utilisateur
            var user = await _serviceUtilisateur.CreateUserAsync(request.Email, request.Password, request.Name);

            if (user == null)
            {
                _logger.LogWarning("Failed to create user: {Email}", request.Email);
                return StatusCode(500, new { message = "Erreur lors de la création du compte" });
            }

            _logger.LogInformation("Utilisateur created successfully - UserId: {UserId}, Email: {Email}", user.UserId, user.Email);

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
                IsPersistent = true
            };

            // Créer le cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Utilisateur registered and logged in successfully: {Email}", request.Email);

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
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, new { message = $"Erreur serveur: {ex.Message}" });
        }
    }

    /// <summary>
    /// POST /api/ControleurAuthentification/logout - Déconnecter l'utilisateur
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
            _logger.LogInformation("Utilisateur logged out successfully");

            return Ok(new { message = "Déconnexion réussie" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { error = "Erreur lors de la déconnexion" });
        }
    }

    /// <summary>
    /// GET /api/ControleurAuthentification/me - Récupérer les infos de l'utilisateur connecté
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
    /// GET /api/ControleurAuthentification/health - Health check public
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
}
