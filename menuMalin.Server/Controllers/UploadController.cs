using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace menuMalin.Server.Controllers;

/// <summary>
/// Contrôleur pour l'upload de fichiers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private const string UploadsFolder = "uploads";
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public UploadController(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    /// <summary>
    /// Upload une image de recette
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

            // Vérifier le type MIME
            var validTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!validTypes.Contains(file.ContentType))
                return BadRequest(new { error = "Format d'image non supporté" });

            // Créer le dossier s'il n'existe pas
            var uploadsPath = Path.Combine(_hostEnvironment.WebRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Générer un nom de fichier unique
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Sauvegarder le fichier
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retourner le chemin relatif
            var relativePath = $"/{UploadsFolder}/{fileName}";
            System.Console.WriteLine($"✅ Image uploadée: {relativePath}");

            return Ok(new { imageUrl = relativePath, message = "Image uploadée avec succès" });
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Erreur lors de l'upload: {ex.Message}");
            return StatusCode(500, new { error = $"Erreur serveur: {ex.Message}" });
        }
    }
}
