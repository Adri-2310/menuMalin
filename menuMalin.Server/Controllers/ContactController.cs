using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using menuMalin.Server.Data;
using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur pour les messages de contact
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance de ContactController
    /// </summary>
    /// <param name="context">Le contexte de base de données</param>
    public ContactController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Envoie un message de contact (endpoint public)
    /// </summary>
    /// <param name="request">Contient l'email, le sujet et le message à envoyer</param>
    /// <returns>Objet JSON avec l'ID du message et un message de confirmation</returns>
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("L'email est requis");

        if (string.IsNullOrWhiteSpace(request.Subject))
            return BadRequest("Le sujet est requis");

        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Le message ne peut pas être vide");

        // Récupérer l'utilisateur connecté (optionnel)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Créer le message de contact
        var contactMessage = new ContactMessage
        {
            ContactId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            Status = 0, // NEW
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ContactMessages.Add(contactMessage);
        await _context.SaveChangesAsync();

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

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
