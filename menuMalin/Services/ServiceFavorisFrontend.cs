using menuMalin.Modeles;
using menuMalin.DTOs;
using menuMalin.Shared.Modeles.DTOs;

namespace menuMalin.Services;

/// <summary>
/// Service pour les favoris (Frontend) - communique avec le backend
/// </summary>
public class ServiceFavorisFrontend : IServiceFavorisFrontend
{
    private readonly IServiceApiHttp _httpApiService;
    private const string BaseUrl = "favorites";

    public ServiceFavorisFrontend(IServiceApiHttp httpApiService)
    {
        _httpApiService = httpApiService;
    }

    // Note: Les logs Console.WriteLine sont intentionnellement conservés pour le debug frontend
    // car ce service s'exécute en Blazor WASM sans accès à ILogger structuré

    public async Task<List<Recette>> GetUserFavoritesAsync()
    {
        // Le backend retourne une liste de RecetteDTO (recipeId, title, imageUrl, ...)
        var dtos = await _httpApiService.GetAsync<List<RecetteDTO>>(BaseUrl);
        var favorites = dtos?.ToLocalRecettes() ?? new();
        if (favorites.Count > 0)
        {
            Console.WriteLine($"✅ {favorites.Count} favoris chargés");
        }
        else
        {
            Console.WriteLine("ℹ️ Aucun favori trouvé");
        }
        return favorites;
    }

    public async Task<bool> AddFavoriteAsync(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
        {
            Console.WriteLine("❌ AddFavoriteAsync: RecipeId vide ou null");
            return false;
        }

        var request = new { recipeId };
        var result = await _httpApiService.PostAsync<object>(BaseUrl, request);

        if (result != null)
        {
            Console.WriteLine($"✅ Favori ajouté: {recipeId}");
            return true;
        }
        else
        {
            Console.WriteLine($"❌ Erreur lors de l'ajout du favori {recipeId}");
            return false;
        }
    }

    public async Task<bool> RemoveFavoriteAsync(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
        {
            Console.WriteLine("❌ RemoveFavoriteAsync: RecipeId vide ou null");
            return false;
        }

        var result = await _httpApiService.DeleteAsync($"{BaseUrl}/{recipeId}");
        if (result)
        {
            Console.WriteLine($"✅ Favori supprimé: {recipeId}");
        }
        else
        {
            Console.WriteLine($"❌ Erreur lors de la suppression du favori {recipeId}");
        }
        return result;
    }

    public async Task<bool> IsFavoriteAsync(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
        {
            Console.WriteLine("❌ IsFavoriteAsync: RecipeId vide ou null");
            return false;
        }

        var result = await _httpApiService.GetAsync<ReponseEstFavori>($"{BaseUrl}/{recipeId}/exists");
        var isFavorite = result?.IsFavorite ?? false;
        Console.WriteLine($"ℹ️ Vérification favori {recipeId}: {(isFavorite ? "OUI" : "NON")}");
        return isFavorite;
    }
}

/// <summary>
/// Réponse pour vérifier si favori
/// </summary>
public class ReponseEstFavori
{
    public bool IsFavorite { get; set; }
}
