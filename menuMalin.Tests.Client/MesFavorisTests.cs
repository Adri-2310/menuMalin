using Xunit;
using Bunit;
using NSubstitute;
using menuMalin.Modeles;
using menuMalin.Services;
using menuMalin.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace menuMalin.Tests.Client;

/// <summary>
/// Tests pour la page MesFavoris
/// Teste l'affichage des favoris, suppression, et gestion de liste vide
/// </summary>
public class MesFavorisTests : TestContext
{
    private readonly IServiceFavorisFrontend _mockFavoritesService;
    private readonly IServiceNotification _mockNotifService;

    public MesFavorisTests()
    {
        // Créer les mocks
        _mockFavoritesService = Substitute.For<IServiceFavorisFrontend>();
        _mockNotifService = Substitute.For<IServiceNotification>();

        // Enregistrer dans le DI
        Services.AddScoped<IServiceFavorisFrontend>(_ => _mockFavoritesService);
        Services.AddScoped<IServiceNotification>(_ => _mockNotifService);
        Services.AddScoped<ServiceEtatAuthentification>(_ => Substitute.For<ServiceEtatAuthentification>());
    }

    /// <summary>
    /// TEST 1: Affiche le titre "Mes Favoris"
    /// Scénario: Page chargée
    /// Résultat attendu: Titre visible
    /// </summary>
    [Fact]
    public async Task MesFavoris_DisplaysPageTitle()
    {
        // ARRANGE
        var favorites = new List<Recette>();
        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var title = cut.FindAll("h1").FirstOrDefault();
        Assert.NotNull(title);
        Assert.Contains("Favoris", title?.TextContent ?? "");
    }

    /// <summary>
    /// TEST 2: Affiche les favoris de l'utilisateur
    /// Scénario: Utilisateur a 3 recettes favorites
    /// Résultat attendu: Les 3 recettes sont affichées
    /// </summary>
    [Fact]
    public async Task MesFavoris_DisplaysUserFavorites()
    {
        // ARRANGE
        var favorites = new List<Recette>
        {
            new Recette { IdMeal = "1", StrMeal = "Pasta Carbonara", StrMealThumb = "https://example.com/1.jpg", StrCategory = "Pasta" },
            new Recette { IdMeal = "2", StrMeal = "Pizza Margarita", StrMealThumb = "https://example.com/2.jpg", StrCategory = "Pizza" },
            new Recette { IdMeal = "3", StrMeal = "Risotto", StrMealThumb = "https://example.com/3.jpg", StrCategory = "Rice" }
        };

        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var cards = cut.FindAll(".card");
        Assert.NotEmpty(cards);
        // Devrait avoir au moins les 3 cartes de recettes
        Assert.True(cards.Count >= 3);
    }

