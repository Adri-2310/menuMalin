using System.Text.Json.Serialization;

namespace menuMalin.Shared.Models.Dtos;

/// <summary>
/// DTO pour les repas de l'API TheMealDB
/// </summary>
public class MealDto
{
    [JsonPropertyName("idMeal")]
    public string IdMeal { get; set; } = string.Empty;

    [JsonPropertyName("strMeal")]
    public string StrMeal { get; set; } = string.Empty;

    [JsonPropertyName("strMealThumb")]
    public string StrMealThumb { get; set; } = string.Empty;

    [JsonPropertyName("strCategory")]
    public string? StrCategory { get; set; }

    [JsonPropertyName("strArea")]
    public string? StrArea { get; set; }

    [JsonPropertyName("strTags")]
    public string? StrTags { get; set; }

    [JsonPropertyName("strInstructions")]
    public string? StrInstructions { get; set; }

    [JsonPropertyName("strSource")]
    public string? StrSource { get; set; }

    [JsonPropertyName("strImageSource")]
    public string? StrImageSource { get; set; }

    // Ingrédients (20 max dans TheMealDB)
    [JsonPropertyName("strIngredient1")]
    public string? StrIngredient1 { get; set; }

    [JsonPropertyName("strMeasure1")]
    public string? StrMeasure1 { get; set; }

    [JsonPropertyName("strIngredient2")]
    public string? StrIngredient2 { get; set; }

    [JsonPropertyName("strMeasure2")]
    public string? StrMeasure2 { get; set; }

    [JsonPropertyName("strIngredient3")]
    public string? StrIngredient3 { get; set; }

    [JsonPropertyName("strMeasure3")]
    public string? StrMeasure3 { get; set; }

    [JsonPropertyName("strIngredient4")]
    public string? StrIngredient4 { get; set; }

    [JsonPropertyName("strMeasure4")]
    public string? StrMeasure4 { get; set; }

    [JsonPropertyName("strIngredient5")]
    public string? StrIngredient5 { get; set; }

    [JsonPropertyName("strMeasure5")]
    public string? StrMeasure5 { get; set; }
}

/// <summary>
/// Réponse de l'API TheMealDB
/// </summary>
public class MealResponse
{
    [JsonPropertyName("meals")]
    public List<MealDto>? Meals { get; set; }
}
