using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using menuMalin.Server.Services;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Controleurs;

/// <summary>
/// Contrôleur pour les favoris
/// Toutes les routes nécessitent une authentification cookie valide
/// </summary>
[ApiController]
[Route("api/favorites")]
public class ControleurFavoris : ControllerBase
{
    private readonly IServiceFavoris _favoriteService;
    private readonly IServiceUtilisateur _serviceUtilisateurService;
    private readonly ILogger<ControleurFavoris> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de FavoritesController
    /// </summary>
    /// <param name="favoriteService">Le service de gestion des favoris</param>
    /// <param name="userService">Le service de gestion des utilisateurs</param>
    /// <param name="logger">Service de logging</param>
    public ControleurFavoris(IServiceFavoris favoriteService, IServiceUtilisateur userService, ILogger<ControleurFavoris> logger)
    {
        _favoriteService = favoriteService;
        _serviceUtilisateurService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les favoris de l'utilisateur connecté
    /// </summary>
    /// <returns>Liste complète des favoris de l'utilisateur</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserFavoris()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Impossible d'extraire l'ID utilisateur des claims");
            return Unauthorized();
        }

        _logger.LogInformation("Récupération des favoris pour l'utilisateur: {UserId}", userId);

        try
        {
            var favorites = await _favoriteService.GetUserFavorisAsync(userId);
            return Ok(favorites);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des favoris pour {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Ajoute une recette aux favoris de l'utilisateur connecté
    /// </summary>
    /// <param name="request">Contient l'ID de la recette à ajouter</param>
    /// <returns>La recette ajoutée aux favoris</returns>
    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Impossible d'extraire l'ID utilisateur pour ajouter un favori");
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.RecipeId))
            return BadRequest("L'ID de la recette est requis");

        _logger.LogInformation("Ajout du favori {RecipeId} pour l'utilisateur {UserId}", request.RecipeId, userId);

        try
        {
            var recipe = await _favoriteService.AddFavoriteAsync(userId, request.RecipeId);
            return Ok(recipe);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erreur lors de l'ajout du favori");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur serveur lors de l'ajout du favori");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Supprime une recette des favoris de l'utilisateur connecté
    /// </summary>
    /// <param name="recipeId">L'ID de la recette à supprimer</param>
    /// <returns>Message de confirmation de suppression</returns>
    [HttpDelete("{recipeId}")]
    public async Task<IActionResult> RemoveFavorite(string recipeId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Impossible d'extraire l'ID utilisateur pour supprimer un favori");
            return Unauthorized();
        }

        _logger.LogInformation("Suppression du favori {RecipeId} pour l'utilisateur {UserId}", recipeId, userId);

        try
        {
            var result = await _favoriteService.RemoveFavoriteAsync(userId, recipeId);
            if (!result)
                return NotFound();

            return Ok(new { message = "Favori supprimé avec succès" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du favori");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Vérifie si une recette figure dans les favoris de l'utilisateur connecté
    /// </summary>
    /// <param name="recipeId">L'ID de la recette à vérifier</param>
    /// <returns>Objet JSON contenant le booléen 'isFavorite'</returns>
    [HttpGet("{recipeId}/exists")]
    public async Task<IActionResult> IsFavorite(string recipeId)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Impossible d'extraire l'ID utilisateur pour vérifier un favori");
            return Unauthorized();
        }

        try
        {
            var isFavorite = await _favoriteService.IsFavoriteAsync(userId, recipeId);
            return Ok(new { isFavorite });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification du favori");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Extrait l'ID utilisateur des claims du cookie d'authentification
    /// </summary>
    /// <returns>L'ID utilisateur ou null si absent</returns>
    private string? GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Aucun NameIdentifier trouvé dans les claims");
        }

        return userId;
    }
}

/// <summary>
/// Model pour ajouter un favori
/// </summary>
public class AddFavoriteRequest
{
    public string RecipeId { get; set; } = string.Empty;
}
