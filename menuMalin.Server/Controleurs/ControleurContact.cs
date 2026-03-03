using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using menuMalin.Server.Donnees;
using menuMalin.Server.Modeles.Entites;
using menuMalin.Server.Depots;
using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Services;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Controleurs;

/// <summary>
/// Contrôleur pour les messages de contact
/// </summary>
[ApiController]
[Route("api/contact")]
public class ControleurContact : ControllerBase
{
    private readonly IDepotMessage _contactRepository;
    private readonly ApplicationDbContext _context;
    private readonly IServiceEmail _serviceEmail;
    private readonly ILogger<ControleurContact> _logger;

    /// <summary>
    /// Initialise une nouvelle instance de ContactController
    /// </summary>
    /// <param name="contactRepository">Le repository des messages de contact</param>
    /// <param name="context">Le contexte de base de données (pour vérification d'user)</param>
    /// <param name="emailService">Le service d'envoi d'emails</param>
    /// <param name="logger">Le logger</param>
    public ControleurContact(IDepotMessage contactRepository, ApplicationDbContext context, IServiceEmail emailService, ILogger<ControleurContact> logger)
    {
        _contactRepository = contactRepository;
        _context = context;
        _serviceEmail = emailService;
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

        if (request.Email.Length > 255)
        {
            return BadRequest("L'email ne peut pas dépasser 255 caractères");
        }

        if (string.IsNullOrWhiteSpace(request.Subject))
        {
            _logger.LogWarning("POST /api/contact - validation échouée: sujet vide");
            return BadRequest("Le sujet est requis");
        }

        if (request.Subject.Length > 500)
        {
            return BadRequest("Le sujet ne peut pas dépasser 500 caractères");
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            _logger.LogWarning("POST /api/contact - validation échouée: message vide");
            return BadRequest("Le message ne peut pas être vide");
        }

        if (request.Message.Length > 10000)
        {
            return BadRequest("Le message ne peut pas dépasser 10 000 caractères");
        }

        // Récupérer l'utilisateur connecté (optionnel)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogDebug("POST /api/contact - UserId depuis le cookie: {UserId}", userId ?? "anonyme");

        // Vérifier que l'utilisateur existe en base de données (évite violation FK)
        // Le userId vient du claim NameIdentifier et est déjà le UserId (UUID)
        if (!string.IsNullOrEmpty(userId))
        {
            var dbUser = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.UserId == userId);
            if (dbUser == null)
            {
                _logger.LogDebug("POST /api/contact - userId {UserId} non trouvé en DB, contact anonyme", userId);
                userId = null;
            }
        }

        // Créer le message de contact
        var contactMessage = new Message
        {
            ContactId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            MessageContenu = request.Message,
            SubscribeNewsletter = request.SubscribeNewsletter,
            Statut = StatutMessage.New,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _logger.LogDebug("POST /api/contact - Sauvegarde en DB du message {ContactId}", contactMessage.ContactId);

        try
        {
            await _contactRepository.AddAsync(contactMessage);
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
            var emailSent = await _serviceEmail.SendContactNotificationAsync(
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
