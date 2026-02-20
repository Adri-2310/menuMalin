namespace menuMalin.Server.Models.Entities;

/// <summary>
/// Représente un message de contact (accessible par utilisateurs authentifiés ET anonymes)
/// </summary>
public class ContactMessage
{
    /// <summary>
    /// Identifiant unique du message (UUID)
    /// </summary>
    public string ContactId { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant de l'utilisateur (clé étrangère, NULLABLE pour les contacts anonymes)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Email du contacteur
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Sujet du message (Bug, Suggestion, Autre)
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Corps du message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Statut du message (NEW, READ, RESPONDED, ARCHIVED)
    /// </summary>
    public ContactMessageStatus Status { get; set; } = ContactMessageStatus.New;

    /// <summary>
    /// Date de création du message
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière mise à jour
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Utilisateur qui a envoyé le message (NULL si anonyme)
    /// </summary>
    public User? User { get; set; }
}

/// <summary>
/// Énumération des statuts possibles d'un message de contact
/// </summary>
public enum ContactMessageStatus
{
    /// <summary>Nouveau message non lu</summary>
    New,

    /// <summary>Message lu</summary>
    Read,

    /// <summary>Réponse envoyée</summary>
    Responded,

    /// <summary>Message archivé</summary>
    Archived
}