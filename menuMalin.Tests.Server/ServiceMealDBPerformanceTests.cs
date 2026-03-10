using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NSubstitute;
using menuMalin.Server.Services;
using Microsoft.Extensions.Logging;
using System.Net;

namespace menuMalin.Tests.Server;

/// <summary>
/// Tests de performance pour ServiceMealDB
/// Mesure: temps de réponse, allocations mémoire, retry overhead
///
/// Exécuter avec: dotnet run -c Release -- --filter ServiceMealDBPerformanceTests
/// </summary>
[MemoryDiagnoser]  // Mesure allocations mémoire
[SimpleJob(warmupCount: 3, targetCount: 5)]  // 3 warmups, 5 iterations
public class ServiceMealDBPerformanceTests
{
    private ServiceMealDB _service = null!;
    private ILogger<ServiceMealDB> _mockLogger = null!;
    private FakeHttpMessageHandler _handler = null!;
    private HttpClient _httpClient = null!;

    [GlobalSetup]
    public void Setup()
    {
        _mockLogger = Substitute.For<ILogger<ServiceMealDB>>();
        _handler = new FakeHttpMessageHandler();
        _httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };
    }

    /// <summary>
    /// PERF 1: GetRandomAsync - Succès API (cas normal)
    /// Mesure le temps et mémoire pour un appel API réussi
    /// Baseline: doit être < 5ms
    /// </summary>
    [Benchmark(Description = "GetRandomAsync - API Success")]
    public async Task GetRandomAsync_Success()
    {
        // Setup mock response
        var responseJson = @"{""meals"": [{""idMeal"": ""52977"", ""strMeal"": ""Corba""}]}";
        _handler.SetupResponse(HttpStatusCode.OK, responseJson);
        _service = new ServiceMealDB(_httpClient, _mockLogger);

        // Execute
        var result = await _service.GetRandomAsync();

        // Assert result is not null
        if (result == null)
            throw new Exception("API call failed unexpectedly");
    }

    /// <summary>
    /// PERF 2: GetRandomAsync - Avec retry (1 timeout, puis succès)
    /// Mesure l'overhead du retry mechanism
    /// Baseline: devrait être 2-3x plus lent que succès direct
    /// </summary>
    [Benchmark(Description = "GetRandomAsync - With Retry (1 fail)")]
    public async Task GetRandomAsync_WithRetry()
    {
        // Setup: fail 1 time, then succeed
        var responseJson = @"{""meals"": [{""idMeal"": ""52977"", ""strMeal"": ""Corba""}]}";
        _handler.SetupSequence(HttpStatusCode.ServiceUnavailable, HttpStatusCode.OK, responseJson);
        _service = new ServiceMealDB(_httpClient, _mockLogger);

        // Execute
        var result = await _service.GetRandomAsync();

        if (result == null)
            throw new Exception("Retry logic failed");
    }

    /// <summary>
    /// PERF 3: SearchByNameAsync - Résultats nombreux
    /// Mesure performance avec 50 résultats
    /// Baseline: doit rester < 10ms même avec beaucoup de données
    /// </summary>
    [Benchmark(Description = "SearchByNameAsync - 50 Results")]
    public async Task SearchByNameAsync_ManyResults()
    {
        // Build response with 50 meals
        var meals = string.Join(",", Enumerable.Range(1, 50)
            .Select(i => $@"{{""idMeal"": ""{i}"", ""strMeal"": ""Meal {i}""}}"));
        var responseJson = $@"{{""meals"": [{meals}]}}";

        _handler.SetupResponse(HttpStatusCode.OK, responseJson);
        _service = new ServiceMealDB(_httpClient, _mockLogger);

        // Execute
        var result = await _service.SearchByNameAsync("test");

        if (result.Count != 50)
            throw new Exception($"Expected 50 results, got {result.Count}");
    }

    /// <summary>
    /// PERF 4: GetRandomAsync - Allocation comparison
    /// Mesure les allocations mémoire (objet vs null)
    /// Baseline: succès < 5KB, retry < 10KB
    /// </summary>
    [Benchmark(Description = "GetRandomAsync - Memory Allocations")]
    public async Task GetRandomAsync_MemoryTest()
    {
        var responseJson = @"{""meals"": [{""idMeal"": ""1"", ""strMeal"": ""Test""}]}";
        _handler.SetupResponse(HttpStatusCode.OK, responseJson);
        _service = new ServiceMealDB(_httpClient, _mockLogger);

        // Execute 10 times to measure total allocation
        for (int i = 0; i < 10; i++)
        {
            var result = await _service.GetRandomAsync();
            if (result == null)
                throw new Exception("Allocation test failed");
        }
    }

    /// <summary>
    /// PERF 5: Désérialisation JSON - Impact sur temps total
    /// Mesure le coût de la désérialisation
    /// Baseline: doit être < 2ms pour un objet simple
    /// </summary>
    [Benchmark(Description = "JSON Deserialization")]
    public async Task JsonDeserializationPerformance()
    {
        var largeJson = @"{""meals"": [" +
            string.Join(",", Enumerable.Range(1, 10)
                .Select(i => $@"{{""idMeal"": ""{i}"", ""strMeal"": ""Meal {i}"", ""strCategory"": ""Pasta"", ""strArea"": ""Italian""}}"))
            + "]}";

        _handler.SetupResponse(HttpStatusCode.OK, largeJson);
        _service = new ServiceMealDB(_httpClient, _mockLogger);

        var result = await _service.SearchByNameAsync("large");

        if (result.Count == 0)
            throw new Exception("Deserialization failed");
    }
}

