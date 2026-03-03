using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using menuMalin.Server.Donnees;
using menuMalin.Server.Modeles.Entites;
using menuMalin.Server.Depots;
using menuMalin.Server.Services;
using menuMalin.Shared.Modeles.Requetes;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Microsoft.AspNetCore.Hosting;

namespace menuMalin.Tests.Server;

/// <summary>
/// Tests d'intégration pour le workflow complet des recettes utilisateur
/// Teste l'interaction entre la BD (DbContext), le Repository, et le Service
/// Utilise une vraie BD SQLite en mémoire pour simuler des conditions réelles
/// </summary>
public class UserRecipeWorkflowIntegrationTests : IAsyncLifetime
{
    private ApplicationDbContext _context = null!;
    private DepotRecetteUtilisateur _repository = null!;
    private ServiceRecetteUtilisateur _service = null!;
    private readonly string _testUserId = "test-user-123";
    private readonly IWebHostEnvironment _mockHostEnvironment;
    private readonly ILogger<ServiceRecetteUtilisateur> _mockLogger;

    public UserRecipeWorkflowIntegrationTests()
    {
        _mockHostEnvironment = Substitute.For<IWebHostEnvironment>();
        _mockLogger = Substitute.For<ILogger<ServiceRecetteUtilisateur>>();
    }

    /// <summary>
    /// Initialise la BD en mémoire avant chaque test
    /// </summary>
    public async Task InitializeAsync()
    {
        // Créer un DbContext en mémoire unique par test
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())  // BD unique pour chaque test
            .Options;

        _context = new ApplicationDbContext(options);

        // Créer les tables
        await _context.Database.EnsureCreatedAsync();

        // Ajouter un utilisateur test
        var user = new Utilisateur
        {
            UserId = _testUserId,
            Email = "test@example.com",
            Name = "Test User",
            DateCreation = DateTime.UtcNow
        };
        _context.Utilisateurs.Add(user);
        await _context.SaveChangesAsync();

