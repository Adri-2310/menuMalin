namespace menuMalin.DTOs;

using menuMalin.Shared.Modeles.DTOs;
using menuMalin.Modeles;

public class ReponseRecette
{
    public List<Recette>? Meals { get; set; }
}

// Extension pour convertir entre RecetteDTO (Shared) et Recette (local)
public static class RecetteDTOExtensions
{
    public static Recette? ToLocalRecette(this RecetteDTO? dto)
    {
        if (dto == null) return null;
        return new Recette
        {
            IdMeal = dto.RecipeId,
            StrMeal = dto.Title,
            StrCategory = dto.Category ?? string.Empty,
            StrArea = dto.Area ?? string.Empty,
            StrInstructions = dto.Instructions ?? string.Empty,
            StrMealThumb = dto.ImageUrl ?? string.Empty
        };
    }

    public static List<Recette> ToLocalRecettes(this List<RecetteDTO>? dtos)
    {
        if (dtos == null) return new();
        return dtos.Select(ToLocalRecette).Where(r => r != null).Cast<Recette>().ToList();
    }
}
