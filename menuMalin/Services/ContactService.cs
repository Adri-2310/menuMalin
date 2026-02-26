namespace menuMalin.Services;

/// <summary>
/// Service pour gérer les messages de contact
/// </summary>
public class ContactService : IContactService
{
    private readonly IHttpApiService _httpApiService;

    public ContactService(IHttpApiService httpApiService)
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

            Console.WriteLine($"[ContactService] Envoi message de contact - Email: {email}, Sujet: {subject}");

            var result = await _httpApiService.PostAsync<ContactResponse>("contact", request);

            if (result?.Id != null)
            {
                Console.WriteLine($"[ContactService] Message envoyé avec succès - ID: {result.Id}");
                return true;
            }

            Console.WriteLine("[ContactService] Echec: réponse null ou ID null (voir logs du serveur pour la cause)");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ContactService] Exception lors de l'envoi du message: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Classe de réponse du serveur pour la création d'un message de contact
    /// </summary>
    internal class ContactResponse
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
    }
}
