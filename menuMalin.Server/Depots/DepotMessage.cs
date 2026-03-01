using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Donnees;
using menuMalin.Server.Modeles.Entites;

using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Depots.Interfaces;

namespace menuMalin.Server.Depots;

/// <summary>
/// Implémentation du repository pour les messages de contact
/// </summary>
public class DepotMessage : IDepotMessage
{
    private readonly ApplicationDbContext _context;

    public DepotMessage(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message> AddAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<Message>> GetAllAsync()
    {
        return await _context.Messages.ToListAsync();
    }
}
