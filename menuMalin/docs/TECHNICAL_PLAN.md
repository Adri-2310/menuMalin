# Plan Technique MenuMalin - Blazor WebAssembly

## Vue d'ensemble
Application PWA Blazor WebAssembly .NET 9 avec intégration TheMealDB API, authentification OIDC Auth0, et persistance locale.

---

## 1. Structure des Composants Blazor

### 1.1 Architecture des Dossiers
```
menuMalin/
├── Components/
│   ├── Recipe/
│   │   ├── RecipeCard.razor                 # Carte de recette réutilisable
│   │   ├── RecipeDetails.razor              # Vue détaillée d'une recette
│   │   ├── RecipeEditor.razor               # Formulaire d'édition
│   │   ├── RecipeList.razor                 # Liste avec pagination
│   │   └── YouTubePlayer.razor              # Lecteur vidéo intégré
│   ├── Search/
│   │   ├── SearchBar.razor                  # Barre de recherche
│   │   ├── SearchFilters.razor              # Filtres avancés
│   │   └── SearchResults.razor              # Résultats paginés
│   ├── Shopping/
│   │   ├── IngredientCheckbox.razor         # Case à cocher pour ingrédient
│   │   ├── ShoppingList.razor               # Liste de courses générée
│   │   └── ShoppingListExport.razor         # Export PDF/Print
│   ├── Common/
│   │   ├── LoadingSpinner.razor             # Indicateur de chargement
│   │   ├── ErrorBoundary.razor              # Gestion d'erreurs
│   │   ├── Toast.razor                      # Notifications
│   │   └── Pagination.razor                 # Composant de pagination
│   └── Favorites/
│       ├── FavoriteButton.razor             # Bouton favori avec icône
│       └── FavoritesList.razor              # Liste des favoris utilisateur
├── Pages/
│   ├── Index.razor                          # Page d'accueil
│   ├── Search.razor                         # Page de recherche
│   ├── RecipeDetail.razor                   # Page détail recette (@page "/recipe/{id}")
│   ├── Favorites.razor                      # Page favoris (AuthorizeView)
│   ├── ShoppingList.razor                   # Page liste de courses
│   └── Profile.razor                        # Profil utilisateur
├── Layout/
│   ├── MainLayout.razor                     # Layout principal (existant)
│   ├── NavMenu.razor                        # Navigation (existant)
│   └── LoginDisplay.razor                   # Affichage login (existant)
├── Models/
│   ├── Recipe.cs                            # Modèle de recette
│   ├── Ingredient.cs                        # Modèle d'ingrédient
│   ├── UserRecipe.cs                        # Recette utilisateur modifiée
│   ├── ShoppingListItem.cs                  # Élément de liste de courses
│   └── ApiModels/
│       ├── MealDbResponse.cs                # Réponse API TheMealDB
│       └── MealDbMeal.cs                    # Meal de l'API
├── Services/
│   ├── Api/
│   │   ├── IMealDbApiService.cs             # Interface API
│   │   └── MealDbApiService.cs              # Service API TheMealDB
│   ├── Storage/
│   │   ├── ILocalStorageService.cs          # Interface storage
│   │   ├── LocalStorageService.cs           # Implémentation LocalStorage
│   │   └── ISqliteService.cs                # Interface SQLite (future)
│   ├── State/
│   │   ├── IAppStateService.cs              # Interface state management
│   │   └── AppStateService.cs               # Service d'état global
│   └── Business/
│       ├── IRecipeService.cs                # Interface métier recettes
│       ├── RecipeService.cs                 # Logique métier recettes
│       ├── IShoppingListService.cs          # Interface liste de courses
│       └── ShoppingListService.cs           # Logique liste de courses
├── Utilities/
│   ├── Constants.cs                         # Constantes globales
│   ├── RecipeMapper.cs                      # Mapping API -> Modèles
│   └── ValidationHelper.cs                  # Helpers de validation
└── wwwroot/
    ├── css/
    │   ├── app.css                          # Styles globaux
    │   └── components/                      # Styles par composant
    ├── js/
    │   └── interop.js                       # JS Interop helpers
    └── manifest.webmanifest                 # PWA manifest
```

### 1.2 Composants Principaux

#### RecipeCard.razor
```razor
@* Composant réutilisable pour afficher une recette en format carte *@
<div class="recipe-card" @onclick="OnCardClick">
    <img src="@Recipe.StrMealThumb" alt="@Recipe.StrMeal" />
    <div class="recipe-card-body">
        <h3>@Recipe.StrMeal</h3>
        <p class="category">@Recipe.StrCategory</p>
        <p class="area">@Recipe.StrArea</p>
        <FavoriteButton RecipeId="@Recipe.IdMeal" IsFavorite="@IsFavorite"
                        OnToggle="HandleFavoriteToggle" />
    </div>
</div>

@code {
    [Parameter] public Recipe Recipe { get; set; } = null!;
    [Parameter] public bool IsFavorite { get; set; }
    [Parameter] public EventCallback<string> OnCardClick { get; set; }
    [Parameter] public EventCallback<string> OnToggleFavorite { get; set; }
}
```

