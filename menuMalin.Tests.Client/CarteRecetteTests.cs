using Xunit;
using Bunit;
using NSubstitute;
using menuMalin.Composants.Recette;
using menuMalin.Services;
using menuMalin.Modeles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;

namespace menuMalin.Tests.Client;

/// <summary>
/// Tests pour le composant CarteRecette avec bUnit
/// Teste le rendu et les interactions utilisateur
/// </summary>
public class CarteRecetteTests : TestContext
{
    // Mocks des services
    private readonly IServiceFavorisFrontend _mockFavoriteService;
    private readonly IServiceNotification _mockNotifService;

    public CarteRecetteTests()
    {
        // Créer les mocks
        _mockFavoriteService = Substitute.For<IServiceFavorisFrontend>();
        _mockNotifService = Substitute.For<IServiceNotification>();

        // Enregistrer dans le DI de bUnit
        Services.AddScoped<IServiceFavorisFrontend>(_ => _mockFavoriteService);
        Services.AddScoped<IServiceNotification>(_ => _mockNotifService);
        Services.AddScoped<IServiceAuthentification>(_ => Substitute.For<IServiceAuthentification>());
        Services.AddScoped<ServiceEtatAuthentification>(_ => Substitute.For<ServiceEtatAuthentification>());
        Services.AddScoped<NavigationManager>(_ => Substitute.For<NavigationManager>());
    }

    /// <summary>
    /// TEST 1: Le composant affiche correctement le titre
    /// Scénario: Passer Title="Spaghetti Carbonara"
    /// Résultat attendu: Le titre est rendu dans le composant
    /// </summary>
    [Fact]
    public void CarteRecette_DisplaysTitle_WhenTitleParameterIsSet()
    {
        // ARRANGE - Configurer les mocks
        _mockFavoriteService.IsFavoriteAsync("123").Returns(Task.FromResult(false));

        // ACT - Renderer avec les paramètres
        var cut = Render<CarteRecette>(parameters =>
        {
            parameters.Add(c => c.RecipeId, "123");
            parameters.Add(c => c.MealDBId, "52977");
            parameters.Add(c => c.Title, "Spaghetti Carbonara");
            parameters.Add(c => c.ImageUrl, "https://example.com/pasta.jpg");
            parameters.Add(c => c.Category, "Pasta");
        });

        // ASSERT - Vérifier le contenu
        var titleElement = cut.Find("h5.card-title");
        Assert.Contains("Spaghetti Carbonara", titleElement.TextContent);
    }

    /// <summary>
    /// TEST 2: Le lien "Voir plus" a le bon href
    /// Scénario: MealDBId = "52977"
    /// Résultat attendu: href="/recette/52977"
    /// </summary>
    [Fact]
    public void CarteRecette_VoirPlusButtonLinksToRecipeDetail_WhenMealDBIdIsSet()
    {
        // ARRANGE
        _mockFavoriteService.IsFavoriteAsync("123").Returns(Task.FromResult(false));

        // ACT
        var cut = Render<CarteRecette>(parameters =>
        {
            parameters.Add(c => c.RecipeId, "123");
            parameters.Add(c => c.MealDBId, "52977");
            parameters.Add(c => c.Title, "Meal");
            parameters.Add(c => c.ImageUrl, "https://example.com/img.jpg");
            parameters.Add(c => c.Category, "Meat");
        });

        // ASSERT
        var link = cut.Find("a.btn-success");
        var href = link.GetAttribute("href");
        Assert.Equal("/recette/52977", href);
    }

    /// <summary>
    /// TEST 3: Image a le bon attribut src
    /// Scénario: ImageUrl="https://example.com/recipe.jpg"
    /// Résultat attendu: <img src="https://example.com/recipe.jpg" />
    /// </summary>
    [Fact]
    public void CarteRecette_DisplaysImage_WithCorrectSrc()
    {
        // ARRANGE
        _mockFavoriteService.IsFavoriteAsync("123").Returns(Task.FromResult(false));

        // ACT
        var cut = Render<CarteRecette>(parameters =>
        {
            parameters.Add(c => c.RecipeId, "123");
            parameters.Add(c => c.MealDBId, "52977");
            parameters.Add(c => c.Title, "Test");
            parameters.Add(c => c.ImageUrl, "https://example.com/recipe.jpg");
            parameters.Add(c => c.Category, "Seafood");
        });

        // ASSERT
        var img = cut.Find("img.card-img-top");
        var src = img.GetAttribute("src");
        Assert.Equal("https://example.com/recipe.jpg", src);
    }

    /// <summary>
    /// TEST 4: Badge affiche la catégorie
    /// Scénario: Category="Pasta"
    /// Résultat attendu: Texte "Pasta" dans le badge
    /// </summary>
    [Fact]
    public void CarteRecette_DisplaysCategory_InBadge()
    {
        // ARRANGE
        _mockFavoriteService.IsFavoriteAsync("123").Returns(Task.FromResult(false));

        // ACT
        var cut = Render<CarteRecette>(parameters =>
        {
            parameters.Add(c => c.RecipeId, "123");
            parameters.Add(c => c.MealDBId, "52977");
            parameters.Add(c => c.Title, "Lasagna");
            parameters.Add(c => c.ImageUrl, "https://example.com/img.jpg");
            parameters.Add(c => c.Category, "Pasta");
        });

        // ASSERT
        var badge = cut.Find("span.badge");
        Assert.Contains("Pasta", badge.TextContent);
    }

    /// <summary>
    /// TEST 5: Le bouton favori existe et est cliquable
    /// Scénario: Composant chargé
    /// Résultat attendu: Bouton <button class="btn-favorite"> est présent
    /// </summary>
    [Fact]
    public void CarteRecette_HasFavoriteButton_WhenRendered()
    {
        // ARRANGE
        _mockFavoriteService.IsFavoriteAsync("123").Returns(Task.FromResult(false));

        // ACT
        var cut = Render<CarteRecette>(parameters =>
        {
            parameters.Add(c => c.RecipeId, "123");
            parameters.Add(c => c.MealDBId, "52977");
            parameters.Add(c => c.Title, "Recipe");
            parameters.Add(c => c.ImageUrl, "https://example.com/img.jpg");
            parameters.Add(c => c.Category, "Vegetarian");
        });

        // ASSERT
        var button = cut.Find("button.btn-favorite");
        Assert.NotNull(button);

        // Vérifier qu'il contient une icône coeur
        var heartIcon = button.QuerySelector("i.bi-heart");
        Assert.NotNull(heartIcon);
    }

    /// <summary>
    /// TEST 6: Le composant utilise onerror pour l'image par défaut
    /// Scénario: L'image a l'attribut onerror
    /// Résultat attendu: onerror="this.src='Images/logo.png'"
    /// </summary>
    [Fact]
    public void CarteRecette_ImageHasOnerrorFallback()
    {
        // ARRANGE
        _mockFavoriteService.IsFavoriteAsync("123").Returns(Task.FromResult(false));

        // ACT
        var cut = Render<CarteRecette>(parameters =>
        {
            parameters.Add(c => c.RecipeId, "123");
            parameters.Add(c => c.MealDBId, "52977");
            parameters.Add(c => c.Title, "Food");
            parameters.Add(c => c.ImageUrl, "https://example.com/img.jpg");
            parameters.Add(c => c.Category, "Meat");
        });

        // ASSERT
        var img = cut.Find("img");
        var onerror = img.GetAttribute("onerror");
        Assert.Contains("this.src='Images/logo.png'", onerror);
    }
}
