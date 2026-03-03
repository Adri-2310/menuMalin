using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace menuMalin.Server.Controleurs;

/// <summary>
/// Contrôleur pour l'upload de fichiers
/// </summary>
[ApiController]
[Route("api/upload")]
[Authorize]
public class ControleurTeleversement : ControllerBase
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly ILogger<ControleurTeleversement> _logger;
    private const string UploadsFolder = "uploads";
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public ControleurTeleversement(IWebHostEnvironment hostEnvironment, ILogger<ControleurTeleversement> logger)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    /// <summary>
    /// Upload une image de recette (authentifié)
    /// </summary>
    [HttpPost("recipe-image")]
    public async Task<IActionResult> UploadRecipeImage(IFormFile file)
    {
        try
        {
            // Vérifier que le fichier n'est pas null
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Aucun fichier sélectionné" });

            // Vérifier la taille du fichier
            if (file.Length > MaxFileSize)
                return BadRequest(new { error = "L'image est trop grande (max 5MB)" });

            // Vérifier le type MIME déclaré
            var validTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!validTypes.Contains(file.ContentType))
                return BadRequest(new { error = "Format d'image non supporté" });

            // Valider les magic bytes du fichier
            var magicBytesValid = await ValidateImageMagicBytes(file);
            if (!magicBytesValid)
                return BadRequest(new { error = "Le fichier n'est pas une image valide" });

            // Créer le dossier s'il n'existe pas
            var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Générer un nom de fichier entièrement nouveau (pas le nom original)
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? ".jpg";
            // Valider l'extension pour éviter les uploads avec des extensions dangereuses
            if (!IsValidImageExtension(extension))
                return BadRequest(new { error = "Extension de fichier non autorisée" });

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Double-check : vérifier que le chemin final est bien dans le dossier uploads
            var fullPath = Path.GetFullPath(filePath);
            var uploadsFullPath = Path.GetFullPath(uploadsPath);
            if (!fullPath.StartsWith(uploadsFullPath))
                return BadRequest(new { error = "Chemin de fichier invalide" });

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retourner le chemin relatif
            var relativePath = $"/{UploadsFolder}/{fileName}";
            _logger.LogInformation("Image uploaded successfully: {RelativePath}", relativePath);

            return Ok(new { imageUrl = relativePath, message = "Image uploadée avec succès" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, new { error = "Erreur serveur lors de l'upload" });
        }
    }

    /// <summary>
    /// Valide les magic bytes du fichier pour s'assurer que c'est bien une image
    /// </summary>
    private static async Task<bool> ValidateImageMagicBytes(IFormFile file)
    {
        // Dictionnaire des magic bytes pour chaque type d'image
        var imageMagicBytes = new Dictionary<string, byte[]>
        {
            { "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF } },
            { "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47 } },
            { "image/gif", new byte[] { 0x47, 0x49, 0x46 } },
            { "image/webp", new byte[] { 0x52, 0x49, 0x46, 0x46 } }
        };

        if (!imageMagicBytes.ContainsKey(file.ContentType))
            return false;

        var expectedMagic = imageMagicBytes[file.ContentType];
        var buffer = new byte[expectedMagic.Length];

        using var stream = file.OpenReadStream();
        stream.Seek(0, SeekOrigin.Begin);
        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

        if (bytesRead < expectedMagic.Length)
            return false;

        // Comparer les magic bytes
        return buffer.SequenceEqual(expectedMagic);
    }

    /// <summary>
    /// Vérifie que l'extension de fichier est autorisée
    /// </summary>
    private static bool IsValidImageExtension(string extension)
    {
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        return validExtensions.Contains(extension);
    }
}
