using System.Net.Http.Json;
using System.Text.Json;

namespace menuMalin.Services;

/// <summary>
/// Service pour les appels HTTP au Backend
/// </summary>
public class HttpApiService : IHttpApiService
{
    private readonly HttpClient _httpClient;

    public HttpApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<T>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur GET {url}: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string url, object? data = null)
    {
        try
        {
            HttpResponseMessage response;

            if (data != null)
            {
                response = await _httpClient.PostAsJsonAsync(url, data);
            }
            else
            {
                response = await _httpClient.PostAsync(url, null);
            }

            if (!response.IsSuccessStatusCode)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur POST {url}: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string url)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur DELETE {url}: {ex.Message}");
            return false;
        }
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
