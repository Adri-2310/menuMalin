using Xunit;
using Moq;
using FluentAssertions;
using menuMalin.Services;
using menuMalin.Models;
using Blazored.LocalStorage;

namespace menuMalin.Tests.Services;

public class FavoriteServiceTests
{
    private readonly Mock<ILocalStorageService> _localStorageMock;
    private readonly FavoriteService _favoriteService;

    public FavoriteServiceTests()
    {
        _localStorageMock = new Mock<ILocalStorageService>();
        _favoriteService = new FavoriteService(_localStorageMock.Object);
    }

    [Fact]
    public async Task AddFavoriteAsync_WithValidRecipe_ShouldSucceed()
    {
        // Arrange
        var recipe = new Recipe
        {
            IdMeal = "12345",
            StrMeal = "Test Recipe",
            StrCategory = "Test",
            StrArea = "Test"
        };

        _localStorageMock
            .Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        await _favoriteService.AddFavoriteAsync(recipe);

        // Assert
        _localStorageMock.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveFavoriteAsync_WithValidId_ShouldSucceed()
    {
        // Arrange
        string recipeId = "12345";
        var initialFavorites = new List<Recipe>
        {
            new Recipe { IdMeal = "12345", StrMeal = "Recipe to Remove" }
        };

        _localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(initialFavorites));

        _localStorageMock
            .Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        await _favoriteService.RemoveFavoriteAsync(recipeId);

        // Assert
        _localStorageMock.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task IsFavoriteAsync_WithStoredFavorite_ShouldReturnTrue()
    {
        // Arrange
        string recipeId = "12345";
        var storedFavorites = new List<Recipe>
        {
            new Recipe { IdMeal = "12345", StrMeal = "Test Recipe" }
        };

        _localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(storedFavorites));

        // Act
        var result = await _favoriteService.IsFavoriteAsync(recipeId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFavoriteAsync_WithoutStoredFavorite_ShouldReturnFalse()
    {
        // Arrange
        string recipeId = "12345";

        _localStorageMock
            .Setup(x => x.GetItemAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<string>((string)null));

        // Act
        var result = await _favoriteService.IsFavoriteAsync(recipeId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetFavoritesAsync_ShouldReturnList()
    {
        // Arrange
        var expectedFavorites = new List<Recipe>
        {
            new Recipe { IdMeal = "1", StrMeal = "Recipe 1" },
            new Recipe { IdMeal = "2", StrMeal = "Recipe 2" }
        };

        _localStorageMock
            .Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<List<Recipe>>(expectedFavorites));

        // Act
        var result = await _favoriteService.GetFavoritesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<Recipe>>();
    }
}
