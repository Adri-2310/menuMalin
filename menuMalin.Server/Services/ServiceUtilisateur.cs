using menuMalin.Server.Modeles.Entites;
using menuMalin.Server.Depots;
using menuMalin.Server.Depots.Interfaces;

using menuMalin.Server.Services.Interfaces;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour la gestion des utilisateurs
/// </summary>
public class ServiceUtilisateur : IServiceUtilisateur
{
    private readonly IDepotUtilisateur _userRepository;

    public ServiceUtilisateur(IDepotUtilisateur userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Utilisateur> GetOrCreateUserAsync(string emailOrUserId, string? email = null, string? name = null)
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
        var newUser = new Utilisateur
        {
            UserId = Guid.NewGuid().ToString("N"),
            Email = userEmail,
            Name = name ?? userEmail.Split("@")[0],
            DateCreation = DateTime.UtcNow
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

    public async Task<Utilisateur?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<Utilisateur> CreateUserAsync(string email, string password, string name)
    {
        var newUser = new Utilisateur
        {
            UserId = Guid.NewGuid().ToString("N"),
            Email = email,
            Name = name,
            DateCreation = DateTime.UtcNow
        };

        System.Console.WriteLine($"👤 Création d'un nouvel utilisateur: {email}");
        return await _userRepository.AddAsync(newUser);
    }

    public async Task<Utilisateur?> GetUserByAuth0IdAsync(string userIdOrAuth0Id)
    {
        // Backward compatibility: traiter comme UserId
        return await _userRepository.GetByUserIdAsync(userIdOrAuth0Id);
    }
}
