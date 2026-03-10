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

    public async Task<List<Recette>> GetUserFavoritesAsync()
    {
        var dtos = await _httpApiService.GetAsync<List<RecetteDTO>>(BaseUrl);
        var favorites = dtos?.ToLocalRecettes() ?? new();
        return favorites;
    }

    public async Task<bool> AddFavoriteAsync(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
            return false;

        var request = new { recipeId };
        var result = await _httpApiService.PostAsync<object>(BaseUrl, request);
        return result != null;
    }

    public async Task<bool> RemoveFavoriteAsync(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
            return false;

        return await _httpApiService.DeleteAsync($"{BaseUrl}/{recipeId}");
    }

    public async Task<bool> IsFavoriteAsync(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
            return false;

        var result = await _httpApiService.GetAsync<ReponseEstFavori>($"{BaseUrl}/{recipeId}/exists");
        return result?.IsFavorite ?? false;
    }
}

/// <summary>
/// Réponse pour vérifier si favori
/// </summary>
public class ReponseEstFavori
{
    public bool IsFavorite { get; set; }
}
