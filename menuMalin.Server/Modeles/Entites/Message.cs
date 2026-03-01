namespace menuMalin.Server.Modeles.Entites;

/// <summary>
/// Représente un message de contact (accessible par utilisateurs authentifiés ET anonymes)
/// </summary>
public class Message
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
    /// Nom du contacteur
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Email du contacteur
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Sujet du message (Bug, Suggestion, Autre)
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Consentement pour la newsletter
    /// </summary>
    public bool SubscribeNewsletter { get; set; } = false;

    /// <summary>
    /// Corps du message
    /// </summary>
    public string MessageContenu { get; set; } = string.Empty;

    /// <summary>
    /// Statut du message (NEW, READ, RESPONDED, ARCHIVED)
    /// </summary>
    public StatutMessage Statut { get; set; } = StatutMessage.New;

    /// <summary>
    /// Date de création du message
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière mise à jour
    /// </summary>
    public DateTime DateMaj { get; set; } = DateTime.UtcNow;

    // Relationships
    /// <summary>
    /// Utilisateur qui a envoyé le message (NULL si anonyme)
    /// </summary>
    public Utilisateur? Utilisateur { get; set; }
}

/// <summary>
/// Énumération des statuts possibles d'un message de contact
/// </summary>
public enum StatutMessage
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