#### RecipeDetails.razor
```razor
@* Vue détaillée avec édition si favori *@
<AuthorizeView>
    <Authorized>
        @if (IsEditing)
        {
            <RecipeEditor Recipe="@EditableRecipe" OnSave="SaveChanges" OnCancel="CancelEdit" />
        }
        else
        {
            <button @onclick="StartEdit">Modifier</button>
        }
    </Authorized>
</AuthorizeView>

<div class="recipe-details">
    <h1>@Recipe.StrMeal</h1>
    @if (!string.IsNullOrEmpty(Recipe.StrYoutube))
    {
        <YouTubePlayer VideoUrl="@Recipe.StrYoutube" />
    }
    <h2>Ingrédients</h2>
    <ul>
        @foreach (var ingredient in Recipe.Ingredients)
        {
            <IngredientCheckbox Ingredient="@ingredient" OnCheckChanged="UpdateShoppingList" />
        }
    </ul>
    <h2>Instructions</h2>
    <p>@Recipe.StrInstructions</p>
</div>
```

#### SearchBar.razor
```razor
@* Barre de recherche avec debounce *@
<div class="search-bar">
    <input type="text"
           @bind="searchTerm"
           @bind:event="oninput"
           @bind:after="OnSearchChanged"
           placeholder="Rechercher une recette..." />
    <button @onclick="PerformSearch">
        <span class="icon-search"></span>
    </button>
</div>

@code {
    private string searchTerm = string.Empty;
    private System.Timers.Timer? debounceTimer;

    [Parameter] public EventCallback<string> OnSearch { get; set; }
    [Parameter] public int DebounceMs { get; set; } = 300;

    private void OnSearchChanged()
    {
        debounceTimer?.Stop();
        debounceTimer = new System.Timers.Timer(DebounceMs);
        debounceTimer.Elapsed += async (s, e) => await PerformSearch();
        debounceTimer.AutoReset = false;
        debounceTimer.Start();
    }
}
```

---

## 2. Services et Injection de Dépendances

### 2.1 Configuration dans Program.cs

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using menuMalin;
using menuMalin.Services.Api;
using menuMalin.Services.Storage;
using menuMalin.Services.State;
using menuMalin.Services.Business;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration HttpClient pour TheMealDB API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
});

// HttpClient pour l'application elle-même
builder.Services.AddScoped<HttpClient>(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Authentification OIDC Auth0
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience",
        builder.Configuration["Auth0:Audience"] ?? "");
});

// Services de stockage
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

// Services API
builder.Services.AddScoped<IMealDbApiService, MealDbApiService>();

// Services métier
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();

// State Management
builder.Services.AddSingleton<IAppStateService, AppStateService>();

// Logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

await builder.Build().RunAsync();
```

### 2.2 Services API

#### IMealDbApiService.cs
```csharp
namespace menuMalin.Services.Api;

public interface IMealDbApiService
{
    Task<IEnumerable<Recipe>> SearchByNameAsync(string name);
    Task<IEnumerable<Recipe>> SearchByIngredientAsync(string ingredient);
    Task<Recipe?> GetRecipeByIdAsync(string id);
    Task<IEnumerable<Recipe>> GetRandomRecipesAsync(int count = 10);
    Task<IEnumerable<string>> GetCategoriesAsync();
    Task<IEnumerable<string>> GetAreasAsync();
}
```

#### MealDbApiService.cs
```csharp
namespace menuMalin.Services.Api;

public class MealDbApiService : IMealDbApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MealDbApiService> _logger;

    public MealDbApiService(HttpClient httpClient, ILogger<MealDbApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Recipe>> SearchByNameAsync(string name)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<MealDbResponse>(
                $"search.php?s={Uri.EscapeDataString(name)}");

            return response?.Meals?.Select(RecipeMapper.MapFromApiMeal)
                   ?? Enumerable.Empty<Recipe>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching recipes by name: {Name}", name);
            throw;
        }
    }

    public async Task<Recipe?> GetRecipeByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<MealDbResponse>(
                $"lookup.php?i={id}");

            return response?.Meals?.FirstOrDefault() is var meal && meal != null
                ? RecipeMapper.MapFromApiMeal(meal)
                : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recipe by ID: {Id}", id);
            return null;
        }
    }

    // Autres méthodes...
}
```

### 2.3 Services de Stockage

#### ILocalStorageService.cs
```csharp
namespace menuMalin.Services.Storage;

public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
    Task<IEnumerable<T>> GetAllItemsAsync<T>(string keyPrefix);
}
```

#### LocalStorageService.cs
```csharp
using Blazored.LocalStorage;

namespace menuMalin.Services.Storage;

public class LocalStorageService : ILocalStorageService
{
    private readonly ILocalStorageService _blazoredLocalStorage;
    private readonly ILogger<LocalStorageService> _logger;

    public LocalStorageService(
        Blazored.LocalStorage.ILocalStorageService blazoredLocalStorage,
        ILogger<LocalStorageService> logger)
    {
        _blazoredLocalStorage = blazoredLocalStorage;
        _logger = logger;
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            return await _blazoredLocalStorage.GetItemAsync<T>(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item from local storage: {Key}", key);
            return default;
        }
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            await _blazoredLocalStorage.SetItemAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting item in local storage: {Key}", key);
            throw;
        }
    }

    // Autres méthodes...
}
```

### 2.4 Services Métier

#### IRecipeService.cs
```csharp
namespace menuMalin.Services.Business;

