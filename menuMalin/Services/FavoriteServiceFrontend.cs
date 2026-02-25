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
            return favorites ?? new();
        }
        catch (Exception ex)
        {
            // Logged in development
            return new();
        }
    }

    public async Task<bool> AddFavoriteAsync(string recipeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(recipeId))
                return false;

            var request = new { recipeId };
            var result = await _httpApiService.PostAsync<object>(BaseUrl, request);
            return result != null;
        }
        catch (Exception ex)
        {
            // Logged in development
            return false;
        }
    }

    public async Task<bool> RemoveFavoriteAsync(string recipeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(recipeId))
                return false;

            return await _httpApiService.DeleteAsync($"{BaseUrl}/{recipeId}");
        }
        catch (Exception ex)
        {
            // Logged in development
            return false;
        }
    }

    public async Task<bool> IsFavoriteAsync(string recipeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(recipeId))
                return false;

            var result = await _httpApiService.GetAsync<IsFavoriteResponse>($"{BaseUrl}/{recipeId}/exists");
            return result?.IsFavorite ?? false;
        }
        catch (Exception ex)
        {
            // Logged in development
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
