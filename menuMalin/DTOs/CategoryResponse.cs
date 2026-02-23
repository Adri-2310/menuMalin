namespace menuMalin.DTOs;

public class CategoryResponse
{
    public List<Category>? Meals { get; set; }
}

public class Category
{
    public string? StrCategory { get; set; }
}