public interface IRecipeService
{
    // Recherche API
    Task<IEnumerable<Recipe>> SearchRecipesAsync(string searchTerm);
    Task<Recipe?> GetRecipeDetailsAsync(string recipeId);

    // Gestion des favoris
    Task<IEnumerable<UserRecipe>> GetUserFavoritesAsync(string userId);
    Task<UserRecipe?> GetUserRecipeAsync(string userId, string recipeId);
    Task AddToFavoritesAsync(string userId, Recipe recipe);
    Task RemoveFromFavoritesAsync(string userId, string recipeId);
    Task<bool> IsFavoriteAsync(string userId, string recipeId);

    // Édition de recettes
    Task UpdateUserRecipeAsync(string userId, UserRecipe recipe);
}
```

#### RecipeService.cs
```csharp
namespace menuMalin.Services.Business;

public class RecipeService : IRecipeService
{
    private readonly IMealDbApiService _apiService;
    private readonly ILocalStorageService _storageService;
    private readonly ILogger<RecipeService> _logger;

    private const string FAVORITES_KEY_PREFIX = "favorite_";

    public RecipeService(
        IMealDbApiService apiService,
        ILocalStorageService storageService,
        ILogger<RecipeService> logger)
    {
        _apiService = apiService;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<IEnumerable<Recipe>> SearchRecipesAsync(string searchTerm)
    {
        return await _apiService.SearchByNameAsync(searchTerm);
    }

    public async Task<IEnumerable<UserRecipe>> GetUserFavoritesAsync(string userId)
    {
        var key = $"{FAVORITES_KEY_PREFIX}{userId}";
        var favorites = await _storageService.GetItemAsync<List<UserRecipe>>(key);
        return favorites ?? new List<UserRecipe>();
    }

    public async Task AddToFavoritesAsync(string userId, Recipe recipe)
    {
        var favorites = (await GetUserFavoritesAsync(userId)).ToList();

        if (!favorites.Any(f => f.IdMeal == recipe.IdMeal))
        {
            var userRecipe = new UserRecipe
            {
                IdMeal = recipe.IdMeal,
                StrMeal = recipe.StrMeal,
                StrInstructions = recipe.StrInstructions,
                StrYoutube = recipe.StrYoutube,
                Ingredients = recipe.Ingredients,
                OwnerId = userId,
                UserNote = string.Empty,
                DateAdded = DateTime.UtcNow
            };

            favorites.Add(userRecipe);
            var key = $"{FAVORITES_KEY_PREFIX}{userId}";
            await _storageService.SetItemAsync(key, favorites);

            _logger.LogInformation("Recipe {RecipeId} added to favorites for user {UserId}",
                recipe.IdMeal, userId);
        }
    }

    // Autres méthodes...
}
```

---

## 3. Gestion de l'État (State Management)

### 3.1 Service d'État Global

#### IAppStateService.cs
```csharp
namespace menuMalin.Services.State;

public interface IAppStateService
{
    // État de recherche
    string CurrentSearchTerm { get; set; }
    IEnumerable<Recipe> SearchResults { get; set; }

    // État de liste de courses
    List<ShoppingListItem> ShoppingList { get; }

    // État utilisateur
    string? CurrentUserId { get; set; }

    // Événements pour notifier les changements
    event Action? OnSearchResultsChanged;
    event Action? OnShoppingListChanged;
    event Action? OnFavoritesChanged;

    // Méthodes
    void UpdateSearchResults(IEnumerable<Recipe> results);
    void AddToShoppingList(Ingredient ingredient, string recipeId);
    void RemoveFromShoppingList(string ingredientName);
    void ClearShoppingList();
    void NotifyFavoritesChanged();
}
```

#### AppStateService.cs
```csharp
namespace menuMalin.Services.State;

public class AppStateService : IAppStateService
{
    private string _currentSearchTerm = string.Empty;
    private IEnumerable<Recipe> _searchResults = Enumerable.Empty<Recipe>();
    private List<ShoppingListItem> _shoppingList = new();

    public string CurrentSearchTerm
    {
        get => _currentSearchTerm;
        set
        {
            if (_currentSearchTerm != value)
            {
                _currentSearchTerm = value;
            }
        }
    }

    public IEnumerable<Recipe> SearchResults
    {
        get => _searchResults;
        set
        {
            _searchResults = value;
            OnSearchResultsChanged?.Invoke();
        }
    }

    public List<ShoppingListItem> ShoppingList => _shoppingList;

    public string? CurrentUserId { get; set; }

    public event Action? OnSearchResultsChanged;
    public event Action? OnShoppingListChanged;
    public event Action? OnFavoritesChanged;

    public void UpdateSearchResults(IEnumerable<Recipe> results)
    {
        SearchResults = results;
    }

    public void AddToShoppingList(Ingredient ingredient, string recipeId)
    {
        if (!_shoppingList.Any(item =>
            item.IngredientName == ingredient.Name && item.RecipeId == recipeId))
        {
            _shoppingList.Add(new ShoppingListItem
            {
                IngredientName = ingredient.Name,
                Measure = ingredient.Measure,
                RecipeId = recipeId,
                IsChecked = false
            });
            OnShoppingListChanged?.Invoke();
        }
    }

