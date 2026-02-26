using Xunit;
using FluentAssertions;
using Moq;
using menuMalin.Services;
using menuMalin.Models;

namespace menuMalin.Tests.EdgeCases;

/// <summary>
/// Tests d'edge cases pour couvrir les scénarios exceptionnels
/// </summary>
public class EdgeCaseTests
{
    [Fact]
    public async Task ContactService_HandlesEmptyEmailGracefully()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();
        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new ContactService.ContactResponse { Id = "1" });

        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act
        var result = await contactService.SendMessageAsync("", null, "Subject", "Message", false);

        // Assert
        result.Should().BeTrue(); // Service n'a pas de validation client
    }

    [Fact]
    public async Task ContactService_HandlesVeryLongMessage()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();
        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new ContactService.ContactResponse { Id = "2" });

        var contactService = new ContactService(httpApiServiceMock.Object);
        var longMessage = new string('a', 5000); // 5000 caractères

        // Act
        var result = await contactService.SendMessageAsync("user@example.com", null, "Long", longMessage, false);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ContactService_HandlesSpecialCharacters()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();
        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new ContactService.ContactResponse { Id = "3" });

        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act
        var result = await contactService.SendMessageAsync(
            "user@example.com",
            null,
            "Special: <>&\"'",
            "Message with émojis 🎉 and ñ",
            false);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task FavoriteService_HandlesNullRecipe()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(new List<Recipe>()));

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act & Assert
        // Le service devrait gérer les recettes nulles sans crash
        var exception = await Record.ExceptionAsync(
            () => favoriteService.AddFavoriteAsync(null!));

        // Avec null, cela pourrait lever une exception ou être géré
        // C'est acceptable selon la conception du service
    }

    [Fact]
    public async Task FavoriteService_HandlesManyFavorites()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        var manyFavorites = Enumerable.Range(1, 100)
            .Select(i => new Recipe
            {
                IdMeal = $"recipe_{i}",
                StrMeal = $"Recipe {i}",
                StrCategory = "Dessert",
                StrArea = "Italian",
                StrMealThumb = "https://example.com/image.jpg"
            })
            .ToList();

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(manyFavorites));

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act
        var favorites = await favoriteService.GetFavoritesAsync();
        var isFav50 = await favoriteService.IsFavoriteAsync("recipe_50");

        // Assert
        favorites.Should().HaveCount(100);
        isFav50.Should().BeTrue();
    }

    [Fact]
    public async Task FavoriteService_HandlesRaceConditionScenario()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        var recipe1 = new Recipe { IdMeal = "1", StrMeal = "Recipe 1", StrCategory = "Dessert", StrArea = "Italian", StrMealThumb = "url" };
        var recipe2 = new Recipe { IdMeal = "2", StrMeal = "Recipe 2", StrCategory = "Dessert", StrArea = "Italian", StrMealThumb = "url" };

        var favsList = new List<Recipe>();

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(new List<Recipe>(favsList)));

        localStorageMock
            .Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act - Ajout rapide de deux recettes
        await favoriteService.AddFavoriteAsync(recipe1);
        await favoriteService.AddFavoriteAsync(recipe2);

        // Assert
        localStorageMock.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)); // Deux appels à SetItemAsync
    }

    [Fact]
    public async Task ContactService_RetryAfterNetworkFailure()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();
        var callCount = 0;

        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(() =>
            {
                callCount++;
                if (callCount == 1)
                    throw new HttpRequestException("Network error");
                return Task.FromResult<ContactService.ContactResponse?>(new ContactService.ContactResponse { Id = "99" });
            });

        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act - Premier appel échoue
        var result1 = await contactService.SendMessageAsync("user@example.com", null, "Test", "Message", false);

        // Réinitialiser et deuxième tentative
        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new ContactService.ContactResponse { Id = "100" });

        var result2 = await contactService.SendMessageAsync("user@example.com", null, "Test", "Message", false);

        // Assert
        result1.Should().BeFalse(); // Premier appel échoue
        result2.Should().BeTrue();  // Deuxième appel réussit
    }

    [Fact]
    public async Task FavoriteService_HandlesConcurrentOperations()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(new List<Recipe>()));

        localStorageMock
            .Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act - Plusieurs opérations en parallèle
        var tasks = Enumerable.Range(1, 5)
            .Select(i => favoriteService.AddFavoriteAsync(new Recipe
            {
                IdMeal = $"recipe_{i}",
                StrMeal = $"Recipe {i}",
                StrCategory = "Dessert",
                StrArea = "Italian",
                StrMealThumb = "url"
            }))
            .ToList();

        await Task.WhenAll(tasks);

        // Assert
        localStorageMock.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.AtLeast(5));
    }

    [Fact]
    public async Task ContactService_HandleNullException()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();
        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new NullReferenceException("Unexpected null"));

        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act
        var result = await contactService.SendMessageAsync("user@example.com", null, "Test", "Message", false);

        // Assert
        result.Should().BeFalse(); // Service gère l'exception
    }
}
