using menuMalin.Server.Models.Entities;
using menuMalin.Server.Repositories;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour la gestion des utilisateurs
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetOrCreateUserAsync(string auth0Id, string? email = null, string? name = null)
    {
        // Essayer de récupérer l'utilisateur existant
        var existingUser = await _userRepository.GetByAuth0IdAsync(auth0Id);
        if (existingUser != null)
        {
            return existingUser;
        }

        // Créer un nouvel utilisateur
        var newUser = new User
        {
            UserId = Guid.NewGuid().ToString("N"),
            Auth0Id = auth0Id,
            Email = email ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        System.Console.WriteLine($"👤 Création d'un nouvel utilisateur: {auth0Id}");

        try
        {
            return await _userRepository.AddAsync(newUser);
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("Duplicate") ?? false)
        {
            // Race condition : un autre thread/requête a créé l'utilisateur en même temps
            // Récupérer l'utilisateur qui a été créé
            System.Console.WriteLine($"⚠️ Race condition détectée pour {auth0Id}, récupération de l'utilisateur créé");
            var raceCreatedUser = await _userRepository.GetByAuth0IdAsync(auth0Id);
            if (raceCreatedUser != null)
            {
                return raceCreatedUser;
            }
            throw; // Re-lever si on ne peut pas le récupérer
        }
    }

    public async Task<User?> GetUserByAuth0IdAsync(string auth0Id)
    {
        return await _userRepository.GetByAuth0IdAsync(auth0Id);
    }
}