    public void RemoveFromShoppingList(string ingredientName)
    {
        _shoppingList.RemoveAll(item => item.IngredientName == ingredientName);
        OnShoppingListChanged?.Invoke();
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
        OnShoppingListChanged?.Invoke();
    }

    public void NotifyFavoritesChanged()
    {
        OnFavoritesChanged?.Invoke();
    }
}
```

### 3.2 Utilisation dans les Composants

```razor
@inject IAppStateService AppState
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        AppState.OnSearchResultsChanged += StateHasChanged;
        AppState.OnShoppingListChanged += StateHasChanged;
    }

    public void Dispose()
    {
        AppState.OnSearchResultsChanged -= StateHasChanged;
        AppState.OnShoppingListChanged -= StateHasChanged;
    }
}
```

---

## 4. Configuration PWA

### 4.1 manifest.webmanifest

```json
{
  "name": "MenuMalin - Gestionnaire de Recettes",
  "short_name": "MenuMalin",
  "description": "Application de gestion de recettes avec liste de courses intelligente",
  "start_url": "/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#512bd4",
  "orientation": "portrait-primary",
  "icons": [
    {
      "src": "icon-192.png",
      "sizes": "192x192",
      "type": "image/png",
      "purpose": "any maskable"
    },
    {
      "src": "icon-512.png",
      "sizes": "512x512",
      "type": "image/png",
      "purpose": "any maskable"
    }
  ],
  "categories": ["food", "lifestyle"],
  "screenshots": [
    {
      "src": "screenshots/home.png",
      "sizes": "540x720",
      "type": "image/png"
    }
  ],
  "shortcuts": [
    {
      "name": "Rechercher",
      "short_name": "Search",
      "description": "Rechercher une recette",
      "url": "/search",
      "icons": [{ "src": "icon-search.png", "sizes": "96x96" }]
    },
    {
      "name": "Favoris",
      "short_name": "Favorites",
      "description": "Mes recettes favorites",
      "url": "/favorites",
      "icons": [{ "src": "icon-favorite.png", "sizes": "96x96" }]
    },
    {
      "name": "Liste de courses",
      "short_name": "Shopping",
      "description": "Ma liste de courses",
      "url": "/shopping-list",
      "icons": [{ "src": "icon-shopping.png", "sizes": "96x96" }]
    }
  ]
}
```

### 4.2 Service Worker (wwwroot/service-worker.js)

```javascript
// Service Worker pour mode offline
const CACHE_NAME = 'menumalin-cache-v1';
const STATIC_CACHE = 'menumalin-static-v1';
const API_CACHE = 'menumalin-api-v1';

// Ressources à mettre en cache immédiatement
const STATIC_RESOURCES = [
    '/',
    '/index.html',
    '/css/app.css',
    '/css/bootstrap/bootstrap.min.css',
    '/icon-192.png',
    '/icon-512.png',
    '/manifest.webmanifest'
];

// Installation du Service Worker
self.addEventListener('install', event => {
    console.log('Service Worker installing...');
    event.waitUntil(
        caches.open(STATIC_CACHE)
            .then(cache => cache.addAll(STATIC_RESOURCES))
            .then(() => self.skipWaiting())
    );
});

// Activation du Service Worker
self.addEventListener('activate', event => {
    console.log('Service Worker activating...');
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== STATIC_CACHE &&
                        cacheName !== API_CACHE &&
                        cacheName !== CACHE_NAME) {
                        return caches.delete(cacheName);
                    }
                })
            );
        }).then(() => self.clients.claim())
    );
});

// Stratégie de fetch: Network First pour API, Cache First pour static
self.addEventListener('fetch', event => {
    const { request } = event;
    const url = new URL(request.url);

    // API TheMealDB - Network First avec fallback cache
    if (url.hostname === 'www.themealdb.com') {
        event.respondWith(
            fetch(request)
                .then(response => {
                    const responseClone = response.clone();
                    caches.open(API_CACHE).then(cache => {
                        cache.put(request, responseClone);
                    });
                    return response;
                })
                .catch(() => {
                    return caches.match(request);
                })
        );
        return;
    }

    // Ressources statiques - Cache First
    event.respondWith(
        caches.match(request)
            .then(cachedResponse => {
                if (cachedResponse) {
                    return cachedResponse;
                }
                return fetch(request).then(response => {
                    if (response.status === 200) {
                        const responseClone = response.clone();
                        caches.open(STATIC_CACHE).then(cache => {
                            cache.put(request, responseClone);
                        });
                    }
                    return response;
                });
            })
    );
});

// Gestion des notifications push (future feature)
self.addEventListener('push', event => {
    const data = event.data.json();
    self.registration.showNotification(data.title, {
        body: data.body,
        icon: '/icon-192.png',
        badge: '/icon-192.png'
    });
});
```

### 4.3 Configuration dans index.html

```html
<!DOCTYPE html>
<html lang="fr">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>MenuMalin</title>
    <base href="/" />
    <link rel="stylesheet" href="css/bootstrap/bootstrap.min.css" />
    <link rel="stylesheet" href="css/app.css" />
    <link rel="icon" type="image/png" href="favicon.png" />
    <link rel="manifest" href="manifest.webmanifest" />
    <link rel="apple-touch-icon" sizes="512x512" href="icon-512.png" />
    <link rel="apple-touch-icon" sizes="192x192" href="icon-192.png" />

    <!-- PWA Meta Tags -->
    <meta name="theme-color" content="#512bd4" />
    <meta name="apple-mobile-web-app-capable" content="yes" />
    <meta name="apple-mobile-web-app-status-bar-style" content="black-translucent" />
    <meta name="apple-mobile-web-app-title" content="MenuMalin" />
