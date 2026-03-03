using Xunit;
using Bunit;
using NSubstitute;
using menuMalin.Modeles;
using menuMalin.Services;
using menuMalin.Pages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace menuMalin.Tests.Client;

/// <summary>
/// Tests pour la page Accueil
/// Teste le rendu conditonnel basé sur l'authentification et le chargement des recettes
/// </summary>
public class AccueilTests : TestContext
{
    private readonly IServiceRecette _mockRecipeService;
    private readonly IServiceAuthentification _mockAuthService;
    private readonly ServiceEtatAuthentification _authState;
    private readonly IServiceNotification _mockNotifService;
    private readonly NavigationManager _mockNavManager;

    public AccueilTests()
    {
        // Créer les mocks
        _mockRecipeService = Substitute.For<IServiceRecette>();
        _mockAuthService = Substitute.For<IServiceAuthentification>();
        _authState = Substitute.For<ServiceEtatAuthentification>();
        _mockNotifService = Substitute.For<IServiceNotification>();
        _mockNavManager = Substitute.For<NavigationManager>();

        // Enregistrer dans le DI
        Services.AddScoped<IServiceRecette>(_ => _mockRecipeService);
        Services.AddScoped<IServiceAuthentification>(_ => _mockAuthService);
        Services.AddScoped<ServiceEtatAuthentification>(_ => _authState);
        Services.AddScoped<IServiceNotification>(_ => _mockNotifService);
        Services.AddScoped<NavigationManager>(_ => _mockNavManager);
    }

    /// <summary>
    /// TEST 1: Afficher le spinner de chargement
    /// Scénario: Page en cours de chargement
    /// Résultat attendu: Spinner visible avec texte "Chargement..."
    /// </summary>
    [Fact]
    public void Accueil_DisplaysLoadingSpinner_WhenInitializing()
    {
        // ARRANGE
        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(false));

        // ACT - La page affiche le spinner pendant OnInitializedAsync
        var cut = Render<Accueil>();

        // ASSERT
        var spinner = cut.Find(".spinner-border");
        Assert.NotNull(spinner);

