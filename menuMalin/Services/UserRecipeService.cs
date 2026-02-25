using menuMalin.Shared.Models.Dtos;
using menuMalin.Shared.Models.Requests;

namespace menuMalin.Services;

/// <summary>
/// Service pour les recettes utilisateur (Frontend) - communique avec le backend
/// </summary>
public class UserRecipeService : IUserRecipeService
{
    private readonly IHttpApiService _httpApiService;
    private const string BaseUrl = "userrecipes";

    /// <summary>
    /// Initialise une nouvelle instance de UserRecipeService
    /// </summary>
    /// <param name="httpApiService">Le service HTTP pour communiquer avec le backend</param>
    public UserRecipeService(IHttpApiService httpApiService)
    {
        _httpApiService = httpApiService;
    }

    public async Task<UserRecipeDto?> CreateAsync(CreateUserRecipeRequest request)
    {
        try
        {
            if (request == null)
                return null;

            if (string.IsNullOrWhiteSpace(request.Title))
                return null;

            if (string.IsNullOrWhiteSpace(request.Instructions))
                return null;

            var result = await _httpApiService.PostAsync<UserRecipeDto>(BaseUrl, request);
            return result;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<UserRecipeDto>> GetMyRecipesAsync()
    {
        try
        {
            var recipes = await _httpApiService.GetAsync<List<UserRecipeDto>>($"{BaseUrl}/my");
            return recipes ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<UserRecipeDto>> GetPublicRecipesAsync()
    {
        try
        {
            var recipes = await _httpApiService.GetAsync<List<UserRecipeDto>>($"{BaseUrl}/public");
            return recipes ?? new();
        }
        catch (Exception ex)
        {
            // Error handled silently
            return new();
        }
    }

    public async Task<UserRecipeDto?> GetByIdAsync(string userRecipeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userRecipeId))
                return null;

            var recipe = await _httpApiService.GetAsync<UserRecipeDto>($"{BaseUrl}/{userRecipeId}");
            return recipe;
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

            var request = new UpdateVisibilityRequest { IsPublic = isPublic };
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
public class UpdateVisibilityRequest
{
    /// <summary>
    /// Nouvelle valeur de visibilité
    /// </summary>
    public bool IsPublic { get; set; }
}