</head>
<body>
    <div id="app">Loading...</div>

    <script src="_framework/blazor.webassembly.js"></script>
    <script>navigator.serviceWorker.register('service-worker.js');</script>
</body>
</html>
```

### 4.4 Configuration .csproj pour PWA

```xml
<PropertyGroup>
    <ServiceWorkerAssetsManifest>service-worker-assets.js</ServiceWorkerAssetsManifest>
    <BlazorEnableCompression>true</BlazorEnableCompression>
    <BlazorWebAssemblyPreserveCollationData>false</BlazorWebAssemblyPreserveCollationData>
    <InvariantGlobalization>false</InvariantGlobalization>
</PropertyGroup>

<ItemGroup>
    <ServiceWorker Include="wwwroot\service-worker.js"
                   PublishedContent="wwwroot\service-worker.published.js" />
</ItemGroup>
```

---

## 5. Stratégie de Tests

### 5.1 Structure des Tests

```
menuMalin.Tests/
├── Unit/
│   ├── Services/
│   │   ├── MealDbApiServiceTests.cs
│   │   ├── RecipeServiceTests.cs
│   │   ├── ShoppingListServiceTests.cs
│   │   └── LocalStorageServiceTests.cs
│   ├── Utilities/
│   │   ├── RecipeMapperTests.cs
│   │   └── ValidationHelperTests.cs
│   └── Models/
│       └── RecipeTests.cs
├── Integration/
│   ├── Api/
│   │   └── MealDbApiIntegrationTests.cs
│   └── Storage/
│       └── LocalStorageIntegrationTests.cs
├── Component/
│   ├── RecipeCardTests.cs
│   ├── SearchBarTests.cs
│   ├── ShoppingListTests.cs
│   └── FavoriteButtonTests.cs
├── E2E/
│   ├── SearchFlowTests.cs
│   ├── FavoritesFlowTests.cs
│   └── ShoppingListFlowTests.cs
└── Helpers/
    ├── TestAuthenticationStateProvider.cs
    ├── MockHttpMessageHandler.cs
    └── TestDataBuilder.cs
```

### 5.2 Configuration du Projet de Tests

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="bunit" Version="1.28.9" />
    <PackageReference Include="bunit.web" Version="1.28.9" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\menuMalin\menuMalin.csproj" />
  </ItemGroup>
</Project>
```

### 5.3 Tests Unitaires - Services

#### MealDbApiServiceTests.cs
```csharp
using Xunit;
using Moq;
using FluentAssertions;
using menuMalin.Services.Api;
using Microsoft.Extensions.Logging;

namespace menuMalin.Tests.Unit.Services;

public class MealDbApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly Mock<ILogger<MealDbApiService>> _mockLogger;
    private readonly MealDbApiService _sut;

    public MealDbApiServiceTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _mockLogger = new Mock<ILogger<MealDbApiService>>();

        var httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };

        _sut = new MealDbApiService(httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task SearchByNameAsync_WithValidName_ReturnsRecipes()
    {
        // Arrange
        var searchTerm = "chicken";
        var mockResponse = new MealDbResponse
        {
            Meals = new List<MealDbMeal>
            {
                new MealDbMeal { IdMeal = "1", StrMeal = "Chicken Curry" }
            }
        };

        _mockHttpHandler.SetupRequest(HttpMethod.Get, $"search.php?s={searchTerm}")
            .ReturnsResponse(mockResponse);

        // Act
        var result = await _sut.SearchByNameAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().StrMeal.Should().Be("Chicken Curry");
    }

    [Fact]
    public async Task SearchByNameAsync_WithInvalidName_ReturnsEmptyList()
    {
        // Arrange
        var searchTerm = "invalidrecipename123";
        var mockResponse = new MealDbResponse { Meals = null };

        _mockHttpHandler.SetupRequest(HttpMethod.Get, $"search.php?s={searchTerm}")
            .ReturnsResponse(mockResponse);

        // Act
        var result = await _sut.SearchByNameAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRecipeByIdAsync_WithValidId_ReturnsRecipe()
    {
        // Arrange
        var recipeId = "52772";
        var mockMeal = new MealDbMeal
        {
            IdMeal = recipeId,
            StrMeal = "Teriyaki Chicken"
        };

        _mockHttpHandler.SetupRequest(HttpMethod.Get, $"lookup.php?i={recipeId}")
            .ReturnsResponse(new MealDbResponse { Meals = new[] { mockMeal } });

        // Act
        var result = await _sut.GetRecipeByIdAsync(recipeId);

        // Assert
        result.Should().NotBeNull();
        result!.IdMeal.Should().Be(recipeId);
        result.StrMeal.Should().Be("Teriyaki Chicken");
    }
}
```

