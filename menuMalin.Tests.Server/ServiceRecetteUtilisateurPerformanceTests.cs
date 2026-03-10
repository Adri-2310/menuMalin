using BenchmarkDotNet.Attributes;
using NSubstitute;
using menuMalin.Server.Services;
using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Modeles.Entites;
using menuMalin.Shared.Modeles.Requetes;
using Microsoft.Extensions.Logging;

namespace menuMalin.Tests.Server;

/// <summary>
/// Tests de performance pour ServiceRecetteUtilisateur
/// Mesure: validation, sérialisation JSON, sécurité propriétaire
///
/// Exécuter avec: dotnet run -c Release -- --filter ServiceRecetteUtilisateurPerformanceTests
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, targetCount: 5)]
public class ServiceRecetteUtilisateurPerformanceTests
{
    private ServiceRecetteUtilisateur _service = null!;
    private IDepotRecetteUtilisateur _mockRepository = null!;
    private IWebHostEnvironment _mockHostEnvironment = null!;
    private ILogger<ServiceRecetteUtilisateur> _mockLogger = null!;
    private const string TestUserId = "perf-user";

    [GlobalSetup]
    public void Setup()
    {
        _mockRepository = Substitute.For<IDepotRecetteUtilisateur>();
        _mockHostEnvironment = Substitute.For<IWebHostEnvironment>();
        _mockLogger = Substitute.For<ILogger<ServiceRecetteUtilisateur>>();

        _service = new ServiceRecetteUtilisateur(_mockRepository, _mockHostEnvironment, _mockLogger);
    }

    /// <summary>
    /// PERF 9: CreateAsync - Validation overhead
    /// Mesure le coût de la validation avant BD
    /// Baseline: doit rester < 1ms (validation seule)
    /// </summary>
    [Benchmark(Description = "CreateAsync - Validation Overhead")]
    public async Task CreateAsync_ValidationOnly()
    {
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "Performance Test Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "This is a detailed instruction set for testing purposes",
            ImageUrl = "https://example.com/image.jpg",
            Ingredients = new List<string> { "Pasta", "Tomato", "Olive Oil" },
            IsPublic = true
        };

        var createdRecipe = new RecetteUtilisateur
        {
            UserRecipeId = Guid.NewGuid().ToString("N"),
            UserId = TestUserId,
            Title = request.Title,
            Category = request.Category,
            Area = request.Area,
            Instructions = request.Instructions,
            ImageUrl = request.ImageUrl,
            IngredientsJson = System.Text.Json.JsonSerializer.Serialize(request.Ingredients),
            IsPublic = request.IsPublic,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.AddAsync(Arg.Any<RecetteUtilisateur>()).Returns(Task.FromResult(createdRecipe));

        var result = await _service.CreateAsync(TestUserId, request);

        if (result == null)
            throw new Exception("Creation failed");
    }

    /// <summary>
    /// PERF 10: CreateAsync - JSON Serialization impact
    /// Mesure le coût de la sérialisation des ingrédients
    /// Avec 50 ingrédients vs 5
    /// Baseline: doit rester < 2ms même avec beaucoup d'ingrédients
    /// </summary>
    [Benchmark(Description = "CreateAsync - 50 Ingredients")]
    public async Task CreateAsync_ManyIngredients()
    {
        var ingredients = Enumerable.Range(1, 50)
            .Select(i => $"Ingredient {i}")
            .ToList();

        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "Complex Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Detailed instructions",
            ImageUrl = "https://example.com/image.jpg",
            Ingredients = ingredients,
            IsPublic = true
        };

        var createdRecipe = new RecetteUtilisateur
        {
            UserRecipeId = Guid.NewGuid().ToString("N"),
            UserId = TestUserId,
            Title = request.Title,
            Category = request.Category,
            Area = request.Area,
            Instructions = request.Instructions,
            ImageUrl = request.ImageUrl,
            IngredientsJson = System.Text.Json.JsonSerializer.Serialize(request.Ingredients),
            IsPublic = request.IsPublic,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.AddAsync(Arg.Any<RecetteUtilisateur>()).Returns(Task.FromResult(createdRecipe));

        var result = await _service.CreateAsync(TestUserId, request);

        if (result?.Ingredients.Count != 50)
            throw new Exception("Ingredient serialization failed");
    }

