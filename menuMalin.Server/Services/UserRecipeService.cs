using System.Text.Json;
using System.Net;
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
    private readonly IWebHostEnvironment _hostEnvironment;
    private const string UploadsFolder = "uploads";

    /// <summary>
    /// Initialise une nouvelle instance de UserRecipeService
    /// </summary>
    /// <param name="userRecipeRepository">Le repository des recettes utilisateur</param>
    /// <param name="hostEnvironment">L'environnement de l'hôte web</param>
    public UserRecipeService(IUserRecipeRepository userRecipeRepository, IWebHostEnvironment hostEnvironment)
    {
        _userRecipeRepository = userRecipeRepository;
        _hostEnvironment = hostEnvironment;
    }

    public async Task<UserRecipeDto> CreateAsync(string userId, CreateUserRecipeRequest request)
    {
        // Valider les données requises
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Le titre est requis");
        if (request.Title.Length < 3)
            throw new ArgumentException("Le titre doit contenir au moins 3 caractères");
        if (request.Title.Length > 200)
            throw new ArgumentException("Le titre ne peut pas dépasser 200 caractères");

        if (string.IsNullOrWhiteSpace(request.Instructions))
            throw new ArgumentException("Les instructions sont requises");
        if (request.Instructions.Length < 10)
            throw new ArgumentException("Les instructions doivent contenir au moins 10 caractères");

        // Valider l'ImageUrl pour éviter les SSRF
        if (!string.IsNullOrEmpty(request.ImageUrl))
        {
            ValidateImageUrl(request.ImageUrl);
        }

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

        // Supprimer l'image du serveur si elle existe et provient du dossier uploads
        if (!string.IsNullOrEmpty(recipe.ImageUrl))
        {
            DeleteRecipeImage(recipe.ImageUrl);
        }

        var result = await _userRecipeRepository.DeleteAsync(userRecipeId);
        System.Console.WriteLine($"✅ Recette supprimée: {result}");
        return result;
    }

    public async Task<UserRecipeDto?> UpdateAsync(string userRecipeId, string userId, CreateUserRecipeRequest request)
    {
        // Valider les données requises
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Le titre est requis");
        if (string.IsNullOrWhiteSpace(request.Instructions))
            throw new ArgumentException("Les instructions sont requises");

        // Récupérer la recette pour vérifier le propriétaire
        var recipe = await _userRecipeRepository.GetByIdAsync(userRecipeId);
        if (recipe == null)
            return null;

        // Vérifier que l'utilisateur est le propriétaire
        if (recipe.UserId != userId)
            throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à modifier cette recette");

        // Sérialiser les ingrédients en JSON
        var ingredientsJson = JsonSerializer.Serialize(request.Ingredients ?? new List<string>());

        // Mettre à jour la recette
        recipe.Title = request.Title;
        recipe.Category = request.Category;
        recipe.Area = request.Area;
        recipe.Instructions = request.Instructions;
        recipe.IngredientsJson = ingredientsJson;
        recipe.IsPublic = request.IsPublic;
        recipe.UpdatedAt = DateTime.UtcNow;

        var updated = await _userRecipeRepository.UpdateAsync(recipe);
        return updated != null ? MapToDto(updated) : null;
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
    /// Supprime l'image associée à une recette du serveur
    /// </summary>
    /// <param name="imageUrl">L'URL relative de l'image (ex: /uploads/guid_filename.jpg)</param>
    private void DeleteRecipeImage(string imageUrl)
    {
        try
        {
            // Vérifier que l'image provient du dossier uploads interne
            if (!imageUrl.StartsWith($"/{UploadsFolder}/"))
            {
                System.Console.WriteLine($"⚠️ Image externe, non suppression: {imageUrl}");
                return;
            }

            // Construire le chemin du fichier
            var fileName = imageUrl.Replace($"/{UploadsFolder}/", "");
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, UploadsFolder, fileName);

            // Vérifier que le fichier existe
            if (!File.Exists(filePath))
            {
                System.Console.WriteLine($"⚠️ Fichier image non trouvé: {filePath}");
                return;
            }

            // Supprimer le fichier
            File.Delete(filePath);
            System.Console.WriteLine($"✅ Image supprimée: {filePath}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Erreur lors de la suppression de l'image: {ex.Message}");
            // Ne pas lever d'exception - la recette doit être supprimée même si l'image ne l'est pas
        }
    }

    /// <summary>
    /// Valide qu'une URL d'image ne pose pas de risque SSRF
    /// </summary>
    /// <param name="imageUrl">L'URL à valider</param>
    /// <throws>ArgumentException si l'URL est dangereuse</throws>
    private static void ValidateImageUrl(string imageUrl)
    {
        // Les URLs internes (relatives) commençant par / sont acceptées
        if (imageUrl.StartsWith("/"))
        {
            return;
        }

        // Tenter de parser l'URL
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("URL d'image invalide");
        }

        // Vérifier que le schéma est HTTP ou HTTPS
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("Seuls les schémas HTTP et HTTPS sont autorisés");
        }

        // Vérifier que ce n'est pas une adresse IP privée ou réservée
        if (IPAddress.TryParse(uri.Host, out var ipAddress))
        {
            // Bloquer les adresses privées et réservées
            if (ipAddress.IsLoopback || // 127.x.x.x
                ipAddress.IsPrivate || // 10.x.x.x, 172.16-31.x.x, 192.168.x.x
                IsReservedAddress(ipAddress)) // 169.254.x.x, 0.x.x.x, etc.
            {
                throw new ArgumentException("Les adresses IP privées ou réservées ne sont pas autorisées");
            }
        }
    }

    /// <summary>
    /// Vérifie si une adresse IP est réservée (169.254.x.x, etc.)
    /// </summary>
    private static bool IsReservedAddress(IPAddress address)
    {
        // 169.254.x.x (APIPA)
        if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            var bytes = address.GetAddressBytes();
            return bytes[0] == 169 && bytes[1] == 254; // 169.254.x.x
        }
        return false;
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
            CreatedAt = userRecipe.CreatedAt,
            UpdatedAt = userRecipe.UpdatedAt
        };
    }
}