#### RecipeServiceTests.cs
```csharp
using Xunit;
using Moq;
using FluentAssertions;
using menuMalin.Services.Business;

namespace menuMalin.Tests.Unit.Services;

public class RecipeServiceTests
{
    private readonly Mock<IMealDbApiService> _mockApiService;
    private readonly Mock<ILocalStorageService> _mockStorageService;
    private readonly Mock<ILogger<RecipeService>> _mockLogger;
    private readonly RecipeService _sut;

    public RecipeServiceTests()
    {
        _mockApiService = new Mock<IMealDbApiService>();
        _mockStorageService = new Mock<ILocalStorageService>();
        _mockLogger = new Mock<ILogger<RecipeService>>();

        _sut = new RecipeService(
            _mockApiService.Object,
            _mockStorageService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task AddToFavoritesAsync_NewRecipe_AddsToStorage()
    {
        // Arrange
        var userId = "user123";
        var recipe = TestDataBuilder.CreateRecipe("1", "Test Recipe");

        _mockStorageService
            .Setup(x => x.GetItemAsync<List<UserRecipe>>($"favorite_{userId}"))
            .ReturnsAsync(new List<UserRecipe>());

        // Act
        await _sut.AddToFavoritesAsync(userId, recipe);

        // Assert
        _mockStorageService.Verify(
            x => x.SetItemAsync(
                $"favorite_{userId}",
                It.Is<List<UserRecipe>>(list => list.Count == 1 && list[0].IdMeal == "1")),
            Times.Once);
    }

    [Fact]
    public async Task AddToFavoritesAsync_DuplicateRecipe_DoesNotAddAgain()
    {
        // Arrange
        var userId = "user123";
        var recipe = TestDataBuilder.CreateRecipe("1", "Test Recipe");
        var existingFavorites = new List<UserRecipe>
        {
            new UserRecipe { IdMeal = "1", StrMeal = "Test Recipe", OwnerId = userId }
        };

        _mockStorageService
            .Setup(x => x.GetItemAsync<List<UserRecipe>>($"favorite_{userId}"))
            .ReturnsAsync(existingFavorites);

        // Act
        await _sut.AddToFavoritesAsync(userId, recipe);

        // Assert
        _mockStorageService.Verify(
            x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<List<UserRecipe>>()),
            Times.Never);
    }

    [Fact]
    public async Task IsFavoriteAsync_ExistingFavorite_ReturnsTrue()
    {
        // Arrange
        var userId = "user123";
        var recipeId = "1";
        var favorites = new List<UserRecipe>
        {
            new UserRecipe { IdMeal = recipeId, OwnerId = userId }
        };

        _mockStorageService
            .Setup(x => x.GetItemAsync<List<UserRecipe>>($"favorite_{userId}"))
            .ReturnsAsync(favorites);

        // Act
        var result = await _sut.IsFavoriteAsync(userId, recipeId);

        // Assert
        result.Should().BeTrue();
    }
}
```

### 5.4 Tests de Composants - bUnit

#### RecipeCardTests.cs
```csharp
using Bunit;
using Xunit;
using FluentAssertions;
using menuMalin.Components.Recipe;

namespace menuMalin.Tests.Component;

public class RecipeCardTests : TestContext
{
    [Fact]
    public void RecipeCard_RendersCorrectly_WithValidRecipe()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateRecipe("1", "Chicken Curry");

        // Act
        var cut = RenderComponent<RecipeCard>(parameters => parameters
            .Add(p => p.Recipe, recipe)
            .Add(p => p.IsFavorite, false));

        // Assert
        cut.Find("h3").TextContent.Should().Be("Chicken Curry");
        cut.Find("img").GetAttribute("src").Should().Be(recipe.StrMealThumb);
    }

    [Fact]
    public void RecipeCard_ClickEvent_TriggersCallback()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateRecipe("1", "Test Recipe");
        var wasClicked = false;

        // Act
        var cut = RenderComponent<RecipeCard>(parameters => parameters
            .Add(p => p.Recipe, recipe)
            .Add(p => p.OnCardClick, () => wasClicked = true));

        cut.Find(".recipe-card").Click();

        // Assert
        wasClicked.Should().BeTrue();
    }

    [Fact]
    public void FavoriteButton_Click_TogglesFavoriteState()
    {
        // Arrange
        var recipe = TestDataBuilder.CreateRecipe("1", "Test Recipe");
        var isFavorite = false;

        // Act
        var cut = RenderComponent<RecipeCard>(parameters => parameters
            .Add(p => p.Recipe, recipe)
            .Add(p => p.IsFavorite, isFavorite)
            .Add(p => p.OnToggleFavorite, (id) => isFavorite = !isFavorite));

        var favoriteBtn = cut.Find("button.favorite-btn");
        favoriteBtn.Click();

        // Assert
        isFavorite.Should().BeTrue();
    }
}
```

#### SearchBarTests.cs
```csharp
using Bunit;
using Xunit;
using FluentAssertions;
using menuMalin.Components.Search;

namespace menuMalin.Tests.Component;

public class SearchBarTests : TestContext
{
    [Fact]
    public void SearchBar_InputChange_TriggersDebounce()
    {
        // Arrange
        var searchTerm = string.Empty;
        var cut = RenderComponent<SearchBar>(parameters => parameters
            .Add(p => p.OnSearch, (term) => searchTerm = term)
            .Add(p => p.DebounceMs, 100));

        // Act
        var input = cut.Find("input");
        input.Change("chicken");

        // Wait for debounce
        System.Threading.Thread.Sleep(150);

        // Assert
        searchTerm.Should().Be("chicken");
    }

    [Fact]
    public void SearchBar_ButtonClick_TriggersImmediateSearch()
    {
        // Arrange
        var searchTerm = string.Empty;
        var cut = RenderComponent<SearchBar>(parameters => parameters
            .Add(p => p.OnSearch, (term) => searchTerm = term));

        // Act
        var input = cut.Find("input");
        input.Change("pasta");

        var button = cut.Find("button");
        button.Click();

        // Assert
        searchTerm.Should().Be("pasta");
    }
}
```

