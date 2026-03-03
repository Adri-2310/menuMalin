using menuMalin.DTOs;
using menuMalin.Shared.Modeles.DTOs;
using menuMalin.Shared.Modeles.Requetes;

namespace menuMalin.Services;

public interface IServiceRecetteUtilisateur
{
    Task<RecetteUtilisateurDTO?> CreateAsync(RequeteCreationRecetteUtilisateur request);
    Task<List<RecetteUtilisateurDTO>> GetMyRecipesAsync();
    Task<List<RecetteUtilisateurDTO>> GetPublicRecipesAsync();
    Task<RecetteUtilisateurDTO?> GetByIdAsync(string userRecipeId);
    Task<RecetteUtilisateurDTO?> UpdateAsync(string userRecipeId, RequeteCreationRecetteUtilisateur request);
    Task<bool> DeleteAsync(string userRecipeId);
    Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic);
}
