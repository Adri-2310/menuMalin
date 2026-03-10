using Xunit;
using NSubstitute;
using menuMalin.Server.Services;
using menuMalin.Server.Modeles.Entites;
using menuMalin.Server.Depots.Interfaces;
using menuMalin.Shared.Modeles.Requetes;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace menuMalin.Tests.Server;

/// <summary>
/// Tests unitaires pour ServiceRecetteUtilisateur
/// Teste la création, mise à jour et suppression des recettes utilisateur avec validation
/// </summary>
public class ServiceRecetteUtilisateurTests
{
    private readonly IDepotRecetteUtilisateur _mockRepository;
    private readonly IWebHostEnvironment _mockHostEnvironment;
    private readonly ILogger<ServiceRecetteUtilisateur> _mockLogger;
    private readonly ServiceRecetteUtilisateur _service;

    public ServiceRecetteUtilisateurTests()
    {
        // Créer les mocks
        _mockRepository = Substitute.For<IDepotRecetteUtilisateur>();
        _mockHostEnvironment = Substitute.For<IWebHostEnvironment>();
        _mockLogger = Substitute.For<ILogger<ServiceRecetteUtilisateur>>();

        // Initialiser le service avec les mocks
        _service = new ServiceRecetteUtilisateur(_mockRepository, _mockHostEnvironment, _mockLogger);
    }

