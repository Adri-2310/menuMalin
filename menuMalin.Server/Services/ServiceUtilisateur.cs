using menuMalin.Server.Modeles.Entites;
using menuMalin.Server.Depots;
using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Services.Interfaces;

namespace menuMalin.Server.Services;

/// <summary>
/// Service pour la gestion des utilisateurs
/// </summary>
public class ServiceUtilisateur : IServiceUtilisateur
{
    private readonly IDepotUtilisateur _userRepository;
    private readonly ILogger<ServiceUtilisateur> _logger;

    public ServiceUtilisateur(IDepotUtilisateur userRepository, ILogger<ServiceUtilisateur> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
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

        _logger.LogInformation("Creating new user: {UserEmail}", userEmail);

        try
        {
            return await _userRepository.AddAsync(newUser);
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("Duplicate") ?? false)
        {
            // Race condition : un autre thread/requête a créé l'utilisateur en même temps
            // Récupérer l'utilisateur qui a été créé
            _logger.LogWarning("Race condition detected for {UserEmail}, retrieving created user", userEmail);
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
        // Hasher le password avec BCrypt (workFactor = 12)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        var newUser = new Utilisateur
        {
            UserId = Guid.NewGuid().ToString("N"),
            Email = email,
            Name = name,
            PasswordHash = passwordHash,
            DateCreation = DateTime.UtcNow
        };

        _logger.LogInformation("Creating new user: {Email}", email);
        return await _userRepository.AddAsync(newUser);
    }

    public async Task<Utilisateur?> GetUserByAuth0IdAsync(string userIdOrAuth0Id)
    {
        // Backward compatibility: traiter comme UserId
        return await _userRepository.GetByUserIdAsync(userIdOrAuth0Id);
    }

    public async Task<Utilisateur?> ModifierNomAsync(string userId, string nouveauNom)
    {
        var user = await _userRepository.GetByUserIdAsync(userId);
        if (user == null)
            return null;

        user.Name = nouveauNom.Trim();
        _logger.LogInformation("Updating name for user: {UserId}", userId);
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<(bool Succes, string? MessageErreur)> ModifierMotDePasseAsync(
        string userId, string motDePasseActuel, string nouveauMotDePasse)
    {
        var user = await _userRepository.GetByUserIdAsync(userId);
        if (user == null)
            return (false, "Utilisateur introuvable");

        // Vérifier que le mot de passe actuel est correct
        if (string.IsNullOrEmpty(user.PasswordHash) ||
            !BCrypt.Net.BCrypt.Verify(motDePasseActuel, user.PasswordHash))
            return (false, "Mot de passe actuel incorrect");

        // Hasher le nouveau mot de passe
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nouveauMotDePasse, workFactor: 12);

        _logger.LogInformation("Updating password for user: {UserId}", userId);
        await _userRepository.UpdateAsync(user);
        return (true, null);
    }
}
