namespace menuMalin.Services;

/// <summary>
/// Interface pour les appels HTTP à l'API Backend
/// </summary>
public interface IHttpApiService
{
    /// <summary>
    /// Effectue une requête GET
    /// </summary>
    Task<T?> GetAsync<T>(string url);

    /// <summary>
    /// Effectue une requête POST
    /// </summary>
    Task<T?> PostAsync<T>(string url, object? data = null);

    /// <summary>
    /// Effectue une requête DELETE
    /// </summary>
    Task<bool> DeleteAsync(string url);

    /// <summary>
    /// Définit le token JWT
    /// </summary>
    void SetAuthToken(string token);

    /// <summary>
    /// Efface le token JWT
    /// </summary>
    void ClearAuthToken();
}