    /// <summary>
    /// TEST 1: Créer une recette avec données valides
    /// Scénario: Toutes les données requises sont fournies
    /// Résultat attendu: La recette est créée et retournée en DTO
    /// </summary>
    [Fact]
    public async Task CreateAsync_CreatesRecipe_WhenDataIsValid()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "Spaghetti Carbonara",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Cuire les pâtes, mélanger avec les œufs et le fromage",
            ImageUrl = "https://example.com/carbonara.jpg",
            Ingredients = new List<string> { "Pasta", "Eggs", "Cheese" },
            IsPublic = true
        };

        var createdRecipe = new RecetteUtilisateur
        {
            UserRecipeId = "recipe-123",
            UserId = userId,
            Title = request.Title,
            Category = request.Category,
            Area = request.Area,
            Instructions = request.Instructions,
            ImageUrl = request.ImageUrl,
            IngredientsJson = """["Pasta","Eggs","Cheese"]""",
            IsPublic = request.IsPublic,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.AddAsync(Arg.Any<RecetteUtilisateur>()).Returns(Task.FromResult(createdRecipe));

        // ACT
        var result = await _service.CreateAsync(userId, request);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("recipe-123", result.UserRecipeId);
        Assert.Equal("Spaghetti Carbonara", result.Title);
        Assert.Equal("Pasta", result.Category);
        Assert.True(result.IsPublic);
        await _mockRepository.Received(1).AddAsync(Arg.Any<RecetteUtilisateur>());
    }

    /// <summary>
    /// TEST 2: Validation du titre - titre vide
    /// Scénario: Title est null ou whitespace
    /// Résultat attendu: ArgumentException levée
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsException_WhenTitleIsEmpty()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions valides",
            Ingredients = new List<string>(),
            IsPublic = false
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(userId, request));
        Assert.Contains("titre est requis", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST 3: Validation du titre - trop court
    /// Scénario: Title a moins de 3 caractères
    /// Résultat attendu: ArgumentException levée
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsException_WhenTitleIsTooShort()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "AB",  // 2 caractères, min est 3
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions valides",
            Ingredients = new List<string>(),
            IsPublic = false
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(userId, request));
        Assert.Contains("au moins 3", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST 4: Validation du titre - trop long
    /// Scénario: Title dépasse 200 caractères
    /// Résultat attendu: ArgumentException levée
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsException_WhenTitleIsTooLong()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = new string('A', 201),  // 201 caractères, max est 200
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions valides",
            Ingredients = new List<string>(),
            IsPublic = false
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(userId, request));
        Assert.Contains("ne peut pas dépasser 200", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST 5: Validation des instructions - vides
    /// Scénario: Instructions est null ou whitespace
    /// Résultat attendu: ArgumentException levée
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsException_WhenInstructionsAreEmpty()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "Valid Title",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "",
            Ingredients = new List<string>(),
            IsPublic = false
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(userId, request));
        Assert.Contains("instructions sont requises", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST 6: Validation des instructions - trop courtes
    /// Scénario: Instructions a moins de 5 caractères
    /// Résultat attendu: ArgumentException levée
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsException_WhenInstructionsAreTooShort()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "Valid Title",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "1234",  // 4 caractères, min est 5
            Ingredients = new List<string>(),
            IsPublic = false
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(userId, request));
        Assert.Contains("au moins 5", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST 7: Validation ImageUrl - SSRF protection (adresse privée)
    /// Scénario: ImageUrl pointe vers une adresse IP privée
    /// Résultat attendu: ArgumentException levée
    /// </summary>
    [Fact]
    public async Task CreateAsync_ThrowsException_WhenImageUrlIsPrivateIP()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "Valid Title",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Valid instructions",
            ImageUrl = "http://192.168.1.1/admin",  // IP privée
            Ingredients = new List<string>(),
            IsPublic = false
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(userId, request));
        Assert.Contains("adresses IP privées", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST 8: Validation ImageUrl - acceptable (URL publique)
    /// Scénario: ImageUrl pointe vers un URL public HTTPS
    /// Résultat attendu: Validation acceptée
    /// </summary>
    [Fact]
    public async Task CreateAsync_AcceptsPublicImageUrl()
    {
        // ARRANGE
        var userId = "user-123";
        var request = new RequeteCreationRecetteUtilisateur
        {
            Title = "Valid Title",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Valid instructions",
            ImageUrl = "https://example.com/image.jpg",  // URL publique
            Ingredients = new List<string>(),
            IsPublic = false
        };

        var createdRecipe = new RecetteUtilisateur
        {
            UserRecipeId = "recipe-456",
            UserId = userId,
            Title = request.Title,
            Category = request.Category,
            Area = request.Area,
            Instructions = request.Instructions,
            ImageUrl = request.ImageUrl,
            IngredientsJson = "[]",
            IsPublic = request.IsPublic,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.AddAsync(Arg.Any<RecetteUtilisateur>()).Returns(Task.FromResult(createdRecipe));

        // ACT
        var result = await _service.CreateAsync(userId, request);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("https://example.com/image.jpg", result.ImageUrl);
    }

    /// <summary>
    /// TEST 9: Récupérer les recettes de l'utilisateur
    /// Scénario: Utilisateur a 2 recettes
    /// Résultat attendu: Liste de 2 recettes retournée
    /// </summary>
    [Fact]
    public async Task GetMyRecipesAsync_ReturnsUserRecipes()
    {
        // ARRANGE
        var userId = "user-123";
        var recipes = new List<RecetteUtilisateur>
        {
            new RecetteUtilisateur
            {
                UserRecipeId = "recipe-1",
                UserId = userId,
                Title = "Pasta",
                Category = "Pasta",
                Area = "Italian",
                Instructions = "Cuire les pâtes",
                IngredientsJson = """["Pâtes"]""",
                IsPublic = true,
                DateCreation = DateTime.UtcNow,
                DateMaj = DateTime.UtcNow
            },
            new RecetteUtilisateur
            {
                UserRecipeId = "recipe-2",
                UserId = userId,
                Title = "Pizza",
                Category = "Pizza",
                Area = "Italian",
                Instructions = "Cuire la pizza",
                IngredientsJson = """["Farine","Sauce"]""",
                IsPublic = false,
                DateCreation = DateTime.UtcNow,
                DateMaj = DateTime.UtcNow
            }
        };

        _mockRepository.GetByUserIdAsync(userId).Returns(Task.FromResult((IEnumerable<RecetteUtilisateur>)recipes));

        // ACT
        var result = await _service.GetMyRecipesAsync(userId);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Pasta", result.First().Title);
        Assert.Equal("Pizza", result.Last().Title);
    }

    /// <summary>
    /// TEST 10: Supprimer une recette - propriétaire autorisé
    /// Scénario: Utilisateur supprime sa propre recette
    /// Résultat attendu: Recette supprimée, méthode retourne true
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeletesRecipe_WhenOwnerMatches()
    {
        // ARRANGE
        var userId = "user-123";
        var recipeId = "recipe-456";
        var recipe = new RecetteUtilisateur
        {
            UserRecipeId = recipeId,
            UserId = userId,
            Title = "My Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions",
            ImageUrl = "https://example.com/image.jpg",
            IngredientsJson = "[]",
            IsPublic = true,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync(recipeId).Returns(Task.FromResult(recipe));
        _mockRepository.DeleteAsync(recipeId).Returns(Task.FromResult(true));

        // ACT
        var result = await _service.DeleteAsync(recipeId, userId);

        // ASSERT
        Assert.True(result);
        await _mockRepository.Received(1).DeleteAsync(recipeId);
    }

    /// <summary>
    /// TEST 11: Supprimer une recette - propriétaire non autorisé
    /// Scénario: Utilisateur tente de supprimer la recette d'un autre utilisateur
    /// Résultat attendu: UnauthorizedAccessException levée
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenOwnerDoesNotMatch()
    {
        // ARRANGE
        var userId = "user-123";
        var wrongUserId = "user-789";
        var recipeId = "recipe-456";
        var recipe = new RecetteUtilisateur
        {
            UserRecipeId = recipeId,
            UserId = wrongUserId,  // Propriétaire différent
            Title = "Someone Else's Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions",
            IngredientsJson = "[]",
            IsPublic = true,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync(recipeId).Returns(Task.FromResult(recipe));

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteAsync(recipeId, userId));
        Assert.Contains("pas autorisé", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// TEST 12: Supprimer une recette - recette non trouvée
    /// Scénario: Recette n'existe pas
    /// Résultat attendu: Méthode retourne false
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenRecipeNotFound()
    {
        // ARRANGE
        var userId = "user-123";
        var recipeId = "nonexistent-recipe";

        _mockRepository.GetByIdAsync(recipeId).Returns(Task.FromResult((RecetteUtilisateur?)null));

        // ACT
        var result = await _service.DeleteAsync(recipeId, userId);

        // ASSERT
        Assert.False(result);
        await _mockRepository.DidNotReceive().DeleteAsync(Arg.Any<string>());
    }

    /// <summary>
    /// TEST 13: Mettre à jour une recette - succès
    /// Scénario: Utilisateur met à jour sa propre recette avec données valides
    /// Résultat attendu: Recette mise à jour retournée
    /// </summary>
    [Fact]
    public async Task UpdateAsync_UpdatesRecipe_WhenDataIsValidAndOwnerMatches()
    {
        // ARRANGE
        var userId = "user-123";
        var recipeId = "recipe-456";
        var existingRecipe = new RecetteUtilisateur
        {
            UserRecipeId = recipeId,
            UserId = userId,
            Title = "Old Title",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Old instructions",
            ImageUrl = "https://old.com/image.jpg",
            IngredientsJson = """["Pasta"]""",
            IsPublic = false,
            DateCreation = DateTime.UtcNow.AddDays(-1),
            DateMaj = DateTime.UtcNow.AddDays(-1)
        };

        var updateRequest = new RequeteCreationRecetteUtilisateur
        {
            Title = "New Title",
            Category = "Pizza",
            Area = "Italian",
            Instructions = "New instructions that are very detailed",
            ImageUrl = "https://new.com/image.jpg",
            Ingredients = new List<string> { "Flour", "Cheese" },
            IsPublic = true
        };

        var updatedRecipe = new RecetteUtilisateur
        {
            UserRecipeId = recipeId,
            UserId = userId,
            Title = updateRequest.Title,
            Category = updateRequest.Category,
            Area = updateRequest.Area,
            Instructions = updateRequest.Instructions,
            ImageUrl = updateRequest.ImageUrl,
            IngredientsJson = """["Flour","Cheese"]""",
            IsPublic = updateRequest.IsPublic,
            DateCreation = existingRecipe.DateCreation,
            DateMaj = DateTime.UtcNow
        };

        _mockRepository.GetByIdAsync(recipeId).Returns(Task.FromResult((RecetteUtilisateur?)existingRecipe));
        _mockRepository.UpdateAsync(Arg.Any<RecetteUtilisateur>()).Returns(Task.FromResult((RecetteUtilisateur?)updatedRecipe));

        // ACT
        var result = await _service.UpdateAsync(recipeId, userId, updateRequest);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("Pizza", result.Category);
        Assert.True(result.IsPublic);
        await _mockRepository.Received(1).UpdateAsync(Arg.Any<RecetteUtilisateur>());
    }

    /// <summary>
    /// TEST 14: Mettre à jour une recette - propriétaire non autorisé
    /// Scénario: Utilisateur tente de mettre à jour la recette d'un autre
    /// Résultat attendu: UnauthorizedAccessException levée
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenOwnerDoesNotMatch()
    {
        // ARRANGE
        var userId = "user-123";
        var wrongUserId = "user-789";
        var recipeId = "recipe-456";
        var existingRecipe = new RecetteUtilisateur
        {
            UserRecipeId = recipeId,
            UserId = wrongUserId,  // Propriétaire différent
            Title = "Someone Else's Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions",
            IngredientsJson = "[]",
            IsPublic = true,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        var updateRequest = new RequeteCreationRecetteUtilisateur
        {
            Title = "Updated Title",
            Category = "Pizza",
            Area = "Italian",
            Instructions = "Updated instructions",
            Ingredients = new List<string>(),
            IsPublic = true
        };

        _mockRepository.GetByIdAsync(recipeId).Returns(Task.FromResult((RecetteUtilisateur?)existingRecipe));

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UpdateAsync(recipeId, userId, updateRequest));
        Assert.Contains("pas autorisé", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
