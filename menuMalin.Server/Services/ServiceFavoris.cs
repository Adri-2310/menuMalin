using menuMalin.Server.Modeles.Entites;
using menuMalin.Server.Depots;
using menuMalin.Server.Depots.Interfaces;
using menuMalin.Shared.Modeles.DTOs;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour les favoris
/// </summary>
/// <summary>
/// Service pour la gestion des favoris utilisateur
/// </summary>
public class ServiceFavoris : IServiceFavoris
{
    private readonly IDepotFavori _favoriteRepository;
    private readonly IServiceRecette _serviceRecetteService;
    private readonly IDepotRecette _recipeRepository;
    private readonly IServiceMealDB _theMealDBService;

    /// <summary>
    /// Initialise une nouvelle instance de FavoriteService
    /// </summary>
    /// <param name="favoriteRepository">Le repository des favoris</param>
    /// <param name="recipeService">Le service des recettes</param>
    /// <param name="recipeRepository">Le repository des recettes</param>
    /// <param name="theMealDBService">Le service TheMealDB pour récupérer les données externes</param>
    public ServiceFavoris(IDepotFavori favoriteRepository, IServiceRecette recipeService, IDepotRecette recipeRepository, IServiceMealDB theMealDBService)
    {
        _favoriteRepository = favoriteRepository;
        _serviceRecetteService = recipeService;
        _recipeRepository = recipeRepository;
        _theMealDBService = theMealDBService;
    }

    /// <summary>
    /// Ajoute une recette aux favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <param name="recipeId">L'ID de la recette à ajouter</param>
    /// <returns>Le RecetteDTO ajouté aux favoris</returns>
    /// <exception cref="InvalidOperationException">Levée si la recette n'existe pas</exception>
    public async Task<RecetteDTO> AddFavoriteAsync(string userId, string recipeId)
    {
        // Chercher si la recette existe déjà par MealDBId (recipeId est le MealDBId)
        var recipeExists = await _recipeRepository.GetByMealDbIdAsync(recipeId);

        // Si elle n'existe pas, essayer de la récupérer depuis TheMealDB et la créer
        if (recipeExists == null)
        {
            var theMealData = await _theMealDBService.GetByIdAsync(recipeId);
            if (theMealData != null)
            {
                try
                {
                    // Créer la recette complète avec les données de TheMealDB
                    await _serviceRecetteService.CreateOrUpdateRecipeAsync(theMealData);
                    // Récupérer la recette fraîchement créée depuis la BD
                    recipeExists = await _recipeRepository.GetByMealDbIdAsync(recipeId);
                }
                catch (Exception ex) when (ex.InnerException?.Message.Contains("Duplicate") ?? false)
                {
                    // Race condition : un autre thread a créé la recette en même temps
                    System.Console.WriteLine($"⚠️ Race condition sur recette {recipeId}, récupération...");
                    recipeExists = await _recipeRepository.GetByMealDbIdAsync(recipeId);
                }
            }
            else
            {
                // Si TheMealDB échoue, lever une exception plutôt que créer un placeholder
                throw new InvalidOperationException(
                    $"La recette TheMealDB {recipeId} n'a pas pu être trouvée. " +
                    "Assurez-vous que l'ID est correct et que l'API TheMealDB est accessible.");
            }
        }

        // À ce stade, recipeExists ne doit jamais être null
        if (recipeExists == null)
        {
            throw new InvalidOperationException(
                $"Impossible de créer ou trouver la recette {recipeId}.");
        }

        // Créer un nouveau favori (utiliser le RecipeId interne de la recette créée)
        var favorite = new Favori
        {
            FavoriteId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            RecipeId = recipeExists.RecipeId,  // Utiliser le RecipeId interne, pas le MealDBId
            DateCreation = DateTime.UtcNow
        };

        try
        {
            await _favoriteRepository.AddAsync(favorite);
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("Duplicate") ?? false)
        {
            // Favori déjà existant - c'est un ajout duplex, juste retourner la recette
            System.Console.WriteLine($"⚠️ Favori déjà existant pour {userId}/{recipeId}");
        }

        // Retourner la recette complète
        return new RecetteDTO
        {
            RecipeId = recipeExists.RecipeId,
            Title = recipeExists.Title ?? string.Empty,
            MealDBId = recipeExists.MealDBId,
            ImageUrl = recipeExists.ImageUrl,
            Category = recipeExists.Category,
            Area = recipeExists.Area,
            Instructions = recipeExists.Instructions ?? string.Empty
        };
    }

