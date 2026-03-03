using menuMalin.DTOs;
using menuMalin.Shared.Modeles.DTOs;
using menuMalin.Shared.Modeles.Requetes;

namespace menuMalin.Services;

/// <summary>
/// Service pour les recettes utilisateur (Frontend) - communique avec le backend
/// </summary>
public class ServiceRecetteUtilisateur : IServiceRecetteUtilisateur
{
    private readonly IServiceApiHttp _httpApiService;
    private const string BaseUrl = "userrecipes";

    /// <summary>
    /// Initialise une nouvelle instance de ServiceRecetteUtilisateur
    /// </summary>
    /// <param name="httpApiService">Le service HTTP pour communiquer avec le backend</param>
    public ServiceRecetteUtilisateur(IServiceApiHttp httpApiService)
    {
        _httpApiService = httpApiService;
    }

    public async Task<RecetteUtilisateurDTO?> CreateAsync(RequeteCreationRecetteUtilisateur request)
    {
        if (request == null)
            return null;

        if (string.IsNullOrWhiteSpace(request.Title))
            return null;

        if (string.IsNullOrWhiteSpace(request.Instructions))
            return null;

        var result = await _httpApiService.PostAsync<RecetteUtilisateurDTO>(BaseUrl, request);
        return result;
    }

    public async Task<List<RecetteUtilisateurDTO>> GetMyRecipesAsync()
    {
        var recipes = await _httpApiService.GetAsync<List<RecetteUtilisateurDTO>>($"{BaseUrl}/my");
        return recipes ?? new();
    }

    public async Task<List<RecetteUtilisateurDTO>> GetPublicRecipesAsync()
    {
        var recipes = await _httpApiService.GetAsync<List<RecetteUtilisateurDTO>>($"{BaseUrl}/public");
        return recipes ?? new();
    }

    public async Task<RecetteUtilisateurDTO?> GetByIdAsync(string userRecipeId)
    {
        if (string.IsNullOrWhiteSpace(userRecipeId))
            return null;

        var recipe = await _httpApiService.GetAsync<RecetteUtilisateurDTO>($"{BaseUrl}/{userRecipeId}");
        return recipe;
    }

    public async Task<RecetteUtilisateurDTO?> UpdateAsync(string userRecipeId, RequeteCreationRecetteUtilisateur request)
    {
        if (string.IsNullOrWhiteSpace(userRecipeId))
            return null;

        if (request == null)
            return null;

        if (string.IsNullOrWhiteSpace(request.Title))
            return null;

        if (string.IsNullOrWhiteSpace(request.Instructions))
            return null;

        var result = await _httpApiService.PatchAsync<RecetteUtilisateurDTO>($"{BaseUrl}/{userRecipeId}", request);
        return result;
    }

    public async Task<bool> DeleteAsync(string userRecipeId)
    {
        if (string.IsNullOrWhiteSpace(userRecipeId))
            return false;

        return await _httpApiService.DeleteAsync($"{BaseUrl}/{userRecipeId}");
    }

    public async Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic)
    {
        if (string.IsNullOrWhiteSpace(userRecipeId))
            return false;

        var request = new RequeteMajVisibilite { IsPublic = isPublic };
        var result = await _httpApiService.PatchAsync<object>($"{BaseUrl}/{userRecipeId}/visibility", request);
        return result != null;
    }
}

/// <summary>
/// Requête pour mettre à jour la visibilité
/// </summary>
public class RequeteMajVisibilite
{
    /// <summary>
    /// Nouvelle valeur de visibilité
    /// </summary>
    public bool IsPublic { get; set; }
}
