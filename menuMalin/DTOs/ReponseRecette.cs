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

        var recipe = new Recette
        {
            // IdMeal doit contenir le MealDBId (ID TheMealDB) pour la navigation vers le détail
            IdMeal = dto.MealDBId,
            StrMeal = dto.Title,
            StrCategory = dto.Category ?? string.Empty,
            StrArea = dto.Area ?? string.Empty,
            StrInstructions = dto.Instructions ?? string.Empty,
            StrMealThumb = dto.ImageUrl ?? string.Empty
        };

        // Mapper les ingrédients de List<IngredientDTO> vers StrIngredient1..20 et StrMeasure1..20
        if (dto.Ingredients?.Count > 0)
        {
            for (int i = 0; i < dto.Ingredients.Count && i < 20; i++)
            {
                var ingredient = dto.Ingredients[i];
                var propertyName = $"StrIngredient{i + 1}";
                var propertyMeasure = $"StrMeasure{i + 1}";

                recipe.GetType().GetProperty(propertyName)?.SetValue(recipe, ingredient.Name ?? string.Empty);
                recipe.GetType().GetProperty(propertyMeasure)?.SetValue(recipe, ingredient.Measure ?? string.Empty);
            }
        }

        return recipe;
    }

    public static List<Recette> ToLocalRecettes(this List<RecetteDTO>? dtos)
    {
        if (dtos == null) return new();
        return dtos.Select(ToLocalRecette).Where(r => r != null).Cast<Recette>().ToList();
    }
}
