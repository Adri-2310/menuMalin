using Xunit;
using FluentAssertions;
using Moq;
using menuMalin.Services;
using menuMalin.Models;

namespace menuMalin.Tests.Integration;

/// <summary>
/// Tests d'intégration pour vérifier que les services frontend communiquent correctement
/// avec les dépendances backend (mockées pour les tests unitaires)
/// </summary>
public class ServiceIntegrationTests
{
    private Recipe CreateTestRecipe(string id)
    {
        return new Recipe
        {
            IdMeal = id,
            StrMeal = $"Test Recipe {id}",
            StrCategory = "Dessert",
            StrArea = "Italian",
            StrMealThumb = "https://example.com/image.jpg"
        };
    }

    #region FavoriteService Integration Tests

    [Fact]
    public async Task FavoriteService_CanAddAndRetrieveFavorites()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        var recipe = CreateTestRecipe("1");
        var emptyFavorites = new List<Recipe>();

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(emptyFavorites));

        localStorageMock
            .Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act
        await favoriteService.AddFavoriteAsync(recipe);
        var favorites = await favoriteService.GetFavoritesAsync();

        // Assert
        favorites.Should().NotBeNull();
        localStorageMock.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FavoriteService_CanCheckIfRecipeIsFavorited()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        var recipe = CreateTestRecipe("123");

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(new List<Recipe> { recipe }));

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act
        var isFavorite = await favoriteService.IsFavoriteAsync("123");

        // Assert
        isFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task FavoriteService_CanRemoveFavorite()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        var recipe = CreateTestRecipe("456");

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(new List<Recipe> { recipe }));

        localStorageMock
            .Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act
        await favoriteService.RemoveFavoriteAsync("456");

        // Assert
        localStorageMock.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FavoriteService_HandleEmptyFavorites()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>((List<Recipe>)null!));

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act
        var favorites = await favoriteService.GetFavoritesAsync();

        // Assert
        favorites.Should().NotBeNull();
        favorites.Should().BeEmpty();
    }

    #endregion

    #region ContactService Integration Tests

    [Fact]
    public async Task ContactService_CanSendMessage()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();

        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new ContactService.ContactResponse { Id = "1" });

        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act
        var result = await contactService.SendMessageAsync(
            "user@example.com",
            null,
            "Support",
            "I need assistance",
            false);

        // Assert
        result.Should().BeTrue();
        httpApiServiceMock.Verify(
            x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task ContactService_HandlesFailureGracefully()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();

        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act
        var result = await contactService.SendMessageAsync(
            "user@example.com",
            null,
            "Error",
            "Test",
            false);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ContactService_SendsToContactEndpoint()
    {
        // Arrange
        var httpApiServiceMock = new Mock<IHttpApiService>();

        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>("contact", It.IsAny<object>()))
            .ReturnsAsync(new ContactService.ContactResponse { Id = "2" });

        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act
        await contactService.SendMessageAsync("test@example.com", null, "Feedback", "Message", false);

        // Assert
        httpApiServiceMock.Verify(
            x => x.PostAsync<ContactService.ContactResponse>("contact", It.IsAny<object>()),
            Times.Once);
    }

    #endregion

    #region Cross-Service Integration Tests

    [Fact]
    public async Task Services_CanWorkTogether_AddingAndCheckingFavorite()
    {
        // Arrange - Setup both services
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        var httpApiServiceMock = new Mock<IHttpApiService>();

        var recipe = CreateTestRecipe("789");
        var emptyFavorites = new List<Recipe>();

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(emptyFavorites));

        localStorageMock
            .Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        httpApiServiceMock
            .Setup(x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new ContactService.ContactResponse { Id = "3" });

        var favoriteService = new FavoriteService(localStorageMock.Object);
        var contactService = new ContactService(httpApiServiceMock.Object);

        // Act
        await favoriteService.AddFavoriteAsync(recipe);
        var isFavorite = await favoriteService.IsFavoriteAsync("789");
        var messageSent = await contactService.SendMessageAsync("user@example.com", null, "Like", "Love this recipe!", false);

        // Assert
        isFavorite.Should().BeTrue();
        messageSent.Should().BeTrue();
        localStorageMock.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
        httpApiServiceMock.Verify(
            x => x.PostAsync<ContactService.ContactResponse>(It.IsAny<string>(), It.IsAny<object>()),
            Times.Once);
    }

    [Fact]
    public async Task Services_HandleMultipleOperations()
    {
        // Arrange
        var localStorageMock = new Mock<Blazored.LocalStorage.ILocalStorageService>();
        var favorites = new List<Recipe>
        {
            CreateTestRecipe("1"),
            CreateTestRecipe("2"),
            CreateTestRecipe("3")
        };

        localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(favorites));

        var favoriteService = new FavoriteService(localStorageMock.Object);

        // Act
        var fav1 = await favoriteService.IsFavoriteAsync("1");
        var fav2 = await favoriteService.IsFavoriteAsync("2");
        var fav3 = await favoriteService.IsFavoriteAsync("3");
        var allFavorites = await favoriteService.GetFavoritesAsync();

        // Assert
        fav1.Should().BeTrue();
        fav2.Should().BeTrue();
        fav3.Should().BeTrue();
        allFavorites.Should().HaveCount(3);
    }

    #endregion
}
