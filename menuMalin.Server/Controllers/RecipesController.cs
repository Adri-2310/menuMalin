using Microsoft.AspNetCore.Mvc;
using menuMalin.Server.Services;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur pour les recettes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly ITheMealDBService _mealDbService;
    private readonly IRecipeService _recipeService;

    public RecipesController(ITheMealDBService mealDbService, IRecipeService recipeService)
    {
        _mealDbService = mealDbService;
        _recipeService = recipeService;
    }

    /// <summary>
    /// Récupère 6 recettes aléatoires
    /// </summary>
    [HttpGet("random")]
    public async Task<IActionResult> GetRandomRecipes()
    {
        var recipes = new List<object>();

        for (int i = 0; i < 6; i++)
        {
            var meal = await _mealDbService.GetRandomAsync();
            if (meal != null)
            {
                // Créer ou mettre à jour en cache
                var recipe = await _recipeService.CreateOrUpdateRecipeAsync(meal);
                recipes.Add(recipe);
            }
        }

        return Ok(recipes);
    }

    /// <summary>
    /// Recherche des recettes par nom
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchRecipes([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("La requête de recherche ne peut pas être vide");

        var meals = await _mealDbService.SearchByNameAsync(query);
        var recipes = new List<object>();

        foreach (var meal in meals)
        {
            var recipe = await _recipeService.CreateOrUpdateRecipeAsync(meal);
            recipes.Add(recipe);
        }

        return Ok(recipes);
    }

    /// <summary>
    /// Récupère les détails d'une recette
    /// </summary>
    [HttpGet("{mealId}")]
    public async Task<IActionResult> GetRecipeDetails(string mealId)
    {
        if (string.IsNullOrWhiteSpace(mealId))
            return BadRequest("L'ID de la recette ne peut pas être vide");

        var meal = await _mealDbService.GetByIdAsync(mealId);
        if (meal == null)
            return NotFound();

        var recipe = await _recipeService.CreateOrUpdateRecipeAsync(meal);
        return Ok(recipe);
    }

    /// <summary>
    /// Récupère toutes les catégories
    /// </summary>
    [HttpGet("categories/list")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _mealDbService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Récupère toutes les zones/cuisines
    /// </summary>
    [HttpGet("areas/list")]
    public async Task<IActionResult> GetAreas()
    {
        var areas = await _mealDbService.GetAreasAsync();
        return Ok(areas);
    }

    /// <summary>
    /// Filtre les recettes par catégorie
    /// </summary>
    [HttpGet("filter/category")]
    public async Task<IActionResult> FilterByCategory([FromQuery] string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return BadRequest("La catégorie ne peut pas être vide");

        var meals = await _mealDbService.FilterByCategoryAsync(category);
        var recipes = new List<object>();

        foreach (var meal in meals)
        {
            var recipe = await _recipeService.CreateOrUpdateRecipeAsync(meal);
            recipes.Add(recipe);
        }

        return Ok(recipes);
    }

    /// <summary>
    /// Filtre les recettes par zone/cuisine
    /// </summary>
    [HttpGet("filter/area")]
    public async Task<IActionResult> FilterByArea([FromQuery] string area)
    {
        if (string.IsNullOrWhiteSpace(area))
            return BadRequest("La zone ne peut pas être vide");

        var meals = await _mealDbService.FilterByAreaAsync(area);
        var recipes = new List<object>();

        foreach (var meal in meals)
        {
            var recipe = await _recipeService.CreateOrUpdateRecipeAsync(meal);
            recipes.Add(recipe);
        }

        return Ok(recipes);
    }
}
