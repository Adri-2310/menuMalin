using Microsoft.AspNetCore.Components.Forms;

namespace menuMalin.Services;

/// <summary>
/// Interface pour le service d'upload
/// </summary>
public interface IServiceTeleversement
{
    Task<string?> UploadImageAsync(IBrowserFile file);
}
