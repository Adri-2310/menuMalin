using menuMalin.Server.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace menuMalin.Server.Services;

/// <summary>
/// Service d'envoi d'emails via SMTP
/// </summary>
public class ServiceEmail : IServiceEmail
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceEmail> _logger;

    public ServiceEmail(IConfiguration configuration, ILogger<ServiceEmail> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> SendContactNotificationAsync(
        string senderEmail,
        string? senderName,
        string subject,
        string messageBody)
    {
        try
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var host = smtpSection["Host"];
            var portStr = smtpSection["Port"];
            var username = smtpSection["Username"];
            var password = smtpSection["Password"];
            var fromEmail = smtpSection["FromEmail"];
            var toEmail = smtpSection["ToEmail"];
            var enableSsl = bool.Parse(smtpSection["EnableSsl"] ?? "true");

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogWarning("Configuration SMTP incomplète - email non envoyé. Host: {Host}, ToEmail: {ToEmail}", host, toEmail);
                return false;
            }

            if (!int.TryParse(portStr, out var port))
            {
                port = 587;
            }

            var displayName = string.IsNullOrWhiteSpace(senderName) ? senderEmail : $"{senderName} ({senderEmail})";

            // Log de debug pour vérifier la configuration lue depuis appsettings.json
            _logger.LogDebug(
                "Configuration SMTP chargée - Host: '{Host}', Port: {Port}, EnableSsl: {Ssl}, Username: '{Username}', FromEmail: '{FromEmail}', ToEmail: '{ToEmail}'",
                host, port, enableSsl, username, fromEmail, toEmail);

            // Corps de l'email en HTML
            var emailBody = $"""
                <html>
                <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                    <div style="background-color: #4CAF50; color: white; padding: 20px; border-radius: 8px 8px 0 0;">
                        <h2 style="margin: 0;">Nouveau message de contact - menuMalin</h2>
                    </div>
                    <div style="border: 1px solid #ddd; border-top: none; padding: 20px; border-radius: 0 0 8px 8px;">
                        <table style="width: 100%; border-collapse: collapse;">
                            <tr>
                                <td style="padding: 8px; font-weight: bold; color: #555; width: 120px;">De :</td>
                                <td style="padding: 8px;">{System.Web.HttpUtility.HtmlEncode(displayName)}</td>
                            </tr>
                            <tr style="background-color: #f9f9f9;">
                                <td style="padding: 8px; font-weight: bold; color: #555;">Email :</td>
                                <td style="padding: 8px;"><a href="mailto:{System.Web.HttpUtility.HtmlEncode(senderEmail)}">{System.Web.HttpUtility.HtmlEncode(senderEmail)}</a></td>
                            </tr>
                            <tr>
                                <td style="padding: 8px; font-weight: bold; color: #555;">Sujet :</td>
                                <td style="padding: 8px;">{System.Web.HttpUtility.HtmlEncode(subject)}</td>
                            </tr>
                        </table>
                        <hr style="margin: 20px 0; border: none; border-top: 1px solid #eee;" />
                        <h3 style="color: #333; margin-bottom: 10px;">Message :</h3>
                        <div style="background-color: #f5f5f5; padding: 15px; border-radius: 4px; white-space: pre-wrap;">{System.Web.HttpUtility.HtmlEncode(messageBody)}</div>
                        <hr style="margin: 20px 0; border: none; border-top: 1px solid #eee;" />
                        <p style="color: #999; font-size: 12px; margin: 0;">
                            Ce message a été envoyé via le formulaire de contact de menuMalin le {DateTime.Now:dd/MM/yyyy à HH:mm}.
                        </p>
                    </div>
                </body>
                </html>
                """;

            using var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            // Ajouter les credentials seulement s'ils sont configurés
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? username ?? senderEmail, "menuMalin Contact"),
                Subject = $"[menuMalin] Nouveau message : {subject}",
                Body = emailBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            // Ajouter Reply-To pour faciliter la réponse à l'expéditeur
            mailMessage.ReplyToList.Add(new MailAddress(senderEmail, senderName ?? senderEmail));

            _logger.LogInformation("Envoi email à {ToEmail} - De: {SenderEmail}, Sujet: {Subject}", toEmail, senderEmail, subject);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email envoyé avec succès à {ToEmail}", toEmail);
            return true;
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "Erreur SMTP lors de l'envoi de l'email de contact. StatusCode: {StatusCode}, Message: {Message}",
                ex.StatusCode, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de l'envoi de l'email de contact");
            return false;
        }
    }
}
