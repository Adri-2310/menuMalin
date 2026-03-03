using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace menuMalin.Services;

/// <summary>
/// Service pour uploader les fichiers
/// </summary>
public class ServiceTeleversement : IServiceTeleversement
{
    private readonly HttpClient _httpClient;

    public ServiceTeleversement(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> UploadImageAsync(IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream(5 * 1024 * 1024);
        content.Add(new StreamContent(stream), "file", file.Name);

        var request = new HttpRequestMessage(HttpMethod.Post, "upload/recipe-image");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = System.Text.Json.JsonSerializer.Deserialize<ReponseTeleversement>(json, options);
            return result?.ImageUrl;
        }

        return null;
    }
}

public class ReponseTeleversement
{
    public string? ImageUrl { get; set; }
    public string? Message { get; set; }
}
