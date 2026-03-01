using menuMalin.DTOs;

namespace menuMalin.Services;

public interface IServiceRecetteUtilisateur
{
    Task<ReponseRecette?> CreateAsync(RequeteCreationRecetteUtilisateur request);
    Task<List<ReponseRecette>> GetMyRecipesAsync();
    Task<List<ReponseRecette>> GetPublicRecipesAsync();
    Task<ReponseRecette?> GetByIdAsync(string userRecipeId);
    Task<ReponseRecette?> UpdateAsync(string userRecipeId, RequeteCreationRecetteUtilisateur request);
    Task<bool> DeleteAsync(string userRecipeId);
    Task<bool> UpdateVisibilityAsync(string userRecipeId, bool isPublic);
}
