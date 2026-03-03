using System.Text.Json;
using menuMalin.Shared.Modeles.DTOs;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour intégrer l'API TheMealDB
/// Inclut retry automatique sur erreurs transitoires (timeout, connection reset, etc.)
/// </summary>
public class ServiceMealDB : IServiceMealDB
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceMealDB> _logger;
    private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1/";
    private const int MaxRetries = 3;
    private const int InitialDelayMs = 200;

    public ServiceMealDB(HttpClient httpClient, ILogger<ServiceMealDB> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    /// <summary>
    /// Exécute une requête HTTP avec retry automatique sur erreurs transitoires
    /// </summary>
    private async Task<T?> ExecuteWithRetryAsync<T>(Func<Task<HttpResponseMessage>> request, string operationName)
    {
        int attempt = 0;
        while (attempt < MaxRetries)
        {
            try
            {
                attempt++;
                var response = await request();
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (HttpRequestException ex) when (attempt < MaxRetries)
            {
                int delayMs = InitialDelayMs * (int)Math.Pow(2, attempt - 1);
                _logger.LogWarning($"🔄 {operationName} - Tentative {attempt}/{MaxRetries} échouée. Retry après {delayMs}ms: {ex.Message}");
                await Task.Delay(delayMs);
            }
            catch (TaskCanceledException _) when (attempt < MaxRetries)
            {
                int delayMs = InitialDelayMs * (int)Math.Pow(2, attempt - 1);
                _logger.LogWarning($"🔄 {operationName} - Timeout à la tentative {attempt}/{MaxRetries}. Retry après {delayMs}ms");
                await Task.Delay(delayMs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ {operationName} - Erreur inattendue: {ex.Message}");
                return default;
            }
        }

        _logger.LogError($"❌ {operationName} - Échec après {MaxRetries} tentatives");
        return default;
    }

    public async Task<RecetteMealDTO?> GetRandomAsync()
    {
        var result = await ExecuteWithRetryAsync<RecetteMealResponse>(
            () => _httpClient.GetAsync("random.php"),
            "GetRandom");

        return result?.Meals?.FirstOrDefault();
    }

    public async Task<List<RecetteMealDTO>> SearchByNameAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new();

        var result = await ExecuteWithRetryAsync<RecetteMealResponse>(
            () => _httpClient.GetAsync($"search.php?s={query}"),
            $"SearchByName({query})");

        return result?.Meals ?? new();
    }

    public async Task<RecetteMealDTO?> GetByIdAsync(string mealId)
    {
        if (string.IsNullOrWhiteSpace(mealId))
            return null;

        var result = await ExecuteWithRetryAsync<RecetteMealResponse>(
            () => _httpClient.GetAsync($"lookup.php?i={mealId}"),
            $"GetById({mealId})");

        return result?.Meals?.FirstOrDefault();
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        var result = await ExecuteWithRetryAsync<CategoriesResponse>(
            () => _httpClient.GetAsync("categories.php"),
            "GetCategories");

        return result?.Categories?.Select(c => c.StrCategory).Where(c => !string.IsNullOrEmpty(c)).Cast<string>().ToList() ?? new();
    }

    public async Task<List<string>> GetAreasAsync()
    {
        var result = await ExecuteWithRetryAsync<AreasResponse>(
            () => _httpClient.GetAsync("list.php?a=list"),
            "GetAreas");

        return result?.Meals?.Select(m => m.StrArea).Where(a => !string.IsNullOrEmpty(a)).Cast<string>().ToList() ?? new();
    }

    public async Task<List<RecetteMealDTO>> FilterByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return new();

        var result = await ExecuteWithRetryAsync<RecetteMealResponse>(
            () => _httpClient.GetAsync($"filter.php?c={category}"),
            $"FilterByCategory({category})");

        return result?.Meals ?? new();
    }

    public async Task<List<RecetteMealDTO>> FilterByAreaAsync(string area)
    {
        if (string.IsNullOrWhiteSpace(area))
            return new();

        var result = await ExecuteWithRetryAsync<RecetteMealResponse>(
            () => _httpClient.GetAsync($"filter.php?a={area}"),
            $"FilterByArea({area})");

        return result?.Meals ?? new();
    }
}

/// <summary>
/// Modèles pour les réponses TheMealDB
/// </summary>
public class CategoriesResponse
{
    public List<CategoryItem>? Categories { get; set; }
}

public class CategoryItem
{
    public string StrCategory { get; set; } = string.Empty;
}

public class AreasResponse
{
    public List<AreaItem>? Meals { get; set; }
}

public class AreaItem
{
    public string StrArea { get; set; } = string.Empty;
}
