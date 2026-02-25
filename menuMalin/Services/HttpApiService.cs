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
        catch
        {
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
            // Logged in development
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
            // Logged in development
            return false;
        }
    }

    public async Task<T?> PatchAsync<T>(string url, object? data = null)
    {
        try
        {
            HttpResponseMessage response;

            if (data != null)
            {
                var content = JsonSerializer.Serialize(data);
                var httpContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                response = await _httpClient.PatchAsync(url, httpContent);
            }
            else
            {
                response = await _httpClient.PatchAsync(url, null);
            }

            if (!response.IsSuccessStatusCode)
                return default;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent);
        }
        catch (Exception ex)
        {
            // Logged in development
            return default;
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
