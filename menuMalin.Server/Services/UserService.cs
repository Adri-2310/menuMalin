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

    public async Task<User> GetOrCreateUserAsync(string emailOrUserId, string? email = null, string? name = null)
    {
        // Pour l'authentification simple, on utilise l'email comme identifiant unique
        var userEmail = email ?? emailOrUserId;

        // Essayer de récupérer l'utilisateur existant par email
        var existingUser = await _userRepository.GetByEmailAsync(userEmail);
        if (existingUser != null)
        {
            return existingUser;
        }

        // Créer un nouvel utilisateur
        var newUser = new User
        {
            UserId = Guid.NewGuid().ToString("N"),
            Email = userEmail,
            Name = name ?? userEmail.Split("@")[0],
            CreatedAt = DateTime.UtcNow
        };

        System.Console.WriteLine($"👤 Création d'un nouvel utilisateur: {userEmail}");

        try
        {
            return await _userRepository.AddAsync(newUser);
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("Duplicate") ?? false)
        {
            // Race condition : un autre thread/requête a créé l'utilisateur en même temps
            // Récupérer l'utilisateur qui a été créé
            System.Console.WriteLine($"⚠️ Race condition détectée pour {userEmail}, récupération de l'utilisateur créé");
            var raceCreatedUser = await _userRepository.GetByEmailAsync(userEmail);
            if (raceCreatedUser != null)
            {
                return raceCreatedUser;
            }
            throw; // Re-lever si on ne peut pas le récupérer
        }
    }

    public async Task<User?> GetUserByAuth0IdAsync(string userIdOrAuth0Id)
    {
        // Backward compatibility: traiter comme UserId
        return await _userRepository.GetByUserIdAsync(userIdOrAuth0Id);
    }
}
