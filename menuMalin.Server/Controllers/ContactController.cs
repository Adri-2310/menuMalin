using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using menuMalin.Server.Data;
using menuMalin.Server.Models.Entities;
using menuMalin.Server.Services;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur pour les messages de contact
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de ContactController
    /// </summary>
    /// <param name="context">Le contexte de base de données</param>
    /// <param name="emailService">Le service d'envoi d'emails</param>
    /// <param name="logger">Le logger</param>
    public ContactController(ApplicationDbContext context, IEmailService emailService, ILogger<ContactController> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Envoie un message de contact (endpoint public)
    /// </summary>
    /// <param name="request">Contient l'email, le sujet et le message à envoyer</param>
    /// <returns>Objet JSON avec l'ID du message et un message de confirmation</returns>
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest? request)
    {
        _logger.LogDebug("POST /api/contact reçu. Request body null: {IsNull}", request == null);

        // Vérification que le body a été désérialisé
        if (request == null)
        {
            _logger.LogWarning("POST /api/contact - request body null (problème de désérialisation JSON)");
            return BadRequest("Corps de la requête invalide ou manquant");
        }

        _logger.LogDebug("POST /api/contact - Email: {Email}, Subject: {Subject}, MessageLength: {Len}",
            request.Email, request.Subject, request.Message?.Length ?? 0);

        // Validation
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("POST /api/contact - validation échouée: email vide");
            return BadRequest("L'email est requis");
        }

        if (string.IsNullOrWhiteSpace(request.Subject))
        {
            _logger.LogWarning("POST /api/contact - validation échouée: sujet vide");
            return BadRequest("Le sujet est requis");
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            _logger.LogWarning("POST /api/contact - validation échouée: message vide");
            return BadRequest("Le message ne peut pas être vide");
        }

        // Récupérer l'utilisateur connecté (optionnel)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogDebug("POST /api/contact - UserId depuis le cookie: {UserId}", userId ?? "anonyme");

        // Vérifier que l'utilisateur existe en base de données (évite violation FK)
        // Si l'utilisateur n'existe pas, on laisse UserId = null (contact anonyme)
        // IMPORTANT: on doit stocker le UserId (PK UUID), pas le Auth0Id (claim JWT)
        if (!string.IsNullOrEmpty(userId))
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Auth0Id == userId);
            if (dbUser == null)
            {
                _logger.LogDebug("POST /api/contact - userId {UserId} non trouvé en DB, contact anonyme", userId);
                userId = null;
            }
            else
            {
                // Remplacer le Auth0Id par le UserId (PK UUID) pour la FK ContactMessages.UserId -> Users.UserId
                userId = dbUser.UserId;
            }
        }

        // Créer le message de contact
        var contactMessage = new ContactMessage
        {
            ContactId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            SubscribeNewsletter = request.SubscribeNewsletter,
            Status = ContactMessageStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _logger.LogDebug("POST /api/contact - Sauvegarde en DB du message {ContactId}", contactMessage.ContactId);

        try
        {
            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Message de contact {ContactId} sauvegardé en DB avec succès", contactMessage.ContactId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la sauvegarde du message de contact en DB");
            return StatusCode(500, "Erreur lors de la sauvegarde du message. Veuillez réessayer.");
        }

        // Envoyer l'email de notification (erreur non bloquante)
        try
        {
            _logger.LogDebug("POST /api/contact - Envoi email de notification...");
            var emailSent = await _emailService.SendContactNotificationAsync(
                request.Email,
                request.Name,
                request.Subject,
                request.Message
            );

            if (emailSent)
                _logger.LogInformation("Email de notification envoyé avec succès pour le message {ContactId}", contactMessage.ContactId);
            else
                _logger.LogWarning("Email de notification non envoyé pour le message {ContactId} (config SMTP manquante ou erreur)", contactMessage.ContactId);
        }
        catch (Exception ex)
        {
            // L'email échoue ≠ le message échoue - on log mais on ne renvoie pas d'erreur
            _logger.LogError(ex, "Erreur lors de l'envoi de l'email de notification pour le message {ContactId}", contactMessage.ContactId);
        }

        return Ok(new
        {
            id = contactMessage.ContactId,
            message = "Message envoyé avec succès"
        });
    }
}

/// <summary>
/// Model pour envoyer un message de contact
/// </summary>
public class SendMessageRequest
{
    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool SubscribeNewsletter { get; set; } = false;
}
