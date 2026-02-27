using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using menuMalin.Server.Services;
using menuMalin.Shared.Models.Requests;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur pour la gestion des recettes utilisateur
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserRecipesController : ControllerBase
{
    private readonly IUserRecipeService _userRecipeService;

    /// <summary>
    /// Initialise une nouvelle instance de UserRecipesController
    /// </summary>
    /// <param name="userRecipeService">Le service de gestion des recettes utilisateur</param>
    public UserRecipesController(IUserRecipeService userRecipeService)
    {
        _userRecipeService = userRecipeService;
    }

    /// <summary>
    /// Crée une nouvelle recette pour l'utilisateur connecté
    /// </summary>
    /// <param name="request">Les données de la recette à créer</param>
    /// <returns>La recette créée</returns>
    /// <response code="201">Recette créée avec succès</response>
    /// <response code="400">Données invalides</response>
    /// <response code="401">Utilisateur non authentifié</response>
    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] CreateUserRecipeRequest request)
    {
        var userId = ExtractUserId();
        if (userId == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Le titre est requis");

        if (string.IsNullOrWhiteSpace(request.Instructions))
            return BadRequest("Les instructions sont requises");

        try
        {
            var recipe = await _userRecipeService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.UserRecipeId }, recipe);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Récupère toutes les recettes créées par l'utilisateur connecté
    /// </summary>
    /// <returns>Liste des recettes créées par l'utilisateur</returns>
    /// <response code="200">Succès</response>
    /// <response code="401">Utilisateur non authentifié</response>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyRecipes()
    {
        var userId = ExtractUserId();
        if (userId == null)
            return Unauthorized();

        System.Console.WriteLine($"📝 GET /api/userrecipes/my - UserId: {userId}");
        var recipes = await _userRecipeService.GetMyRecipesAsync(userId);
        System.Console.WriteLine($"📝 Recettes trouvées: {recipes.Count()}");
        foreach (var recipe in recipes)
        {
            System.Console.WriteLine($"   - ID: {recipe.UserRecipeId}, Title: {recipe.Title}, UserId: {recipe.UserId}");
        }
        return Ok(recipes);
    }

    /// <summary>
    /// Récupère toutes les recettes publiques de la communauté
    /// </summary>
    /// <returns>Liste des recettes publiques</returns>
    /// <response code="200">Succès</response>
    /// <response code="401">Utilisateur non authentifié</response>
    [HttpGet("public")]
    public async Task<IActionResult> GetPublicRecipes()
    {
        var recipes = await _userRecipeService.GetPublicRecipesAsync();
        return Ok(recipes);
    }

    /// <summary>
    /// Récupère les détails d'une recette spécifique
    /// </summary>
    /// <param name="id">L'ID de la recette</param>
    /// <returns>Les détails de la recette</returns>
    /// <response code="200">Succès</response>
    /// <response code="404">Recette non trouvée</response>
    /// <response code="401">Utilisateur non authentifié</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipe(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("L'ID de la recette est requis");

        var recipe = await _userRecipeService.GetByIdAsync(id);
        if (recipe == null)
            return NotFound();

        return Ok(recipe);
    }

    /// <summary>
    /// Supprime une recette (seul le propriétaire peut la supprimer)
    /// </summary>
    /// <param name="id">L'ID de la recette à supprimer</param>
    /// <returns>Message de confirmation de suppression</returns>
    /// <response code="200">Suppression réussie</response>
    /// <response code="404">Recette non trouvée</response>
    /// <response code="403">Non autorisé (pas propriétaire)</response>
    /// <response code="401">Utilisateur non authentifié</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRecipe(string id)
    {
        var userId = ExtractUserId();
        if (userId == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("L'ID de la recette est requis");

        try
        {
            System.Console.WriteLine($"🗑️ DELETE /api/userrecipes/{id} - UserId: {userId}");
            var result = await _userRecipeService.DeleteAsync(id, userId);
            if (!result)
            {
                System.Console.WriteLine($"❌ Recette non trouvée ou erreur lors de la suppression");
                return NotFound();
            }

            System.Console.WriteLine($"✅ Recette supprimée avec succès");
            return Ok(new { message = "Recette supprimée avec succès" });
        }
        catch (UnauthorizedAccessException ex)
        {
            System.Console.WriteLine($"❌ Accès non autorisé: {ex.Message}");
            return Forbid();  // 403 Forbidden
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Erreur lors de la suppression: {ex.Message}");
            System.Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = $"Erreur serveur: {ex.Message}" });
        }
    }

    /// <summary>
    /// Met à jour une recette (seul le propriétaire peut la modifier)
    /// </summary>
    /// <param name="id">L'ID de la recette</param>
    /// <param name="request">Les nouvelles données de la recette</param>
    /// <returns>La recette mise à jour</returns>
    /// <response code="200">Mise à jour réussie</response>
    /// <response code="404">Recette non trouvée</response>
    /// <response code="400">Données invalides</response>
    /// <response code="403">Non autorisé (pas propriétaire)</response>
    /// <response code="401">Utilisateur non authentifié</response>
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateRecipe(string id, [FromBody] CreateUserRecipeRequest request)
    {
        var userId = ExtractUserId();
        if (userId == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("L'ID de la recette est requis");

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Le titre est requis");

        if (string.IsNullOrWhiteSpace(request.Instructions))
            return BadRequest("Les instructions sont requises");

        try
        {
            var recipe = await _userRecipeService.UpdateAsync(id, userId, request);
            if (recipe == null)
                return NotFound();

            return Ok(recipe);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();  // 403 Forbidden
        }
    }

    /// <summary>
    /// Bascule la visibilité (public/privé) d'une recette
    /// </summary>
    /// <param name="id">L'ID de la recette</param>
    /// <param name="request">Contient la nouvelle visibilité</param>
    /// <returns>Message de confirmation du changement</returns>
    /// <response code="200">Changement réussi</response>
    /// <response code="404">Recette non trouvée</response>
    /// <response code="403">Non autorisé (pas propriétaire)</response>
    /// <response code="401">Utilisateur non authentifié</response>
    [HttpPatch("{id}/visibility")]
    public async Task<IActionResult> UpdateVisibility(string id, [FromBody] UpdateVisibilityRequest request)
    {
        var userId = ExtractUserId();
        if (userId == null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("L'ID de la recette est requis");

        try
        {
            var result = await _userRecipeService.UpdateVisibilityAsync(id, userId, request.IsPublic);
            if (!result)
                return NotFound();

            return Ok(new { message = $"Visibilité mise à jour. La recette est maintenant {(request.IsPublic ? "publique" : "privée")}" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();  // 403 Forbidden
        }
    }


    /// <summary>
    /// Extrait l'ID utilisateur depuis les Claims de l'authentification
    /// </summary>
    /// <returns>L'ID utilisateur ou null si non trouvé</returns>
    private string? ExtractUserId()
    {
        return User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

/// <summary>
/// Model pour mettre à jour la visibilité d'une recette
/// </summary>
public class UpdateVisibilityRequest
{
    /// <summary>
    /// Nouvelle valeur de visibilité (true = public, false = privé)
    /// </summary>
    public bool IsPublic { get; set; }
}
