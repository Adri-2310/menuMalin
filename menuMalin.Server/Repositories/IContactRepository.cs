using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Interface pour le repository des messages de contact
/// </summary>
public interface IContactRepository
{
    /// <summary>
    /// Ajoute un nouveau message de contact
    /// </summary>
    Task<ContactMessage> AddAsync(ContactMessage message);

    /// <summary>
    /// Récupère tous les messages de contact
    /// </summary>
    Task<List<ContactMessage>> GetAllAsync();
}
