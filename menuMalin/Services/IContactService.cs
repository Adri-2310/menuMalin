namespace menuMalin.Services;

/// <summary>
/// Interface pour le service de contact
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Envoie un message de contact
    /// </summary>
    Task<bool> SendMessageAsync(string email, string subject, string message);
}
