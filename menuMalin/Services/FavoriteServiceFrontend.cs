using menuMalin.Shared.Models.Dtos;

namespace menuMalin.Services;

/// <summary>
/// Service pour les favoris (Frontend) - communique avec le backend
/// </summary>
public class FavoriteServiceFrontend : IFavoriteServiceFrontend
{
    private readonly IHttpApiService _httpApiService;
    private const string BaseUrl = "favorites";

    public FavoriteServiceFrontend(IHttpApiService httpApiService)
    {
        _httpApiService = httpApiService;
    }

    public async Task<List<RecipeDto>> GetUserFavoritesAsync()
    {
        try
        {
            var favorites = await _httpApiService.GetAsync<List<RecipeDto>>(BaseUrl);
            if (favorites != null && favorites.Count > 0)
            {
                Console.WriteLine($"✅ {favorites.Count} favoris chargés");
            }
            else
            {
                Console.WriteLine("ℹ️ Aucun favori trouvé");
            }
            return favorites ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur GetUserFavoritesAsync: {ex.Message}");
            return new();
        }
    }

    public async Task<bool> AddFavoriteAsync(string recipeId)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur AddFavoriteAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveFavoriteAsync(string recipeId)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur RemoveFavoriteAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsFavoriteAsync(string recipeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(recipeId))
            {
                Console.WriteLine("❌ IsFavoriteAsync: RecipeId vide ou null");
                return false;
            }

            var result = await _httpApiService.GetAsync<IsFavoriteResponse>($"{BaseUrl}/{recipeId}/exists");
            var isFavorite = result?.IsFavorite ?? false;
            Console.WriteLine($"ℹ️ Vérification favori {recipeId}: {(isFavorite ? "OUI" : "NON")}");
            return isFavorite;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur IsFavoriteAsync: {ex.Message}");
            return false;
        }
    }
}

/// <summary>
/// Réponse pour vérifier si favori
/// </summary>
public class IsFavoriteResponse
{
    public bool IsFavorite { get; set; }
}
