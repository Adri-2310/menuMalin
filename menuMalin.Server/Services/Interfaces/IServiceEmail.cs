namespace menuMalin.Server.Services.Interfaces;

/// <summary>
/// Interface pour l'envoi d'emails
/// </summary>
public interface IServiceEmail
{
    /// <summary>
    /// Envoie un email de notification pour un message de contact reçu
    /// </summary>
    /// <param name="senderEmail">Email de l'expéditeur du message</param>
    /// <param name="senderName">Nom de l'expéditeur (peut être null)</param>
    /// <param name="subject">Sujet du message de contact</param>
    /// <param name="messageBody">Corps du message de contact</param>
    /// <returns>True si l'email a été envoyé avec succès, False sinon</returns>
    Task<bool> SendContactNotificationAsync(string senderEmail, string? senderName, string subject, string messageBody);
}