### 5.5 Tests d'Intégration

#### MealDbApiIntegrationTests.cs
```csharp
using Xunit;
using FluentAssertions;
using menuMalin.Services.Api;

namespace menuMalin.Tests.Integration.Api;

[Collection("API Integration Tests")]
public class MealDbApiIntegrationTests
{
    private readonly IMealDbApiService _apiService;

    public MealDbApiIntegrationTests()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/")
        };

        var logger = new Mock<ILogger<MealDbApiService>>().Object;
        _apiService = new MealDbApiService(httpClient, logger);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SearchByNameAsync_RealApi_ReturnsResults()
    {
        // Act
        var results = await _apiService.SearchByNameAsync("chicken");

        // Assert
        results.Should().NotBeNull();
        results.Should().NotBeEmpty();
        results.All(r => !string.IsNullOrEmpty(r.IdMeal)).Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetRecipeByIdAsync_RealApi_ReturnsRecipe()
    {
        // Arrange - ID d'une recette connue
        var recipeId = "52772";

        // Act
        var result = await _apiService.GetRecipeByIdAsync(recipeId);

        // Assert
        result.Should().NotBeNull();
        result!.IdMeal.Should().Be(recipeId);
        result.Ingredients.Should().NotBeEmpty();
    }
}
```

### 5.6 Helpers de Tests

#### TestDataBuilder.cs
```csharp
namespace menuMalin.Tests.Helpers;

public static class TestDataBuilder
{
    public static Recipe CreateRecipe(string id, string name)
    {
        return new Recipe
        {
            IdMeal = id,
            StrMeal = name,
            StrCategory = "Chicken",
            StrArea = "British",
            StrInstructions = "Test instructions",
            StrMealThumb = $"https://www.themealdb.com/images/media/meals/{id}.jpg",
            StrYoutube = "https://www.youtube.com/watch?v=test",
            Ingredients = new List<Ingredient>
            {
                new Ingredient { Name = "Chicken", Measure = "500g", IsMissing = false },
                new Ingredient { Name = "Salt", Measure = "1 tsp", IsMissing = false }
            }
        };
    }

    public static UserRecipe CreateUserRecipe(string id, string name, string userId)
    {
        var recipe = CreateRecipe(id, name);
        return new UserRecipe
        {
            IdMeal = recipe.IdMeal,
            StrMeal = recipe.StrMeal,
            StrInstructions = recipe.StrInstructions,
            StrYoutube = recipe.StrYoutube,
            Ingredients = recipe.Ingredients,
            OwnerId = userId,
            UserNote = "Test note",
            DateAdded = DateTime.UtcNow
        };
    }
}
```

#### TestAuthenticationStateProvider.cs
```csharp
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace menuMalin.Tests.Helpers;

public class TestAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationState _authState;

    public TestAuthenticationStateProvider(bool isAuthenticated, string? userId = null)
    {
        var identity = isAuthenticated
            ? new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId ?? "test-user-id"),
                new Claim(ClaimTypes.Name, "Test User")
            }, "Test")
            : new ClaimsIdentity();

        _authState = new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(_authState);
    }
}
```

### 5.7 Configuration CI/CD pour Tests

#### .github/workflows/test.yml
```yaml
name: Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Run Unit Tests
      run: dotnet test --no-build --verbosity normal --filter "Category!=Integration"

    - name: Run Integration Tests
      run: dotnet test --no-build --verbosity normal --filter "Category=Integration"

    - name: Generate Coverage Report
      run: dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

    - name: Upload Coverage
      uses: codecov/codecov-action@v3
```

---

## 6. Modèles de Données

### 6.1 Modèles Principaux

#### Recipe.cs
```csharp
namespace menuMalin.Models;

public class Recipe
{
    public string IdMeal { get; set; } = string.Empty;
    public string StrMeal { get; set; } = string.Empty;
    public string StrCategory { get; set; } = string.Empty;
    public string StrArea { get; set; } = string.Empty;
    public string StrInstructions { get; set; } = string.Empty;
    public string StrMealThumb { get; set; } = string.Empty;
    public string StrYoutube { get; set; } = string.Empty;
    public List<Ingredient> Ingredients { get; set; } = new();
}
```

#### UserRecipe.cs
```csharp
namespace menuMalin.Models;

public class UserRecipe : Recipe
{
    public string OwnerId { get; set; } = string.Empty;
    public string UserNote { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }
    public DateTime? LastModified { get; set; }
    public bool IsModified { get; set; }
}
```

#### Ingredient.cs
```csharp
namespace menuMalin.Models;

public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public string Measure { get; set; } = string.Empty;
    public bool IsMissing { get; set; }
}
```

