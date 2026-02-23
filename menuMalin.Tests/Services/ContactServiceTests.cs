using Xunit;
using Moq;
using FluentAssertions;
using menuMalin.Services;

namespace menuMalin.Tests.Services;

public class ContactServiceTests
{
    private readonly Mock<IHttpApiService> _httpApiServiceMock;
    private readonly ContactService _contactService;

    public ContactServiceTests()
    {
        _httpApiServiceMock = new Mock<IHttpApiService>();
        _contactService = new ContactService(_httpApiServiceMock.Object);
    }

    [Fact]
    public async Task SendMessageAsync_WithValidEmail_ShouldReturnTrue()
    {
        // Arrange
        string email = "test@example.com";
        string subject = "Test Subject";
        string message = "Test message content";

        _httpApiServiceMock
            .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new object());

        // Act
        var result = await _contactService.SendMessageAsync(email, subject, message);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SendMessageAsync_WithEmptyEmail_ShouldThrowException()
    {
        // Arrange
        string email = "";
        string subject = "Test Subject";
        string message = "Test message content";

        _httpApiServiceMock
            .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new ArgumentException("Email cannot be empty"));

        // Act
        var result = await _contactService.SendMessageAsync(email, subject, message);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendMessageAsync_WithEmptyMessage_ShouldThrowException()
    {
        // Arrange
        string email = "test@example.com";
        string subject = "Test Subject";
        string message = "";

        _httpApiServiceMock
            .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new ArgumentException("Message cannot be empty"));

        // Act
        var result = await _contactService.SendMessageAsync(email, subject, message);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendMessageAsync_WithValidData_ShouldNotThrow()
    {
        // Arrange
        string email = "test@example.com";
        string subject = "Test Subject";
        string message = "Test message content";

        // Act
        Func<Task> act = async () => await _contactService.SendMessageAsync(email, subject, message);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendMessageAsync_WithMultipleValidCalls_ShouldSucceed()
    {
        // Arrange
        var messages = new List<(string, string, string)>
        {
            ("test1@example.com", "Subject 1", "Message 1"),
            ("test2@example.com", "Subject 2", "Message 2"),
            ("test3@example.com", "Subject 3", "Message 3")
        };

        _httpApiServiceMock
            .Setup(x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(new object());

        // Act & Assert
        foreach (var (email, subject, message) in messages)
        {
            var result = await _contactService.SendMessageAsync(email, subject, message);
            result.Should().BeTrue();
        }

        _httpApiServiceMock.Verify(
            x => x.PostAsync<object>(It.IsAny<string>(), It.IsAny<object>()),
            Times.Exactly(3));
    }
}
