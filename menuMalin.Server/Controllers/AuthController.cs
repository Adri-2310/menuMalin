using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur d'authentification pour Auth0
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Endpoint de test pour vérifier que l'utilisateur est connecté
    /// </summary>
    /// <returns>Les informations de l'utilisateur connecté</returns>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var sub = User.FindFirst("sub")?.Value;

        return Ok(new
        {
            userId,
            email,
            name,
            sub,
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    /// <summary>
    /// Endpoint public pour vérifier que l'API est accessible
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "ok", timestamp = DateTime.UtcNow });
    }
}