#### ShoppingListItem.cs
```csharp
namespace menuMalin.Models;

public class ShoppingListItem
{
    public string IngredientName { get; set; } = string.Empty;
    public string Measure { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public bool IsChecked { get; set; }
}
```

---

## 7. Plan d'Implémentation par Phases

### Phase 1: Infrastructure de Base (Semaine 1)
- [X] Configuration du projet Blazor WebAssembly
- [ ] Configuration Auth0 OIDC
- [ ] Mise en place de l'injection de dépendances
- [ ] Configuration PWA de base
- [ ] Structure des dossiers et organisation

### Phase 2: Services API et Stockage (Semaine 2)
- [ ] Implémentation MealDbApiService
- [ ] Implémentation LocalStorageService
- [ ] Mapper API → Modèles
- [ ] Tests unitaires des services
- [ ] Tests d'intégration API

### Phase 3: Composants de Base (Semaine 3)
- [ ] RecipeCard component
- [ ] SearchBar component
- [ ] RecipeList component
- [ ] LoadingSpinner et ErrorBoundary
- [ ] Tests bUnit des composants

### Phase 4: Fonctionnalités Recherche (Semaine 4)
- [ ] Page Search
- [ ] Filtres de recherche
- [ ] Pagination
- [ ] State management pour recherche
- [ ] Tests end-to-end recherche

### Phase 5: Gestion des Favoris (Semaine 5)
- [ ] RecipeService avec CRUD favoris
- [ ] Page Favorites avec AuthorizeView
- [ ] FavoriteButton component
- [ ] Synchronisation LocalStorage
- [ ] Tests unitaires et intégration

### Phase 6: Édition de Recettes (Semaine 6)
- [ ] RecipeEditor component
- [ ] Validation des formulaires
- [ ] Sauvegarde des modifications
- [ ] Gestion des notes utilisateur
- [ ] Tests de formulaires

### Phase 7: Liste de Courses (Semaine 7)
- [ ] ShoppingListService
- [ ] IngredientCheckbox component
- [ ] Page ShoppingList
- [ ] Export/Print functionality
- [ ] Tests du workflow complet

### Phase 8: Optimisations PWA (Semaine 8)
- [ ] Service Worker avancé
- [ ] Cache strategies
- [ ] Offline support
- [ ] Install prompt
- [ ] Lighthouse audit

### Phase 9: Tests et QA (Semaine 9)
- [ ] Tests end-to-end complets
- [ ] Tests de performance
- [ ] Accessibilité (WCAG 2.1)
- [ ] Corrections de bugs
- [ ] Documentation utilisateur

### Phase 10: Déploiement (Semaine 10)
- [ ] Dockerisation
- [ ] Configuration Coolify
- [ ] CI/CD pipeline
- [ ] Monitoring et logs
- [ ] Documentation technique

---

## 8. Dépendances NuGet Requises

```xml
<ItemGroup>
  <!-- Blazor Core -->
  <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.11" />
  <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0.11" />

  <!-- Authentication -->
  <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="9.0.11" />

  <!-- Storage -->
  <PackageReference Include="Blazored.LocalStorage" Version="4.5.0" />

  <!-- HTTP & JSON -->
  <PackageReference Include="System.Net.Http.Json" Version="9.0.0" />

  <!-- Tests -->
  <PackageReference Include="bunit" Version="1.28.9" />
  <PackageReference Include="xunit" Version="2.6.6" />
  <PackageReference Include="Moq" Version="4.20.70" />
  <PackageReference Include="FluentAssertions" Version="6.12.0" />
</ItemGroup>
```

---

## 9. Métriques de Qualité

### Critères de Succès
- **Couverture de tests**: Minimum 80%
- **Performance Lighthouse**: Score > 90
- **Accessibilité**: WCAG 2.1 Level AA
- **PWA Score**: 100/100
- **Temps de chargement**: < 3 secondes (3G)
- **Bundle size**: < 2MB (gzipped)

### KPIs de Développement
- Toutes les user stories implémentées
- 0 bugs critiques en production
- Documentation complète et à jour
- Tests automatisés passant à 100%
- Code review avant chaque merge

---

## 10. Risques et Mitigations

| Risque | Impact | Probabilité | Mitigation |
|--------|--------|-------------|------------|
| API TheMealDB indisponible | Élevé | Faible | Cache local + fallback data |
| Quota LocalStorage dépassé | Moyen | Moyen | Limit favoris + compression |
| Auth0 configuration complexe | Moyen | Élevé | Documentation détaillée |
| Performance PWA sur mobile | Élevé | Moyen | Lazy loading + code splitting |
| Tests bUnit instables | Faible | Moyen | Isolation des tests + mocks |

---

## Prochaines Étapes

1. **Valider** ce plan technique avec le formateur
2. **Configurer** l'environnement de développement
3. **Créer** le projet de tests unitaires
4. **Commencer** Phase 1: Infrastructure de Base
5. **Établir** la stratégie de branches Git (main, develop, feature/*)

---

## Ressources Complémentaires

- [Blazor WebAssembly Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [bUnit Documentation](https://bunit.dev/)
- [TheMealDB API Documentation](https://www.themealdb.com/api.php)
- [Auth0 Blazor Quickstart](https://auth0.com/docs/quickstart/spa/blazor-webassembly)
- [PWA Best Practices](https://web.dev/progressive-web-apps/)