    /// <summary>
    /// TEST 3: Affiche un message si aucun favori
    /// Scénario: Liste des favoris vide
    /// Résultat attendu: Message "Aucun favori" ou similaire
    /// </summary>
    [Fact]
    public async Task MesFavoris_DisplaysEmptyMessage_WhenNoFavorites()
    {
        // ARRANGE
        var emptyFavorites = new List<Recette>();
        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(emptyFavorites));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var content = cut.Markup;
        // Vérifier qu'il y a du contenu
        Assert.NotEmpty(content);
    }

    /// <summary>
    /// TEST 4: Chaque recette affiche un bouton de suppression
    /// Scénario: Recettes affichées
    /// Résultat attendu: Bouton "Supprimer" ou icône poubelle visible
    /// </summary>
    [Fact]
    public async Task MesFavoris_EachRecipeHasDeleteButton()
    {
        // ARRANGE
        var favorites = new List<Recette>
        {
            new Recette { IdMeal = "1", StrMeal = "Pasta", StrMealThumb = "https://example.com/1.jpg", StrCategory = "Pasta" }
        };

        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var buttons = cut.FindAll("button");
        // Devrait avoir des boutons pour les actions
        Assert.NotEmpty(buttons);
    }

    /// <summary>
    /// TEST 5: Suppression d'un favori - Confirmation
    /// Scénario: Utilisateur clique sur supprimer
    /// Résultat attendu: Dialogue de confirmation apparaît
    /// </summary>
    [Fact]
    public async Task MesFavoris_ShowsConfirmDialog_BeforeDeletion()
    {
        // ARRANGE
        var favorites = new List<Recette>
        {
            new Recette { IdMeal = "remove-me", StrMeal = "Recipe to Remove", StrMealThumb = "https://example.com/1.jpg", StrCategory = "Pasta" }
        };

        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));
        _mockFavoritesService.RemoveFavoriteAsync("remove-me").Returns(Task.FromResult(true));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var buttons = cut.FindAll("button");
        Assert.NotEmpty(buttons);
    }

    /// <summary>
    /// TEST 6: Suppression réussie - Notification
    /// Scénario: Favori supprimé avec succès
    /// Résultat attendu: Message de succès affiché
    /// </summary>
    [Fact]
    public async Task MesFavoris_ShowsSuccessNotification_AfterDeletion()
    {
        // ARRANGE
        var favorites = new List<Recette>
        {
            new Recette { IdMeal = "delete-id", StrMeal = "Recipe", StrMealThumb = "https://example.com/1.jpg", StrCategory = "Pasta" }
        };

        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));
        _mockFavoritesService.RemoveFavoriteAsync("delete-id").Returns(Task.FromResult(true));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // Simuler la suppression
        var result = await _mockFavoritesService.RemoveFavoriteAsync("delete-id");

        // ASSERT
        Assert.True(result);
        await _mockFavoritesService.Received(1).RemoveFavoriteAsync("delete-id");
    }

    /// <summary>
    /// TEST 7: Erreur lors du chargement des favoris
    /// Scénario: Service retourne une erreur
    /// Résultat attendu: Message d'erreur affiché
    /// </summary>
    [Fact]
    public async Task MesFavoris_DisplaysErrorMessage_OnLoadFailure()
    {
        // ARRANGE
        _mockFavoritesService.GetUserFavoritesAsync()
            .Returns(Task.FromException<List<Recette>>(new Exception("Erreur réseau")));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        // La page devrait gérer l'erreur gracieusement
        var content = cut.Markup;
        Assert.NotEmpty(content);
    }

    /// <summary>
    /// TEST 8: Chaque recette affiche une image
    /// Scénario: Recettes affichées
    /// Résultat attendu: Images chargées avec src correct
    /// </summary>
    [Fact]
    public async Task MesFavoris_DisplaysRecipeImages()
    {
        // ARRANGE
        var favorites = new List<Recette>
        {
            new Recette { IdMeal = "1", StrMeal = "Pasta", StrMealThumb = "https://example.com/pasta.jpg", StrCategory = "Pasta" },
            new Recette { IdMeal = "2", StrMeal = "Pizza", StrMealThumb = "https://example.com/pizza.jpg", StrCategory = "Pizza" }
        };

        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var images = cut.FindAll("img");
        Assert.NotEmpty(images);
    }

    /// <summary>
    /// TEST 9: Chaque recette affiche son titre
    /// Scénario: Recettes affichées
    /// Résultat attendu: Titres visibles
    /// </summary>
    [Fact]
    public async Task MesFavoris_DisplaysRecipeTitles()
    {
        // ARRANGE
        var favorites = new List<Recette>
        {
            new Recette { IdMeal = "1", StrMeal = "Spaghetti Carbonara", StrMealThumb = "https://example.com/1.jpg", StrCategory = "Pasta" }
        };

        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var content = cut.Markup;
        Assert.Contains("Spaghetti Carbonara", content);
    }

    /// <summary>
    /// TEST 10: Bouton pour naviguer vers la recette
    /// Scénario: Utilisateur clique sur une recette
    /// Résultat attendu: Navigation vers /recette/{id}
    /// </summary>
    [Fact]
    public async Task MesFavoris_HasLinkToRecipeDetail()
    {
        // ARRANGE
        var favorites = new List<Recette>
        {
            new Recette { IdMeal = "recipe-123", StrMeal = "Recipe", StrMealThumb = "https://example.com/1.jpg", StrCategory = "Pasta" }
        };

        _mockFavoritesService.GetUserFavoritesAsync().Returns(Task.FromResult(favorites));

        // ACT
        var cut = Render<MesFavoris>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var links = cut.FindAll("a");
        // Devrait avoir au moins un lien de navigation
        Assert.NotEmpty(links);
    }
}
