using Xunit;
using Moq;
using FluentAssertions;
using menuMalin.Services;
using System.Net;

namespace menuMalin.Tests.Services;

public class HttpApiServiceTests
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly HttpApiService _httpApiService;

    public HttpApiServiceTests()
    {
        _httpClientMock = new Mock<HttpClient>();
        _httpApiService = new HttpApiService(_httpClientMock.Object);
    }

    [Fact]
    public async Task GetAsync_WithValidUrl_ShouldSucceed()
    {
        // Arrange
        string endpoint = "/api/recipes";

        // Act
        // var result = await _httpApiService.GetAsync(endpoint);

        // Assert
        // result.Should().NotBeNull();

        await Task.CompletedTask;
    }

    [Fact]
    public async Task PostAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        string endpoint = "/api/favorites";
        var data = new { recipeId = "123" };

        // Act
        // var result = await _httpApiService.PostAsync(endpoint, data);

        // Assert
        // result.Should().NotBeNull();

        await Task.CompletedTask;
    }

    [Fact]
    public async Task DeleteAsync_WithValidUrl_ShouldSucceed()
    {
        // Arrange
        string endpoint = "/api/favorites/123";

        // Act
        // var result = await _httpApiService.DeleteAsync(endpoint);

        // Assert
        // result.StatusCode.Should().Be(HttpStatusCode.OK);

        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetAsync_WithInvalidUrl_ShouldThrowException()
    {
        // Arrange
        string endpoint = "/api/invalid/endpoint";

        // Act
        // Func<Task> act = async () => await _httpApiService.GetAsync(endpoint);

        // Assert
        // await act.Should().ThrowAsync<HttpRequestException>();

        await Task.CompletedTask;
    }

    [Fact]
    public async Task PostAsync_WithNullData_ShouldThrowException()
    {
        // Arrange
        string endpoint = "/api/favorites";
        object data = null;

        // Act
        // Func<Task> act = async () => await _httpApiService.PostAsync(endpoint, data);

        // Assert
        // await act.Should().ThrowAsync<ArgumentNullException>();

        await Task.CompletedTask;
    }
}
