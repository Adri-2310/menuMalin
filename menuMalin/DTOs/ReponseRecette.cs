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
            RecipeId = dto.RecipeId,
            Title = dto.Title,
            Description = dto.Description,
            Instructions = dto.Instructions,
            ImageUrl = dto.ImageUrl,
            MealDBId = dto.MealDBId,
            Category = dto.Category,
            Area = dto.Area,
            Tags = dto.Tags,
            DateCreation = dto.CreatedAt,
            DateMaj = dto.UpdatedAt
        };
    }

    public static List<Recette> ToLocalRecettes(this List<RecetteDTO>? dtos)
    {
        if (dtos == null) return new();
        return dtos.Select(ToLocalRecette).Where(r => r != null).Cast<Recette>().ToList();
    }
}