    /// <summary>
    /// Supprime une recette des favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <param name="recipeId">L'ID de la recette à supprimer</param>
    /// <returns>true si la suppression a réussi, false sinon</returns>
    public async Task<bool> RemoveFavoriteAsync(string userId, string recipeId)
    {
        return await _favoriteRepository.RemoveByUserAndRecipeAsync(userId, recipeId);
    }

    /// <summary>
    /// Récupère tous les favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <returns>Une collection de RecetteDTO correspondant aux favoris de l'utilisateur</returns>
    public async Task<IEnumerable<RecetteDTO>> GetUserFavorisAsync(string userId)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId);
        var recipes = new List<RecetteDTO>();
        var placeholderIds = new List<string>(); // IDs des placeholders à mettre à jour

        // Première passe : traiter les recettes chargées (pas de N+1, elles sont incluses via Include)
        foreach (var favorite in favorites)
        {
            var recipe = favorite.Recette;
            if (recipe == null)
                continue;

            // Vérifier si c'est une recette placeholder
            bool isPlaceholder = string.IsNullOrEmpty(recipe.ImageUrl)
                || string.IsNullOrEmpty(recipe.Category)
                || (recipe.Title?.StartsWith("Recette #") == true);

            if (!isPlaceholder)
            {
                recipes.Add(new RecetteDTO
                {
                    RecipeId = recipe.RecipeId,
                    MealDBId = recipe.MealDBId,
                    Title = recipe.Title ?? string.Empty,
                    Category = recipe.Category ?? string.Empty,
                    ImageUrl = recipe.ImageUrl ?? string.Empty,
                    Area = recipe.Area ?? string.Empty,
                    Instructions = recipe.Instructions ?? string.Empty
                });
            }
            else if (!string.IsNullOrEmpty(recipe.MealDBId))
            {
                placeholderIds.Add(recipe.MealDBId);
            }
        }

        // Deuxième passe : mettre à jour les placeholders en batch
        foreach (var mealDbId in placeholderIds)
        {
            try
            {
                var theMealData = await _theMealDBService.GetByIdAsync(mealDbId);
                if (theMealData != null)
                {
                    var updatedRecipe = await _serviceRecetteService.CreateOrUpdateRecipeAsync(theMealData);
                    recipes.Add(updatedRecipe);
                }
                else
                {
                    // Si TheMealDB retourne null, afficher un placeholder avec message
                    recipes.Add(new RecetteDTO
                    {
                        RecipeId = mealDbId,
                        MealDBId = mealDbId,
                        Title = $"[Données indisponibles]",
                        Category = "Indisponible",
                        ImageUrl = "/images/placeholder-unavailable.png",
                        Area = string.Empty,
                        Instructions = "Cette recette n'est temporairement pas disponible. L'API TheMealDB est peut-être down."
                    });
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"⚠️ Erreur mise à jour placeholder {mealDbId}: {ex.Message}");
                // Afficher un placeholder en cas d'erreur au lieu de filtrer
                recipes.Add(new RecetteDTO
                {
                    RecipeId = mealDbId,
                    MealDBId = mealDbId,
                    Title = $"[Erreur de chargement]",
                    Category = "Erreur",
                    ImageUrl = "/images/placeholder-error.png",
                    Area = string.Empty,
                    Instructions = $"Erreur lors du chargement: {ex.Message}"
                });
            }
        }

        return recipes;
    }

    /// <summary>
    /// Vérifie si une recette est dans les favoris d'un utilisateur
    /// </summary>
    /// <param name="userId">L'ID de l'utilisateur</param>
    /// <param name="recipeId">L'ID de la recette</param>
    /// <returns>true si la recette est dans les favoris, false sinon</returns>
    public async Task<bool> IsFavoriteAsync(string userId, string recipeId)
    {
        return await _favoriteRepository.IsFavoriteAsync(userId, recipeId);
    }
}
