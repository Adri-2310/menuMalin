using System.Text.Json;
using menuMalin.Server.Models.Entities;
using menuMalin.Server.Repositories;
using menuMalin.Shared.Models.Dtos;
using menuMalin.Shared.Models.Requests;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour la gestion des recettes utilisateur
/// </summary>
public class UserRecipeService : IUserRecipeService
{
    private readonly IUserRecipeRepository _userRecipeRepository;

    /// <summary>
    /// Initialise une nouvelle instance de UserRecipeService
    /// </summary>
    /// <param name="userRecipeRepository">Le repository des recettes utilisateur</param>
    public UserRecipeService(IUserRecipeRepository userRecipeRepository)
    {
        _userRecipeRepository = userRecipeRepository;
    }

    public async Task<UserRecipeDto> CreateAsync(string userId, CreateUserRecipeRequest request)
    {
        // Valider les données requises
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Le titre est requis");
        if (string.IsNullOrWhiteSpace(request.Instructions))
            throw new ArgumentException("Les instructions sont requises");

        // Sérialiser les ingrédients en JSON
        var ingredientsJson = JsonSerializer.Serialize(request.Ingredients ?? new List<string>());

        // Créer la nouvelle recette
        var userRecipe = new UserRecipe
        {
            UserRecipeId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            Title = request.Title,
            Category = request.Category,
            Area = request.Area,
            Instructions = request.Instructions,
            ImageUrl = request.ImageUrl,
            IngredientsJson = ingredientsJson,
            IsPublic = request.IsPublic,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Ajouter à la base de données
        var created = await _userRecipeRepository.AddAsync(userRecipe);

        // Convertir en DTO et retourner
        return MapToDto(created);
    }

    public async Task<IEnumerable<UserRecipeDto>> GetMyRecipesAsync(string userId)
    {
        var recipes = await _userRecipeRepository.GetByUserIdAsync(userId);
        return recipes.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<UserRecipeDto>> GetPublicRecipesAsync()
    {
        var recipes = await _userRecipeRepository.GetPublicAsync();
        return recipes.Select(MapToDto).ToList();
    }

    public async Task<UserRecipeDto?> GetByIdAsync(string userRecipeId)
    {
        var recipe = await _userRecipeRepository.GetByIdAsync(userRecipeId);
        return recipe != null ? MapToDto(recipe) : null;
    }

    public async Task<bool> DeleteAsync(string userRecipeId, string userId)
    {
        // Récupérer la recette pour vérifier le propriétaire
        var recipe = await _userRecipeRepository.GetByIdAsync(userRecipeId);
        if (recipe == null)
        {
            System.Console.WriteLine($"❌ Recette {userRecipeId} non trouvée");
            return false;
        }

        // Vérifier que l'utilisateur est le propriétaire
        if (recipe.UserId != userId)
        {
            System.Console.WriteLine($"❌ Propriétaire invalide. Recipe.UserId: {recipe.UserId}, UserId: {userId}");
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à supprimer cette recette");
        }

        // Supprimer la recette
        System.Console.WriteLine($"🗑️ Suppression de la recette {userRecipeId}");
        var result = await _userRecipeRepository.DeleteAsync(userRecipeId);
        System.Console.WriteLine($"✅ Recette supprimée: {result}");
        return result;
    }

    public async Task<bool> UpdateVisibilityAsync(string userRecipeId, string userId, bool isPublic)
    {
        // Récupérer la recette pour vérifier le propriétaire
        var recipe = await _userRecipeRepository.GetByIdAsync(userRecipeId);
        if (recipe == null)
            return false;

        // Vérifier que l'utilisateur est le propriétaire
        if (recipe.UserId != userId)
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à modifier cette recette");

        // Mettre à jour la visibilité
        return await _userRecipeRepository.UpdateVisibilityAsync(userRecipeId, isPublic);
    }

    /// <summary>
    /// Convertit une entité UserRecipe en DTO UserRecipeDto
    /// </summary>
    /// <param name="userRecipe">L'entité à convertir</param>
    /// <returns>Le DTO correspondant</returns>
    private static UserRecipeDto MapToDto(UserRecipe userRecipe)
    {
        // Désérialiser les ingrédients depuis le JSON
        var ingredients = new List<string>();
        try
        {
            if (!string.IsNullOrEmpty(userRecipe.IngredientsJson))
            {
                ingredients = JsonSerializer.Deserialize<List<string>>(userRecipe.IngredientsJson) ?? new List<string>();
            }
        }
        catch
        {
            // Si la désérialisation échoue, retourner une liste vide
            ingredients = new List<string>();
        }

        return new UserRecipeDto
        {
            UserRecipeId = userRecipe.UserRecipeId,
            UserId = userRecipe.UserId,
            Title = userRecipe.Title,
            Category = userRecipe.Category,
            Area = userRecipe.Area,
            Instructions = userRecipe.Instructions,
            ImageUrl = userRecipe.ImageUrl,
            Ingredients = ingredients,
            IsPublic = userRecipe.IsPublic,
            CreatedAt = userRecipe.CreatedAt
        };
    }
}
