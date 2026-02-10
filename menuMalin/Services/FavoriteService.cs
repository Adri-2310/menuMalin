using Blazored.LocalStorage;
using menuMalin.Models;

namespace menuMalin.Services;

public class FavoriteService : IFavoriteService
{
    private readonly ILocalStorageService _localStorage;
    private const string Key = "favorites";

    public FavoriteService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task AddFavoriteAsync(Recipe recipe)
    {
        var favorites = await GetFavoritesAsync();
        // On évite les doublons
        if (!favorites.Any(r => r.IdMeal == recipe.IdMeal))
        {
            favorites.Add(recipe);
            await _localStorage.SetItemAsync(Key, favorites);
        }
    }

    public async Task RemoveFavoriteAsync(string recipeId)
    {
        var favorites = await GetFavoritesAsync();
        var recipe = favorites.FirstOrDefault(r => r.IdMeal == recipeId);
        if (recipe != null)
        {
            favorites.Remove(recipe);
            await _localStorage.SetItemAsync(Key, favorites);
        }
    }

    public async Task<bool> IsFavoriteAsync(string recipeId)
    {
        var favorites = await GetFavoritesAsync();
        return favorites.Any(r => r.IdMeal == recipeId);
    }

    public async Task<List<Recipe>> GetFavoritesAsync()
    {
        // On récupère la liste ou on en crée une vide si c'est la première fois
        return await _localStorage.GetItemAsync<List<Recipe>>(Key) ?? new List<Recipe>();
    }
}