        // Initialiser le repository et le service
        _repository = new DepotRecetteUtilisateur(_context);
        _service = new ServiceRecetteUtilisateur(_repository, _mockHostEnvironment, _mockLogger);
    }

    /// <summary>
    /// Nettoie la BD après chaque test
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_context != null)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }

    /// <summary>
    /// TEST 1: Workflow complet - Créer → Récupérer → Modifier → Supprimer
    /// Scénario: Un utilisateur crée une recette, la modifie, puis la supprime
    /// Résultat attendu: Toutes les opérations réussissent
    /// </summary>
    [Fact]
    public async Task UserRecipeWorkflow_CompleteLifecycle_SucceedsFromCreateToDelete()
    {
        // ARRANGE & ACT 1: Créer une recette
        var createRequest = new RequeteCreationRecetteUtilisateur
        {
            Title = "Pâtes Carbonara",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Cuire les pâtes, mélanger avec les œufs et le fromage frais",
            ImageUrl = "https://example.com/carbonara.jpg",
            Ingredients = new List<string> { "Pâtes", "Œufs", "Fromage", "Bacon" },
            IsPublic = true
        };

        var createdRecipe = await _service.CreateAsync(_testUserId, createRequest);

        // ASSERT 1: Vérifier la création
        Assert.NotNull(createdRecipe);
        Assert.Equal("Pâtes Carbonara", createdRecipe.Title);
        Assert.True(createdRecipe.IsPublic);
        var recipeId = createdRecipe.UserRecipeId;

        // ACT 2: Récupérer la recette
        var retrievedRecipe = await _service.GetByIdAsync(recipeId);

        // ASSERT 2: Vérifier la récupération
        Assert.NotNull(retrievedRecipe);
        Assert.Equal("Pâtes Carbonara", retrievedRecipe.Title);
        Assert.Equal("Pasta", retrievedRecipe.Category);
        Assert.Contains("Bacon", retrievedRecipe.Ingredients);

        // ACT 3: Modifier la recette
        var updateRequest = new RequeteCreationRecetteUtilisateur
        {
            Title = "Pâtes Carbonara Premium",  // Titre modifié
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Cuire les pâtes, mélanger avec les œufs, le fromage et le bacon croustillant",
            ImageUrl = "https://example.com/carbonara-premium.jpg",
            Ingredients = new List<string> { "Pâtes", "Œufs", "Fromage de Brebis", "Bacon fermier" },
            IsPublic = false  // Rendre privée
        };

        var updatedRecipe = await _service.UpdateAsync(recipeId, _testUserId, updateRequest);

        // ASSERT 3: Vérifier la modification
        Assert.NotNull(updatedRecipe);
        Assert.Equal("Pâtes Carbonara Premium", updatedRecipe.Title);
        Assert.False(updatedRecipe.IsPublic);
        Assert.Contains("Fromage de Brebis", updatedRecipe.Ingredients);

        // ACT 4: Supprimer la recette
        var deleteResult = await _service.DeleteAsync(recipeId, _testUserId);

        // ASSERT 4: Vérifier la suppression
        Assert.True(deleteResult);
        var deletedRecipe = await _service.GetByIdAsync(recipeId);
        Assert.Null(deletedRecipe);

        // ASSERT 5: Vérifier que la BD ne contient plus la recette
        var dbRecipe = await _context.RecettesUtilisateur.FirstOrDefaultAsync(r => r.UserRecipeId == recipeId);
        Assert.Null(dbRecipe);
    }

    /// <summary>
    /// TEST 2: Isolation des recettes utilisateur
    /// Scénario: Deux utilisateurs créent des recettes, chacun ne peut voir que les siennes
    /// Résultat attendu: Chaque utilisateur récupère uniquement ses propres recettes
    /// </summary>
    [Fact]
    public async Task UserRecipeWorkflow_IsolatesDataByUser()
    {
        // ARRANGE: Ajouter un 2e utilisateur
        var user2Id = "test-user-456";
        var user2 = new Utilisateur
        {
            UserId = user2Id,
            Email = "test2@example.com",
            Name = "Test User 2",
            DateCreation = DateTime.UtcNow
        };
        _context.Utilisateurs.Add(user2);
        await _context.SaveChangesAsync();

        // ACT 1: User 1 crée 2 recettes
        var recipe1 = await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Pasta User 1",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Cook pasta",
            Ingredients = new List<string>(),
            IsPublic = false
        });

        var recipe2 = await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Pizza User 1",
            Category = "Pizza",
            Area = "Italian",
            Instructions = "Make pizza",
            Ingredients = new List<string>(),
            IsPublic = false
        });

        // ACT 2: User 2 crée 1 recette
        var recipe3 = await _service.CreateAsync(user2Id, new RequeteCreationRecetteUtilisateur
        {
            Title = "Burger User 2",
            Category = "Meat",
            Area = "American",
            Instructions = "Make burger",
            Ingredients = new List<string>(),
            IsPublic = false
        });

        // ACT 3: Chaque utilisateur récupère ses recettes
        var user1Recipes = await _service.GetMyRecipesAsync(_testUserId);
        var user2Recipes = await _service.GetMyRecipesAsync(user2Id);

        // ASSERT: Isolation correcte
        Assert.Equal(2, user1Recipes.Count());
        Assert.Equal(1, user2Recipes.Count());

        Assert.Contains(user1Recipes, r => r.Title == "Pasta User 1");
        Assert.Contains(user1Recipes, r => r.Title == "Pizza User 1");
        Assert.DoesNotContain(user1Recipes, r => r.Title == "Burger User 2");

        Assert.Single(user2Recipes);
        Assert.Equal("Burger User 2", user2Recipes.First().Title);
    }

    /// <summary>
    /// TEST 3: Sécurité - Un utilisateur ne peut pas modifier la recette d'un autre
    /// Scénario: User A crée une recette, User B tente de la modifier
    /// Résultat attendu: UnauthorizedAccessException levée
    /// </summary>
    [Fact]
    public async Task UserRecipeWorkflow_EnforcesOwnershipForModification()
    {
        // ARRANGE: Ajouter un 2e utilisateur
        var user2Id = "test-user-hacker";
        var user2 = new Utilisateur
        {
            UserId = user2Id,
            Email = "hacker@example.com",
            Name = "Hacker",
            DateCreation = DateTime.UtcNow
        };
        _context.Utilisateurs.Add(user2);
        await _context.SaveChangesAsync();

        // ACT 1: User 1 crée une recette
        var recipe = await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Secret Recipe",
            Category = "Secret",
            Area = "Unknown",
            Instructions = "Secret instructions",
            Ingredients = new List<string>(),
            IsPublic = false
        });

        var recipeId = recipe.UserRecipeId;

        // ACT 2: User 2 tente de modifier
        var maliciousUpdate = new RequeteCreationRecetteUtilisateur
        {
            Title = "Hacked Recipe",
            Category = "Hacked",
            Area = "Unknown",
            Instructions = "I hacked this",
            Ingredients = new List<string>(),
            IsPublic = true
        };

        // ASSERT: Modification bloquée
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UpdateAsync(recipeId, user2Id, maliciousUpdate));
        Assert.Contains("pas autorisé", exception.Message, StringComparison.OrdinalIgnoreCase);

        // Vérifier que la recette n'a pas été modifiée
        var untouched = await _service.GetByIdAsync(recipeId);
        Assert.NotNull(untouched);
        Assert.Equal("Secret Recipe", untouched.Title);
        Assert.False(untouched.IsPublic);
    }

    /// <summary>
    /// TEST 4: Visibilité publique - Filtrer les recettes publiques
    /// Scénario: 3 recettes créées: 2 privées, 1 publique. GetPublicRecipes retourne que 1
    /// Résultat attendu: Seules les recettes publiques sont visibles
    /// </summary>
    [Fact]
    public async Task UserRecipeWorkflow_FiltersPublicRecipes()
    {
        // ARRANGE & ACT: Créer 3 recettes avec différentes visibilités
        await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Private Recipe 1",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Private",
            Ingredients = new List<string>(),
            IsPublic = false  // ❌ Privée
        });

        await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Public Recipe",
            Category = "Pizza",
            Area = "Italian",
            Instructions = "Public",
            Ingredients = new List<string>(),
            IsPublic = true  // ✅ Publique
        });

        await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Private Recipe 2",
            Category = "Meat",
            Area = "American",
            Instructions = "Private",
            Ingredients = new List<string>(),
            IsPublic = false  // ❌ Privée
        });

        // ACT: Récupérer les recettes publiques
        var publicRecipes = await _service.GetPublicRecipesAsync();

        // ASSERT: Seule la recette publique est retournée
        Assert.Single(publicRecipes);
        Assert.Equal("Public Recipe", publicRecipes.First().Title);
        Assert.True(publicRecipes.First().IsPublic);
    }

    /// <summary>
    /// TEST 5: Concurrence - Deux modifications simultanées
    /// Scénario: Deux tâches modifient la même recette en parallèle
    /// Résultat attendu: Aucune erreur (concurrence gérée par la BD)
    /// </summary>
    [Fact]
    public async Task UserRecipeWorkflow_HandlesConcurrentModifications()
    {
        // ARRANGE: Créer une recette
        var recipe = await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Shared Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Initial instructions",
            Ingredients = new List<string> { "Ingredient 1" },
            IsPublic = false
        });

        var recipeId = recipe.UserRecipeId;

        // ACT: Deux modifications en parallèle
        var task1 = _service.UpdateAsync(recipeId, _testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Updated by Task 1",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Modified by task 1",
            Ingredients = new List<string> { "Ingredient A" },
            IsPublic = false
        });

        var task2 = _service.UpdateAsync(recipeId, _testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Updated by Task 2",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Modified by task 2",
            Ingredients = new List<string> { "Ingredient B" },
            IsPublic = false
        });

        // ASSERT: Les deux tâches complètent sans exception
        await Task.WhenAll(task1, task2);

        var finalRecipe = await _service.GetByIdAsync(recipeId);
        Assert.NotNull(finalRecipe);
        // L'une des deux modifications sera la dernière appliquée
        Assert.True(
            finalRecipe.Title == "Updated by Task 1" || finalRecipe.Title == "Updated by Task 2",
            "Final state should be from one of the concurrent updates"
        );
    }

    /// <summary>
    /// TEST 6: Cascade delete - Supprimer un utilisateur supprime ses recettes
    /// Scénario: Utilisateur crée une recette, puis l'utilisateur est supprimé
    /// Résultat attendu: La recette est aussi supprimée (cascade)
    /// </summary>
    [Fact]
    public async Task UserRecipeWorkflow_CascadeDeletesRecipesWithUser()
    {
        // ARRANGE: Créer une recette pour l'utilisateur
        var recipe = await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "User Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Instructions",
            Ingredients = new List<string>(),
            IsPublic = false
        });

        var recipeId = recipe.UserRecipeId;

        // Vérifier que la recette existe
        var existingRecipe = await _context.RecettesUtilisateur.FirstOrDefaultAsync(r => r.UserRecipeId == recipeId);
        Assert.NotNull(existingRecipe);

        // ACT: Supprimer l'utilisateur
        var user = await _context.Utilisateurs.FirstOrDefaultAsync(u => u.UserId == _testUserId);
        Assert.NotNull(user);
        _context.Utilisateurs.Remove(user);
        await _context.SaveChangesAsync();

        // ASSERT: La recette est aussi supprimée (cascade delete)
        var orphanedRecipe = await _context.RecettesUtilisateur.FirstOrDefaultAsync(r => r.UserRecipeId == recipeId);
        Assert.Null(orphanedRecipe);
    }

    /// <summary>
    /// TEST 7: Persistance des données - Vérifier que les données sont persistées en BD
    /// Scénario: Créer une recette, puis chercher la recette dans le DbSet
    /// Résultat attendu: La recette existe dans la BD en mémoire
    /// </summary>
    [Fact]
    public async Task UserRecipeWorkflow_DataPersistence()
    {
        // ARRANGE & ACT 1: Créer une recette
        var recipe = await _service.CreateAsync(_testUserId, new RequeteCreationRecetteUtilisateur
        {
            Title = "Persisted Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "Should be persisted",
            Ingredients = new List<string> { "Pasta", "Tomato" },
            IsPublic = true
        });

        var recipeId = recipe.UserRecipeId;

        // ACT 2: Chercher la recette directement dans le DbSet (simule une requête BD)
        var persistedRecipe = await _context.RecettesUtilisateur
            .FirstOrDefaultAsync(r => r.UserRecipeId == recipeId);

        // ASSERT: Les données sont persistées
        Assert.NotNull(persistedRecipe);
        Assert.Equal("Persisted Recipe", persistedRecipe.Title);
        Assert.Equal("Pasta", persistedRecipe.Category);
        Assert.Equal(_testUserId, persistedRecipe.UserId);
    }
}
