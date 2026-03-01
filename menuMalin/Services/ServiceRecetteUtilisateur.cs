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
        try
        {
            if (request == null)
                return null;

            if (string.IsNullOrWhiteSpace(request.Title))
                return null;

            if (string.IsNullOrWhiteSpace(request.Instructions))
                return null;

            var result = await _httpApiService.PostAsync<ReponseRecette>(BaseUrl, request);
            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<RecetteUtilisateurDTO>> GetMyRecipesAsync()
    {
        try
        {
            var recipes = await _httpApiService.GetAsync<List<RecetteUtilisateurDTO>>($"{BaseUrl}/my");
            return recipes ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<RecetteUtilisateurDTO>> GetPublicRecipesAsync()
    {
        try
        {
            var recipes = await _httpApiService.GetAsync<List<RecetteUtilisateurDTO>>($"{BaseUrl}/public");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Error handled silently
            return new();
        }
    }

    public async Task<RecetteUtilisateurDTO?> GetByIdAsync(string userRecipeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userRecipeId))
                return null;

            var recipe = await _httpApiService.GetAsync<ReponseRecette>($"{BaseUrl}/{userRecipeId}");
            return recipe;
        }
        catch (Exception ex)
        {
            // Error handled silently
            return null;
        }
    }

    public async Task<RecetteUtilisateurDTO?> UpdateAsync(string userRecipeId, RequeteCreationRecetteUtilisateur request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userRecipeId))
                return null;

            if (request == null)
                return null;

            if (string.IsNullOrWhiteSpace(request.Title))
                return null;

            if (string.IsNullOrWhiteSpace(request.Instructions))
                return null;

            var result = await _httpApiService.PatchAsync<ReponseRecette>($"{BaseUrl}/{userRecipeId}", request);
            return result;
        }
        catch (Exception ex)
        {
            // Error handled silently
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string userRecipeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userRecipeId))
                return false;

            return await _httpApiService.DeleteAsync($"{BaseUrl}/{userRecipeId}");
        }
        catch (Exception ex)
        {
            // Error handled silently
            return false;
        }
    }

    public async Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userRecipeId))
                return false;

            var request = new RequeteMajVisibilite { IsPublic = isPublic };
            var result = await _httpApiService.PatchAsync<object>($"{BaseUrl}/{userRecipeId}/visibility", request);
            return result != null;
        }
        catch (Exception ex)
        {
            // Error handled silently
            return false;
        }
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
