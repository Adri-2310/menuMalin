using Xunit;
using NSubstitute;
using menuMalin.Server.Services;
using menuMalin.Shared.Modeles.DTOs;
using Microsoft.Extensions.Logging;
using System.Net;

namespace menuMalin.Tests.Server;

/// <summary>
/// Tests unitaires pour ServiceMealDB
/// Teste la retry logic et la résilience aux erreurs
/// </summary>
public class ServiceMealDBTests
{
    // Fixtures - objets réutilisables pour tous les tests
    private readonly ILogger<ServiceMealDB> _mockLogger;
    private readonly HttpClient _httpClient;

    public ServiceMealDBTests()
    {
        // Créer les mocks
        _mockLogger = Substitute.For<ILogger<ServiceMealDB>>();

        // Créer un HttpClient avec un handler fictif pour contrôler les réponses
        _httpClient = new HttpClient(new FakeHttpMessageHandler());
    }

    /// <summary>
    /// TEST 1: Vérifier que GetRandomAsync retourne une recette valide
    /// Scénario: API répond avec succès
    /// Résultat attendu: Une recette est retournée
    /// </summary>
    [Fact]
    public async Task GetRandomAsync_ReturnsRecipe_WhenApiReturnsSuccess()
    {
        // ARRANGE (Préparer)
        var handler = new FakeHttpMessageHandler();
        var responseJson = @"{
            ""meals"": [{
                ""idMeal"": ""52977"",
                ""strMeal"": ""Corba"",
                ""strMealThumb"": ""https://www.themealdb.com/images/media/meals/58ede6.jpg""
            }]
        }";
        handler.SetupResponse(HttpStatusCode.OK, responseJson);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };

        var service = new ServiceMealDB(httpClient, _mockLogger);

        // ACT (Agir)
        var result = await service.GetRandomAsync();

        // ASSERT (Vérifier)
        Assert.NotNull(result);
        Assert.Equal("52977", result.IdMeal);
        Assert.Equal("Corba", result.StrMeal);
    }

    /// <summary>
    /// TEST 2: Vérifier la retry logic - API échoue puis réussit
    /// Scénario: 1ère tentative échoue (timeout), 2ème tentative réussit
    /// Résultat attendu: Après 2 tentatives, une recette est retournée
    /// </summary>
    [Fact]
    public async Task GetRandomAsync_RetriesAndSucceeds_WhenFirstAttemptFails()
    {
        // ARRANGE
        var handler = new FakeHttpMessageHandler();
        var responseJson = @"{""meals"": [{""idMeal"": ""52977"", ""strMeal"": ""Corba""}]}";

        // Configurer pour échouer 1 fois, puis réussir
        handler.SetupSequence(
            HttpStatusCode.InternalServerError,
            HttpStatusCode.OK,
            responseJson
        );

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };

        var service = new ServiceMealDB(httpClient, _mockLogger);

        // ACT
        var result = await service.GetRandomAsync();

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(2, handler.CallCount); // Doit avoir été appelé 2 fois
    }

    /// <summary>
    /// TEST 3: Vérifier l'échec après retries épuisées
    /// Scénario: API échoue 3 fois (max retries = 3)
    /// Résultat attendu: Null est retourné après 3 tentatives
    /// </summary>
    [Fact]
    public async Task GetRandomAsync_ReturnsNull_WhenAllRetriesFail()
    {
        // ARRANGE
        var handler = new FakeHttpMessageHandler();
        handler.SetupResponse(HttpStatusCode.InternalServerError, "");

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };

        var service = new ServiceMealDB(httpClient, _mockLogger);

        // ACT
        var result = await service.GetRandomAsync();

        // ASSERT
        Assert.Null(result);
        Assert.Equal(3, handler.CallCount); // Doit avoir tenté 3 fois
    }

    /// <summary>
    /// TEST 4: SearchByNameAsync avec paramètre vide
    /// Scénario: Query est vide ou null
    /// Résultat attendu: Liste vide retournée sans appel API
    /// </summary>
    [Fact]
    public async Task SearchByNameAsync_ReturnsEmptyList_WhenQueryIsEmpty()
    {
        // ARRANGE
        var handler = new FakeHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };

        var service = new ServiceMealDB(httpClient, _mockLogger);

        // ACT
        var result = await service.SearchByNameAsync("");

        // ASSERT
        Assert.Empty(result);
        Assert.Equal(0, handler.CallCount); // Aucun appel API
    }

    /// <summary>
    /// TEST 5: SearchByNameAsync avec résultats
    /// Scénario: Recherche "Pasta" retourne 2 recettes
    /// Résultat attendu: Liste avec 2 recettes
    /// </summary>
    [Fact]
    public async Task SearchByNameAsync_ReturnsRecipes_WhenSearchMatches()
    {
        // ARRANGE
        var handler = new FakeHttpMessageHandler();
        var responseJson = @"{
            ""meals"": [
                {""idMeal"": ""1"", ""strMeal"": ""Spaghetti""},
                {""idMeal"": ""2"", ""strMeal"": ""Penne""}
            ]
        }";
        handler.SetupResponse(HttpStatusCode.OK, responseJson);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };

        var service = new ServiceMealDB(httpClient, _mockLogger);

        // ACT
        var result = await service.SearchByNameAsync("Pasta");

        // ASSERT
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Spaghetti", result[0].StrMeal);
    }
}

/// <summary>
/// Handler HTTP fictif pour tester sans vraie connexion Internet
/// Permet de simuler des réponses API sans appeler TheMealDB
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private HttpStatusCode _statusCode = HttpStatusCode.OK;
    private string _responseContent = "{}";
    private Queue<(HttpStatusCode, string)> _responseQueue = new();
    public int CallCount { get; private set; }

    public void SetupResponse(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _responseContent = content;
    }

    public void SetupSequence(HttpStatusCode status1, HttpStatusCode status2, string content)
    {
        _responseQueue.Clear();
        _responseQueue.Enqueue((status1, ""));
        _responseQueue.Enqueue((status2, content));
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        CallCount++;

        var (statusCode, content) = _responseQueue.Count > 0
            ? _responseQueue.Dequeue()
            : (_statusCode, _responseContent);

        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content)
        };

        return Task.FromResult(response);
    }
}
