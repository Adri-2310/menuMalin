using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using menuMalin.Server.Services;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur pour les favoris
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    /// <summary>
    /// Initialise une nouvelle instance de FavoritesController
    /// </summary>
    /// <param name="favoriteService">Le service de gestion des favoris</param>
    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    /// <summary>
    /// Récupère tous les favoris de l'utilisateur connecté
    /// </summary>
    /// <returns>Liste complète des favoris de l'utilisateur</returns>
    [HttpGet]
    public async Task<IActionResult> GetUserFavorites()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
        return Ok(favorites);
    }

    /// <summary>
    /// Ajoute une recette aux favoris de l'utilisateur connecté
    /// </summary>
    /// <param name="request">Contient l'ID de la recette à ajouter</param>
    /// <returns>La recette ajoutée aux favoris</returns>
    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.RecipeId))
            return BadRequest("L'ID de la recette est requis");

        var recipe = await _favoriteService.AddFavoriteAsync(userId, request.RecipeId);
        return Ok(recipe);
    }

    /// <summary>
    /// Supprime une recette des favoris de l'utilisateur connecté
    /// </summary>
    /// <param name="recipeId">L'ID de la recette à supprimer</param>
    /// <returns>Message de confirmation de suppression</returns>
    [HttpDelete("{recipeId}")]
    public async Task<IActionResult> RemoveFavorite(string recipeId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _favoriteService.RemoveFavoriteAsync(userId, recipeId);
        if (!result)
            return NotFound();

        return Ok(new { message = "Favori supprimé avec succès" });
    }

    /// <summary>
    /// Vérifie si une recette figure dans les favoris de l'utilisateur connecté
    /// </summary>
    /// <param name="recipeId">L'ID de la recette à vérifier</param>
    /// <returns>Objet JSON contenant le booléen 'isFavorite'</returns>
    [HttpGet("{recipeId}/exists")]
    public async Task<IActionResult> IsFavorite(string recipeId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var isFavorite = await _favoriteService.IsFavoriteAsync(userId, recipeId);
        return Ok(new { isFavorite });
    }
}

/// <summary>
/// Model pour ajouter un favori
/// </summary>
public class AddFavoriteRequest
{
    public string RecipeId { get; set; } = string.Empty;
}
