using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Data;
using menuMalin.Server.Models.Entities;

namespace menuMalin.Server.Repositories;

/// <summary>
/// Implémentation du repository pour les messages de contact
/// </summary>
public class ContactRepository : IContactRepository
{
    private readonly ApplicationDbContext _context;

    public ContactRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ContactMessage> AddAsync(ContactMessage message)
    {
        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<ContactMessage>> GetAllAsync()
    {
        return await _context.ContactMessages.ToListAsync();
    }
}
