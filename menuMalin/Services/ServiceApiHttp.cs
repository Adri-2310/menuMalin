using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using menuMalin.Services.Exceptions;

namespace menuMalin.Services;

/// <summary>
/// Service pour les appels HTTP au Backend
/// </summary>
public class ServiceApiHttp : IServiceApiHttp
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // Convertir camelCase → PascalCase
    };

    public ServiceApiHttp(HttpClient httpClient)
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
                throw new ErreurApiException(response.StatusCode, errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (ErreurApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ErreurReseauException(ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ErreurReseauException(ex);
        }
    }

    public async Task<T?> PostAsync<T>(string url, object? data = null)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            if (data != null)
            {
                request.Content = new StringContent(
                    JsonSerializer.Serialize(data, JsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ErreurApiException(response.StatusCode, errorContent);
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (ErreurApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ErreurReseauException(ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ErreurReseauException(ex);
        }
    }

    public async Task<bool> DeleteAsync(string url)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ErreurApiException(response.StatusCode, errorContent);
            }

            return true;
        }
        catch (ErreurApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ErreurReseauException(ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ErreurReseauException(ex);
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
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ErreurApiException(response.StatusCode, errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, JsonOptions);
        }
        catch (ErreurApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ErreurReseauException(ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new ErreurReseauException(ex);
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