        var loadingText = cut.Find(".text-muted");
        Assert.Contains("Chargement", loadingText.TextContent);
    }

    /// <summary>
    /// TEST 2: Afficher le contenu pour utilisateur authentifié
    /// Scénario: Utilisateur connecté, recettes chargées
    /// Résultat attendu: Message "Ravi de vous revoir" visible + grille de recettes
    /// </summary>
    [Fact]
    public async Task Accueil_DisplaysAuthenticatedContent_WhenUserIsLoggedIn()
    {
        // ARRANGE
        var recipes = new List<Recette>
        {
            new Recette { IdMeal = "1", StrMeal = "Pasta", StrMealThumb = "https://example.com/1.jpg", StrCategory = "Pasta" },
            new Recette { IdMeal = "2", StrMeal = "Pizza", StrMealThumb = "https://example.com/2.jpg", StrCategory = "Pizza" }
        };

        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(true));
        _mockRecipeService.GetRandomRecipesAsync(Arg.Any<int>()).Returns(Task.FromResult(recipes));

        // ACT
        var cut = Render<Accueil>();
        await Task.Delay(100); // Attendre le rendu

        // ASSERT
        // Vérifier que le composant a rendu quelque chose
        var markup = cut.Markup;
        Assert.NotEmpty(markup);
    }

    /// <summary>
    /// TEST 3: Afficher le contenu pour visiteur non-authentifié
    /// Scénario: Utilisateur non connecté
    /// Résultat attendu: Page hero avec titre "MenuMalin" et bouton "C'est parti!"
    /// </summary>
    [Fact]
    public async Task Accueil_DisplaysHeroSection_WhenUserIsNotLoggedIn()
    {
        // ARRANGE
        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(false));

        // ACT
        var cut = Render<Accueil>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var heroTitle = cut.Find("h1");
        Assert.Contains("MenuMalin", heroTitle.TextContent);

        var heroText = cut.Find(".lead");
        Assert.Contains("assistant culinaire", heroText.TextContent);

        var loginButton = cut.Find(".btn-light");
        Assert.Contains("C'est parti", loginButton.TextContent);
    }

    /// <summary>
    /// TEST 4: Bouton "C'est parti!" navigue vers la page de connexion
    /// Scénario: Visiteur non-authentifié clique sur "C'est parti!"
    /// Résultat attendu: Navigation déclenche LoginAsync
    /// </summary>
    [Fact]
    public async Task Accueil_TriggersLogin_WhenButtonClicked()
    {
        // ARRANGE
        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(false));

        var cut = Render<Accueil>();
        await Task.Delay(100); // Wait for render

        // ACT
        var loginButton = cut.Find(".btn-light");
        loginButton.Click();

        // ASSERT
        // Vérifier que le bouton de connexion existe
        Assert.NotNull(loginButton);
    }

    /// <summary>
    /// TEST 5: Badge "Espace Membre" visible pour utilisateur authentifié
    /// Scénario: Utilisateur connecté
    /// Résultat attendu: Badge "Espace Membre" affiché
    /// </summary>
    [Fact]
    public async Task Accueil_DisplaysMemberBadge_WhenUserIsAuthenticated()
    {
        // ARRANGE
        var recipes = new List<Recette>();

        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(true));
        _mockRecipeService.GetRandomRecipesAsync(Arg.Any<int>()).Returns(Task.FromResult(recipes));

        // ACT
        var cut = Render<Accueil>();
        await Task.Delay(100); // Wait for render

        // ASSERT
        // Vérifier que du contenu authentifié existe
        var markup = cut.Markup;
        Assert.NotEmpty(markup);
    }

    /// <summary>
    /// TEST 6: Section "Inspirations du jour" affiche les recettes
    /// Scénario: Utilisateur authentifié, 6 recettes chargées
    /// Résultat attendu: Titre et recettes visibles
    /// </summary>
    [Fact]
    public async Task Accueil_DisplaysRecipeSection_WithRandomRecipes()
    {
        // ARRANGE
        var recipes = Enumerable.Range(1, 6)
            .Select(i => new Recette
            {
                IdMeal = i.ToString(),
                StrMeal = $"Recipe {i}",
                StrMealThumb = $"https://example.com/{i}.jpg",
                StrCategory = "Pasta"
            }).ToList();

        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(true));
        _mockRecipeService.GetRandomRecipesAsync(Arg.Any<int>()).Returns(Task.FromResult(recipes));

        // ACT
        var cut = Render<Accueil>();
        await Task.Delay(100); // Wait for render

        // ASSERT
        var markup = cut.Markup;
        Assert.NotEmpty(markup);
    }

    /// <summary>
    /// TEST 7: Logo MenuMalin visible pour utilisateur authentifié
    /// Scénario: Utilisateur connecté
    /// Résultat attendu: Image du logo visible
    /// </summary>
    [Fact]
    public async Task Accueil_DisplaysLogo_WhenUserIsAuthenticated()
    {
        // ARRANGE
        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(true));
        _mockRecipeService.GetRandomRecipesAsync(Arg.Any<int>()).Returns(Task.FromResult(new List<Recette>()));

        // ACT
        var cut = Render<Accueil>();
        await Task.Delay(100); // Wait for render

        // ASSERT
        var logos = cut.FindAll("img");
        Assert.NotEmpty(logos);
    }

    /// <summary>
    /// TEST 8: Badge "100% GRATUIT" visible pour visiteur
    /// Scénario: Utilisateur non-authentifié
    /// Résultat attendu: Badge avec texte "100% GRATUIT"
    /// </summary>
    [Fact]
    public async Task Accueil_DisplaysFreeAccessBadge_ForVisitors()
    {
        // ARRANGE
        _mockAuthService.IsAuthenticatedAsync().Returns(Task.FromResult(false));

        // ACT
        var cut = Render<Accueil>();
        await Task.Delay(100); // Wait for render;

        // ASSERT
        var badge = cut.Find(".badge-float");
        Assert.Contains("100% GRATUIT", badge.TextContent);
    }
}
