namespace menuMalin.Services;

/// <summary>
/// Service pour gérer les messages de contact
/// </summary>
public class ServiceContact : IServiceContact
{
    private readonly IServiceApiHttp _httpApiService;

    public ServiceContact(IServiceApiHttp httpApiService)
    {
        _httpApiService = httpApiService;
    }

    public async Task<bool> SendMessageAsync(string email, string? name, string subject, string message, bool subscribeNewsletter)
    {
        try
        {
            var request = new
            {
                email,
                name,
                subject,
                message,
                subscribeNewsletter
            };

            var result = await _httpApiService.PostAsync<ReponseContact>("contact", request);

            if (result?.Id != null)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Classe de réponse du serveur pour la création d'un message de contact
    /// </summary>
    internal class ReponseContact
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
    }
}
