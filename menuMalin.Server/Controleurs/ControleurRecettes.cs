using Microsoft.AspNetCore.Mvc;
using menuMalin.Server.Services;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Controleurs;

/// <summary>
/// Contrôleur pour les recettes
/// </summary>
[ApiController]
[Route("api/recipes")]
public class ControleurRecettes : ControllerBase
{
    private readonly IServiceMealDB _serviceMealDBService;
    private readonly IServiceRecette _serviceRecetteService;
    private readonly ILogger<ControleurRecettes> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de RecipesController
    /// </summary>
    /// <param name="mealDbService">Le service TheMealDB pour les données externes</param>
    /// <param name="recipeService">Le service de gestion des recettes</param>
    /// <param name="logger">Le service de logging</param>
    public ControleurRecettes(IServiceMealDB mealDbService, IServiceRecette recipeService, ILogger<ControleurRecettes> logger)
    {
        _serviceMealDBService = mealDbService;
        _serviceRecetteService = recipeService;
        _logger = logger;
    }

    /// <summary>
    /// Récupère des recettes aléatoires
    /// </summary>
    /// <param name="count">Nombre de recettes à récupérer (par défaut 6)</param>
    /// <returns>Liste de recettes aléatoires</returns>
    [HttpGet("random")]
    public async Task<IActionResult> GetRandomRecipes(int count = 6)
    {
        try
        {
            var recipes = new List<dynamic>();
            var mealDbIds = new HashSet<string>(); // Pour éviter les doublons
            int attempts = 0;
            int maxAttempts = count * 3; // Limiter les tentatives pour éviter une boucle infinie

            while (recipes.Count < count && attempts < maxAttempts)
            {
                attempts++;
                var meal = await _serviceMealDBService.GetRandomAsync();

                if (meal != null && !mealDbIds.Contains(meal.IdMeal))
                {
                    mealDbIds.Add(meal.IdMeal);

                    // Créer ou mettre à jour en cache
                    var recipe = await _serviceRecetteService.CreateOrUpdateRecipeAsync(meal);
                    recipes.Add(new
                    {
                        IdMeal = recipe.MealDBId,
                        StrMeal = recipe.Title,
                        StrMealThumb = recipe.ImageUrl,
                        StrCategory = recipe.Category,
                        StrArea = recipe.Area,
                        StrInstructions = recipe.Instructions,
                        StrTags = recipe.Tags,
                        RecipeId = recipe.RecipeId,
                        MealDBId = recipe.MealDBId
                    });
                }
            }

            return Ok(recipes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des recettes aléatoires");
            return StatusCode(503, new { message = "Service indisponible. Veuillez réessayer plus tard." });
        }
    }

    /// <summary>
    /// Recherche des recettes par nom
    /// </summary>
    /// <param name="query">Le terme de recherche</param>
    /// <returns>Liste des recettes correspondant au terme de recherche</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchRecipes([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("La requête de recherche ne peut pas être vide");

            var meals = await _serviceMealDBService.SearchByNameAsync(query);
            var recipes = new List<dynamic>();

            foreach (var meal in meals)
            {
                var recipe = await _serviceRecetteService.CreateOrUpdateRecipeAsync(meal);
                recipes.Add(new
                {
                    IdMeal = recipe.MealDBId,
                    StrMeal = recipe.Title,
                    StrMealThumb = recipe.ImageUrl,
                    StrCategory = recipe.Category,
                    StrArea = recipe.Area,
                    StrInstructions = recipe.Instructions,
                    StrTags = recipe.Tags,
                    RecipeId = recipe.RecipeId,
                    MealDBId = recipe.MealDBId
                });
            }

            return Ok(recipes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la recherche de recettes avec la requête: {Query}", query);
            return StatusCode(503, new { message = "Service indisponible. Veuillez réessayer plus tard." });
        }
    }

    /// <summary>
    /// Récupère les détails complets d'une recette
    /// </summary>
    /// <param name="mealId">L'ID TheMealDB de la recette</param>
    /// <returns>Les détails complets de la recette</returns>
    [HttpGet("{mealId}")]
    public async Task<IActionResult> GetRecipeDetails(string mealId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(mealId))
                return BadRequest("L'ID de la recette ne peut pas être vide");

            var meal = await _serviceMealDBService.GetByIdAsync(mealId);
            if (meal == null)
                return NotFound();

            var recipe = await _serviceRecetteService.CreateOrUpdateRecipeAsync(meal);
            return Ok(recipe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des détails de la recette: {MealId}", mealId);
            return StatusCode(503, new { message = "Service indisponible. Veuillez réessayer plus tard." });
        }
    }

    /// <summary>
    /// Récupère la liste de toutes les catégories disponibles
    /// </summary>
    /// <returns>Liste des noms de catégories</returns>
    [HttpGet("categories/list")]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _serviceMealDBService.GetCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des catégories");
            return StatusCode(503, new { message = "Service indisponible. Veuillez réessayer plus tard." });
        }
    }

    /// <summary>
    /// Récupère la liste de toutes les zones/cuisines disponibles
    /// </summary>
    /// <returns>Liste des noms de zones/cuisines</returns>
    [HttpGet("areas/list")]
    public async Task<IActionResult> GetAreas()
    {
        try
        {
            var areas = await _serviceMealDBService.GetAreasAsync();
            return Ok(areas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des zones");
            return StatusCode(503, new { message = "Service indisponible. Veuillez réessayer plus tard." });
        }
    }

    /// <summary>
    /// Filtre les recettes par catégorie
    /// </summary>
    /// <param name="category">Le nom de la catégorie pour le filtrage</param>
    /// <returns>Liste des recettes de la catégorie spécifiée</returns>
    [HttpGet("filter/category")]
    public async Task<IActionResult> FilterByCategory([FromQuery] string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("La catégorie ne peut pas être vide");

            var meals = await _serviceMealDBService.FilterByCategoryAsync(category);
            var recipes = new List<object>();

            foreach (var meal in meals)
            {
                var recipe = await _serviceRecetteService.CreateOrUpdateRecipeAsync(meal);
                recipes.Add(recipe);
            }

            return Ok(recipes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du filtrage des recettes par catégorie: {Category}", category);
            return StatusCode(503, new { message = "Service indisponible. Veuillez réessayer plus tard." });
        }
    }

    /// <summary>
    /// Filtre les recettes par zone/cuisine
    /// </summary>
    /// <param name="area">Le nom de la zone/cuisine pour le filtrage</param>
    /// <returns>Liste des recettes de la zone/cuisine spécifiée</returns>
    [HttpGet("filter/area")]
    public async Task<IActionResult> FilterByArea([FromQuery] string area)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(area))
                return BadRequest("La zone ne peut pas être vide");

            var meals = await _serviceMealDBService.FilterByAreaAsync(area);
            var recipes = new List<object>();

            foreach (var meal in meals)
            {
                var recipe = await _serviceRecetteService.CreateOrUpdateRecipeAsync(meal);
                recipes.Add(recipe);
            }

            return Ok(recipes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du filtrage des recettes par zone: {Area}", area);
            return StatusCode(503, new { message = "Service indisponible. Veuillez réessayer plus tard." });
        }
    }
}
