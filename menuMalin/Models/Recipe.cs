namespace menuMalin.Models;

public class Recipe
{
    public string IdMeal { get; set; } = string.Empty;
    public string StrMeal { get; set; } = string.Empty;
    public string StrCategory { get; set; } = string.Empty;
    public string StrArea { get; set; } = string.Empty;
    public string StrInstructions { get; set; } = string.Empty;
    public string StrMealThumb { get; set; } = string.Empty;
    public string StrYoutube { get; set; } = string.Empty;
}

public class RecipeResponse
{
    public List<Recipe>? Meals { get; set; }
}