    /// <summary>
    /// PERF 11: UpdateAsync - Propriétaire check overhead
    /// Mesure le coût de la vérification du propriétaire
    /// Baseline: doit rester < 1ms
    /// </summary>
    [Benchmark(Description = "UpdateAsync - Owner Check")]
    public async Task UpdateAsync_OwnerCheckOverhead()
    {
        var existingRecipe = new RecetteUtilisateur
        {
            UserRecipeId = "test-recipe",
            UserId = TestUserId,
            Title = "Original",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Original instructions",
            IngredientsJson = @"[""Pasta""]",
            IsPublic = false,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        var updateRequest = new RequeteCreationRecetteUtilisateur
        {
            Title = "Updated",
            Category = "Pizza",
            Area = "Italian",
            Instructions = "Updated instructions",
            ImageUrl = null,
            Ingredients = new List<string> { "Dough" },
            IsPublic = true
        };

        var updatedRecipe = new RecetteUtilisateur
        {
            UserRecipeId = existingRecipe.UserRecipeId,
            UserId = TestUserId,
            Title = updateRequest.Title,
            Category = updateRequest.Category,
            Area = updateRequest.Area,
            Instructions = updateRequest.Instructions,
            ImageUrl = updateRequest.ImageUrl,
            IngredientsJson = System.Text.Json.JsonSerializer.Serialize(updateRequest.Ingredients),
            IsPublic = updateRequest.IsPublic,
            DateCreation = existingRecipe.DateCreation,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync("test-recipe").Returns(Task.FromResult((RecetteUtilisateur?)existingRecipe));
        _mockRepository.UpdateAsync(Arg.Any<RecetteUtilisateur>()).Returns(Task.FromResult((RecetteUtilisateur?)updatedRecipe));

        var result = await _service.UpdateAsync("test-recipe", TestUserId, updateRequest);

        if (result == null)
            throw new Exception("Update failed");
    }

    /// <summary>
    /// PERF 12: DeleteAsync - Propriétaire + Image deletion overhead
    /// Mesure le coût de vérification + suppression d'image
    /// Baseline: doit rester < 5ms
    /// </summary>
    [Benchmark(Description = "DeleteAsync - With Image Deletion")]
    public async Task DeleteAsync_ImageDeletionOverhead()
    {
        var recipeWithImage = new RecetteUtilisateur
        {
            UserRecipeId = "delete-recipe",
            UserId = TestUserId,
            Title = "Recipe with Image",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions",
            ImageUrl = "/uploads/guid_image.jpg",  // External image
            IngredientsJson = "[]",
            IsPublic = true,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync("delete-recipe").Returns(Task.FromResult((RecetteUtilisateur?)recipeWithImage));
        _mockRepository.DeleteAsync("delete-recipe").Returns(Task.FromResult(true));

        var result = await _service.DeleteAsync("delete-recipe", TestUserId);

        if (!result)
            throw new Exception("Deletion failed");
    }

    /// <summary>
    /// PERF 13: GetMyRecipesAsync - Désérialisation d'ingrédients
    /// Mesure l'impact de la désérialisation JSON pour 100 recettes
    /// Baseline: doit rester < 10ms
    /// </summary>
    [Benchmark(Description = "GetMyRecipesAsync - 100 Recipes")]
    public async Task GetMyRecipesAsync_LargeDataSet()
    {
        var recipes = Enumerable.Range(1, 100)
            .Select(i => new RecetteUtilisateur
            {
                UserRecipeId = $"recipe-{i}",
                UserId = TestUserId,
                Title = $"Recipe {i}",
                Category = "Pasta",
                Area = "Italian",
                Instructions = $"Instructions for recipe {i}",
                IngredientsJson = System.Text.Json.JsonSerializer.Serialize(
                    new[] { "Pasta", "Tomato", "Olive Oil", "Garlic", "Basil" }),
                IsPublic = i % 2 == 0,
                DateCreation = DateTime.UtcNow.AddDays(-i),
                DateMaj = DateTime.UtcNow.AddDays(-i)
            }).ToList();

        _mockRepository.GetByUserIdAsync(TestUserId)
            .Returns(Task.FromResult((IEnumerable<RecetteUtilisateur>)recipes));

        var result = await _service.GetMyRecipesAsync(TestUserId);

        if (result.Count() != 100)
            throw new Exception($"Expected 100 recipes, got {result.Count()}");
    }

    /// <summary>
    /// PERF 14: SSRF Validation - URL checking overhead
    /// Mesure le coût de la validation SSRF
    /// Baseline: doit rester < 1ms
    /// </summary>
    [Benchmark(Description = "CreateAsync - SSRF Validation")]
    public async Task CreateAsync_SSRFValidationOverhead()
    {
        // Test avec une URL publique (doit passer validation)
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "SSRF Test",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Test SSRF validation",
            ImageUrl = "https://cdn.example.com/recipes/image.jpg",
            Ingredients = new List<string> { "Pasta" },
            IsPublic = true
        };

        var createdRecipe = new RecetteUtilisateur
        {
            UserRecipeId = Guid.NewGuid().ToString("N"),
            UserId = TestUserId,
            Title = request.Title,
            Category = request.Category,
            Area = request.Area,
            Instructions = request.Instructions,
            ImageUrl = request.ImageUrl,
            IngredientsJson = System.Text.Json.JsonSerializer.Serialize(request.Ingredients),
            IsPublic = request.IsPublic,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.AddAsync(Arg.Any<RecetteUtilisateur>()).Returns(Task.FromResult(createdRecipe));

        var result = await _service.CreateAsync(TestUserId, request);

        if (result == null)
            throw new Exception("SSRF validation failed");
    }

    /// <summary>
    /// PERF 15: Memory allocation - DTO mapping
    /// Mesure les allocations lors de la conversion RecetteUtilisateur -> DTO
    /// Baseline: doit rester < 5KB par conversion
    /// </summary>
    [Benchmark(Description = "DTO Mapping - 100 Conversions")]
    public void DTOMapping_ManyConversions()
    {
        var recipes = Enumerable.Range(1, 100)
            .Select(i => new RecetteUtilisateur
            {
                UserRecipeId = $"recipe-{i}",
                UserId = TestUserId,
                Title = $"Recipe {i}",
                Category = "Pasta",
                Area = "Italian",
                Instructions = $"Instructions",
                IngredientsJson = @"[""Pasta"", ""Tomato""]",
                IsPublic = true,
                DateCreation = DateTime.UtcNow,
                DateMaj = DateTime.UtcNow
            }).ToList();

        // Call private method through reflection or via service
        var dtos = recipes.Select(r => new menuMalin.Shared.Modeles.DTOs.RecetteUtilisateurDTO
        {
            UserRecipeId = r.UserRecipeId,
            UserId = r.UserId,
            Title = r.Title,
            Category = r.Category,
            Area = r.Area,
            Instructions = r.Instructions,
            ImageUrl = r.ImageUrl,
            Ingredients = System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.IngredientsJson) ?? new(),
            IsPublic = r.IsPublic,
            CreatedAt = r.DateCreation,
            UpdatedAt = r.DateMaj
        }).ToList();

        if (dtos.Count != 100)
            throw new Exception("DTO mapping failed");
    }
}
