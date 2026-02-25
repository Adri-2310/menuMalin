using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace menuMalin.Services;

/// <summary>
/// Service pour les appels HTTP au Backend
/// </summary>
public class HttpApiService : IHttpApiService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ GET {url} - Status: {response.StatusCode}");
                Console.WriteLine($"Erreur: {errorContent}");
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur dans GetAsync: {ex.Message}");
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string url, object? data = null)
    {
        try
        {
            HttpResponseMessage response;

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            if (data != null)
            {
                request.Content = new StringContent(
                    JsonSerializer.Serialize(data),
                    System.Text.Encoding.UTF8,
                    "application/json");
            }

            response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ POST {url} - Status: {response.StatusCode}");
                Console.WriteLine($"Erreur: {errorContent}");
                Console.WriteLine($"Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Key}={h.Value}"))}");
                return default;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur dans PostAsync: {ex.Message}");
            return default;
        }
    }

    public async Task<bool> DeleteAsync(string url)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await _httpClient.SendAsync(request);
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
            using var request = new HttpRequestMessage(HttpMethod.Patch, url);
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            if (data != null)
            {
                var content = JsonSerializer.Serialize(data);
                request.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return default;

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, JsonOptions);
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
