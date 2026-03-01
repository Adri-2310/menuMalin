using menuMalin.Server.Modeles.Entites;

namespace menuMalin.Server.Depots.Interfaces;

/// <summary>
/// Interface pour le repository des messages de contact
/// </summary>
public interface IDepotMessage
{
    /// <summary>
    /// Ajoute un nouveau message de contact
    /// </summary>
    Task<Message> AddAsync(Message message);

    /// <summary>
    /// Récupère tous les messages de contact
    /// </summary>
    Task<List<Message>> GetAllAsync();
}
