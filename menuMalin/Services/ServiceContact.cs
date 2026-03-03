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

            Console.WriteLine($"[ServiceContact] Envoi message de contact - Email: {email}, Sujet: {subject}");

            var result = await _httpApiService.PostAsync<ReponseContact>("contact", request);

            if (result?.Id != null)
            {
                Console.WriteLine($"[ServiceContact] Message envoyé avec succès - ID: {result.Id}");
                return true;
            }

            Console.WriteLine("[ServiceContact] Echec: réponse null ou ID null (voir logs du serveur pour la cause)");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ServiceContact] Exception lors de l'envoi du message: {ex.GetType().Name} - {ex.Message}");
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