/// <summary>
/// Tests de performance pour le Repository
/// Mesure la performance des opérations BD en mémoire
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, targetCount: 5)]
public class RepositoryPerformanceTests
{
    private ApplicationDbContext _context = null!;
    private DepotRecetteUtilisateur _repository = null!;
    private const string TestUserId = "perf-user-123";

    [GlobalSetup]
    public void Setup()
    {
        var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureCreated();

        // Add test user
        var user = new menuMalin.Server.Modeles.Entites.Utilisateur
        {
            UserId = TestUserId,
            Email = "perf@test.com",
            Name = "Perf Test",
            DateCreation = DateTime.UtcNow
        };
        _context.Utilisateurs.Add(user);

        // Add 100 test recipes
        for (int i = 0; i < 100; i++)
        {
            var recipe = new menuMalin.Server.Modeles.Entites.RecetteUtilisateur
            {
                UserRecipeId = $"recipe-{i}",
                UserId = TestUserId,
                Title = $"Recipe {i}",
                Category = "Pasta",
                Area = "Italian",
                Instructions = $"Instructions for recipe {i}",
                IngredientsJson = @"[""Pasta"", ""Sauce""]",
                IsPublic = i % 2 == 0,  // Moitié publique, moitié privée
                DateCreation = DateTime.UtcNow,
                DateMaj = DateTime.UtcNow
            };
            _context.RecettesUtilisateur.Add(recipe);
        }
        _context.SaveChanges();

        _repository = new DepotRecetteUtilisateur(_context);
    }

    /// <summary>
    /// PERF 6: GetByUserIdAsync - Récupération 100 recettes
    /// Baseline: doit rester < 5ms
    /// </summary>
    [Benchmark(Description = "GetByUserIdAsync - 100 Records")]
    public async Task GetByUserIdAsync_LargeSet()
    {
        var recipes = await _repository.GetByUserIdAsync(TestUserId);

        if (recipes.Count() != 100)
            throw new Exception($"Expected 100 recipes, got {recipes.Count()}");
    }

    /// <summary>
    /// PERF 7: GetPublicAsync - Filtrer les publiques
    /// Mesure la performance du filtrage LINQ
    /// Baseline: doit rester < 5ms
    /// </summary>
    [Benchmark(Description = "GetPublicAsync - Filter Public")]
    public async Task GetPublicAsync_Filter()
    {
        var publicRecipes = await _repository.GetPublicAsync();

        if (publicRecipes.Count() != 50)
            throw new Exception($"Expected 50 public recipes, got {publicRecipes.Count()}");
    }

    /// <summary>
    /// PERF 8: AddAsync - Insertion simple
    /// Baseline: doit rester < 2ms
    /// </summary>
    [Benchmark(Description = "AddAsync - Single Insert")]
    public async Task AddAsync_SingleRecipe()
    {
        var newRecipe = new menuMalin.Server.Modeles.Entites.RecetteUtilisateur
        {
            UserRecipeId = Guid.NewGuid().ToString(),
            UserId = TestUserId,
            Title = "New Recipe",
            Category = "Pasta",
            Area = "Italian",
            Instructions = "New instructions",
            IngredientsJson = @"[""Pasta""]",
            IsPublic = true,
            DateCreation = DateTime.UtcNow,
            DateMaj = DateTime.UtcNow
        };

        var result = await _repository.AddAsync(newRecipe);

        if (result == null)
            throw new Exception("Insert failed");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }
}

/// <summary>
/// Mock HTTP Handler pour les tests
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
