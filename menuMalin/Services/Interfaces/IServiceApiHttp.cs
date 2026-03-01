namespace menuMalin.Services;

/// <summary>
/// Interface pour les appels HTTP au Backend
/// </summary>
public interface IServiceApiHttp
{
    Task<T?> GetAsync<T>(string url);
    Task<T?> PostAsync<T>(string url, object? data = null);
    Task<bool> DeleteAsync(string url);
    Task<T?> PatchAsync<T>(string url, object? data = null);
    void SetAuthToken(string token);
    void ClearAuthToken();
}
