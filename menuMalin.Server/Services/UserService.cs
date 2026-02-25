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
        return await _userRepository.AddAsync(newUser);
    }

    public async Task<User?> GetUserByAuth0IdAsync(string auth0Id)
    {
        return await _userRepository.GetByAuth0IdAsync(auth0Id);
    }
}
