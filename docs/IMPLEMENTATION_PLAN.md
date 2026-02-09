# Plan d'Implémentation - MenuMalin

Date de création: 9 février 2026
Date de livraison: 8 mars 2026
Durée totale: 4 semaines

## 📅 Timeline Globale

```
Semaine 1 (10-16 fév)  : Phase 1 - Corrections critiques + Infrastructure
Semaine 2 (17-23 fév)  : Phase 2 - Services Core + API
Semaine 3 (24 fév-2 mar): Phase 3 - Composants + Fonctionnalités
Semaine 4 (3-8 mars)   : Phase 4 - Tests + Polissage + Déploiement
```

---

## 🎯 Phase 1: Corrections Critiques + Infrastructure
**Durée**: 10-16 février 2026 (7 jours)
**Objectif**: Corriger tous les bugs bloquants et établir une base solide

### Tâches Prioritaires

#### 1.1 Corrections Critiques (Jour 1-2)

**✅ Tâche 1.1.1**: Créer le composant RedirectToLogin
- **Fichier**: `menuMalin/Components/Authentication/RedirectToLogin.razor`
- **Code**:
```razor
@inject NavigationManager Navigation
@code {
    protected override void OnInitialized()
    {
        Navigation.NavigateTo("authentication/login");
    }
}
```
- **Temps estimé**: 15 minutes
- **Priorité**: 🔴 Critique

**✅ Tâche 1.1.2**: Corriger la configuration HttpClient
- **Fichier**: `menuMalin/Program.cs`
- **Modifications**:
```csharp
// Remplacer ligne 9
builder.Services.AddHttpClient("MealDbAPI", client =>
{
    client.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```
- **Temps estimé**: 20 minutes
- **Priorité**: 🔴 Critique

**✅ Tâche 1.1.3**: Enregistrer RecipeService dans DI
- **Fichier**: `menuMalin/Program.cs`
- **Code**:
```csharp
// Modifier le constructeur de RecipeService pour accepter IHttpClientFactory
// Puis ajouter après la configuration HttpClient:
builder.Services.AddScoped<IRecipeService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return new RecipeService(factory.CreateClient("MealDbAPI"));
});
```
- **Temps estimé**: 30 minutes
- **Priorité**: 🔴 Critique

**✅ Tâche 1.1.4**: Configurer Auth0
- **Fichiers**: `appsettings.json`, `Program.cs`
- **Actions**:
  1. Créer compte Auth0
  2. Créer application SPA
  3. Configurer Allowed Callback URLs
  4. Copier Domain et ClientId
  5. Mettre à jour appsettings.json
- **Temps estimé**: 1 heure
- **Priorité**: 🔴 Critique

#### 1.2 Installation des Packages Manquants (Jour 2)

**✅ Tâche 1.2.1**: Installer Blazored.LocalStorage
```bash
cd menuMalin
dotnet add package Blazored.LocalStorage
```
- **Configuration** dans Program.cs:
```csharp
builder.Services.AddBlazoredLocalStorage();
```
- **Temps estimé**: 10 minutes
- **Priorité**: 🟠 Majeur

**✅ Tâche 1.2.2**: Installer packages de logging (optionnel)
```bash
dotnet add package Serilog.Extensions.Logging
dotnet add package Serilog.Sinks.Console
```
- **Temps estimé**: 15 minutes
- **Priorité**: 🟡 Mineur

#### 1.3 Tests de Compilation et Lancement (Jour 2-3)

**✅ Tâche 1.3.1**: Compiler l'application
```bash
dotnet build
```
- Corriger toutes les erreurs de compilation
- **Temps estimé**: 30 minutes

**✅ Tâche 1.3.2**: Tester le lancement
```bash
dotnet run --project menuMalin
```
- Vérifier que l'app démarre sans erreur
- Tester la navigation
- Tester l'authentification (login/logout)
- **Temps estimé**: 1 heure

#### 1.4 Amélioration des Modèles (Jour 3-4)

**✅ Tâche 1.4.1**: Ajouter validation aux modèles
- **Fichier**: `menuMalin/Models/Recipe.cs`
- **Code**:
```csharp
using System.ComponentModel.DataAnnotations;

public class Recipe
{
    [Required]
    public required string IdMeal { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string StrMeal { get; set; }

    [StringLength(50)]
    public string? StrCategory { get; set; }

    [StringLength(50)]
    public string? StrArea { get; set; }

    [Url]
    public string? StrMealThumb { get; set; }

    [Url]
    public string? StrYoutube { get; set; }

    // ... autres propriétés
}
```
- **Temps estimé**: 45 minutes
- **Priorité**: 🟠 Majeur

#### 1.5 Amélioration de RecipeService (Jour 4-5)

**✅ Tâche 1.5.1**: Ajouter gestion d'erreurs complète
- **Fichier**: `menuMalin/Services/RecipeService.cs`
- **Code**:
```csharp
public async Task<List<Recipe>> SearchByNameAsync(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        return new List<Recipe>();

    try
    {
        var response = await _http.GetFromJsonAsync<RecipeResponse>(
            $"search.php?s={Uri.EscapeDataString(name)}"
        );
        return response?.Meals ?? new List<Recipe>();
    }
    catch (HttpRequestException ex)
    {
        Console.Error.WriteLine($"Erreur réseau lors de la recherche: {ex.Message}");
        return new List<Recipe>();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Erreur inattendue lors de la recherche: {ex.Message}");
        return new List<Recipe>();
    }
}
```
- **Temps estimé**: 1 heure
- **Priorité**: 🟠 Majeur

**✅ Tâche 1.5.2**: Paralléliser les appels API
- **Méthode**: `GetRandomRecipesAsync`
- **Code**:
```csharp
public async Task<List<Recipe>> GetRandomRecipesAsync(int count = 6)
{
    try
    {
        var tasks = Enumerable.Range(0, count)
            .Select(_ => _http.GetFromJsonAsync<RecipeResponse>("random.php"));

        var responses = await Task.WhenAll(tasks);

        return responses
            .Where(r => r?.Meals != null && r.Meals.Count > 0)
            .SelectMany(r => r.Meals)
            .ToList();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Erreur lors de la récupération des recettes aléatoires: {ex.Message}");
        return new List<Recipe>();
    }
}
```
- **Temps estimé**: 30 minutes
- **Priorité**: 🟠 Majeur

### Livrables Phase 1
- ✅ Application compile sans erreur
- ✅ Application démarre et fonctionne
- ✅ Authentification Auth0 configurée et fonctionnelle
- ✅ Tous les bugs critiques corrigés
- ✅ Gestion d'erreurs basique implémentée

---

## 🔧 Phase 2: Services Core + API
**Durée**: 17-23 février 2026 (7 jours)
**Objectif**: Implémenter tous les services nécessaires

### 2.1 Service MealDbApiService (Jour 1-2)

**✅ Tâche 2.1.1**: Créer l'interface
- **Fichier**: `menuMalin/Services/Api/IMealDbApiService.cs`
- **Code**:
```csharp
public interface IMealDbApiService
{
    Task<List<Recipe>> SearchByNameAsync(string name);
    Task<List<Recipe>> SearchByFirstLetterAsync(char letter);
    Task<Recipe?> GetRecipeByIdAsync(string id);
    Task<List<Recipe>> GetRandomRecipesAsync(int count = 6);
    Task<List<string>> GetCategoriesAsync();
    Task<List<string>> GetAreasAsync();
}
```
- **Temps estimé**: 20 minutes

**✅ Tâche 2.1.2**: Implémenter le service
- **Fichier**: `menuMalin/Services/Api/MealDbApiService.cs`
- **Inclure**: Retry policy avec Polly (optionnel)
- **Temps estimé**: 2 heures

**✅ Tâche 2.1.3**: Enregistrer dans DI
- **Fichier**: `menuMalin/Program.cs`
```csharp
builder.Services.AddScoped<IMealDbApiService, MealDbApiService>();
```
- **Temps estimé**: 5 minutes

### 2.2 Service LocalStorageService (Jour 2-3)

**✅ Tâche 2.2.1**: Créer abstraction typée
- **Fichier**: `menuMalin/Services/Storage/ILocalStorageService.cs`
- **Code**:
```csharp
public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
    Task<bool> ContainKeyAsync(string key);
    Task ClearAsync();
}
```
- **Temps estimé**: 15 minutes

**✅ Tâche 2.2.2**: Implémenter avec Blazored.LocalStorage
- **Fichier**: `menuMalin/Services/Storage/LocalStorageService.cs`
- **Wrapper**: Ajouter logging et gestion d'erreurs
- **Temps estimé**: 1 heure

### 2.3 Service FavoriteService (Jour 3-4)

**✅ Tâche 2.3.1**: Créer interface et modèles
- **Fichier**: `menuMalin/Services/Business/IFavoriteService.cs`
- **Code**:
```csharp
public interface IFavoriteService
{
    Task<List<Recipe>> GetFavoritesAsync();
    Task AddFavoriteAsync(Recipe recipe);
    Task RemoveFavoriteAsync(string recipeId);
    Task<bool> IsFavoriteAsync(string recipeId);
    Task UpdateFavoriteAsync(Recipe recipe);
    event Action? OnChange;
}
```
- **Temps estimé**: 30 minutes

**✅ Tâche 2.3.2**: Implémenter avec LocalStorage
- **Stockage**: Clé `menumalin_favorites`
- **Limite**: Max 50 recettes
- **Temps estimé**: 2 heures

### 2.4 Service ShoppingListService (Jour 4-5)

**✅ Tâche 2.4.1**: Créer modèles
- **Fichier**: `menuMalin/Models/ShoppingList.cs`
```csharp
public class ShoppingListItem
{
    public string Ingredient { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public bool IsChecked { get; set; }
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
}

public class ShoppingList
{
    public List<ShoppingListItem> Items { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}
```
- **Temps estimé**: 20 minutes

**✅ Tâche 2.4.2**: Implémenter service
- **Fichier**: `menuMalin/Services/Business/ShoppingListService.cs`
- **Fonctionnalités**:
  - Ajouter/retirer items
  - Cocher/décocher
  - Générer depuis recette
  - Export texte
- **Temps estimé**: 2 heures

### 2.5 Service AppStateService (Jour 5-6)

**✅ Tâche 2.5.1**: Créer service de gestion d'état global
- **Fichier**: `menuMalin/Services/State/AppStateService.cs`
- **Pattern**: Observable avec events
- **État géré**:
  - Résultats de recherche
  - Loading states
  - Erreurs globales
- **Temps estimé**: 2 heures

**✅ Tâche 2.5.2**: Enregistrer comme Singleton
```csharp
builder.Services.AddSingleton<IAppStateService, AppStateService>();
```
- **Temps estimé**: 5 minutes

### 2.6 Tests Unitaires Services (Jour 6-7)

**✅ Tâche 2.6.1**: Configurer projet de tests
```bash
dotnet new xunit -n menuMalin.Tests
cd menuMalin.Tests
dotnet add reference ../menuMalin/menuMalin.csproj
dotnet add package Moq
dotnet add package FluentAssertions
```
- **Temps estimé**: 30 minutes

**✅ Tâche 2.6.2**: Écrire tests pour MealDbApiService
- **Fichier**: `menuMalin.Tests/Services/MealDbApiServiceTests.cs`
- **Tests**:
  - Search by name returns recipes
  - Search with empty name returns empty list
  - API error is handled gracefully
- **Temps estimé**: 2 heures

**✅ Tâche 2.6.3**: Écrire tests pour FavoriteService
- **Coverage cible**: 80%
- **Temps estimé**: 1.5 heures

### Livrables Phase 2
- ✅ Tous les services core implémentés
- ✅ Gestion d'erreurs robuste
- ✅ LocalStorage fonctionnel
- ✅ Tests unitaires > 70% coverage
- ✅ Documentation des services

---

## 🎨 Phase 3: Composants + Fonctionnalités
**Durée**: 24 février - 2 mars 2026 (7 jours)
**Objectif**: Créer tous les composants UI et implémenter les fonctionnalités

### 3.1 Composants Communs (Jour 1-2)

**✅ Tâche 3.1.1**: LoadingSpinner.razor
```razor
@if (IsLoading)
{
    <div class="loading-overlay">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">@LoadingText</span>
        </div>
    </div>
}

@code {
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public string LoadingText { get; set; } = "Chargement...";
}
```
- **+ CSS**: `LoadingSpinner.razor.css`
- **Temps estimé**: 30 minutes

**✅ Tâche 3.1.2**: ErrorBoundary.razor
- **Capture**: Erreurs non gérées
- **Display**: Message user-friendly
- **Temps estimé**: 45 minutes

**✅ Tâche 3.1.3**: Toast.razor (notifications)
- **Types**: Success, Error, Info, Warning
- **Auto-dismiss**: 3 secondes
- **Temps estimé**: 1 heure

### 3.2 Composants Recipe (Jour 2-4)

**✅ Tâche 3.2.1**: RecipeCard.razor
```razor
<div class="recipe-card" @onclick="NavigateToDetails">
    <div class="recipe-card-image">
        <img src="@Recipe.StrMealThumb" alt="@Recipe.StrMeal" loading="lazy" />
        <FavoriteButton RecipeId="@Recipe.IdMeal" />
    </div>
    <div class="recipe-card-body">
        <h3>@Recipe.StrMeal</h3>
        <div class="recipe-card-meta">
            <span class="badge bg-primary">@Recipe.StrCategory</span>
            <span class="badge bg-secondary">@Recipe.StrArea</span>
        </div>
    </div>
</div>

@code {
    [Parameter, EditorRequired] public Recipe Recipe { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private void NavigateToDetails()
    {
        Navigation.NavigateTo($"/recipe/{Recipe.IdMeal}");
    }
}
```
- **+ CSS**: Hover effects, transitions
- **Temps estimé**: 2 heures

**✅ Tâche 3.2.2**: RecipeList.razor
- **Grid responsive**: 3 colonnes desktop, 1 mobile
- **Pagination**: 12 recettes par page
- **Temps estimé**: 1.5 heures

**✅ Tâche 3.2.3**: RecipeDetails.razor (Page complète)
```razor
@page "/recipe/{RecipeId}"
@inject IMealDbApiService ApiService
@inject IFavoriteService FavoriteService

@if (recipe == null)
{
    <LoadingSpinner IsLoading="true" />
}
else
{
    <div class="recipe-details">
        <RecipeHeader Recipe="@recipe" />
        <div class="row">
            <div class="col-md-6">
                <IngredientsList Ingredients="@GetIngredients()" />
                <AddToShoppingListButton Recipe="@recipe" />
            </div>
            <div class="col-md-6">
                <Instructions Text="@recipe.StrInstructions" />
                @if (!string.IsNullOrEmpty(recipe.StrYoutube))
                {
                    <YouTubePlayer VideoUrl="@recipe.StrYoutube" />
                }
            </div>
        </div>
    </div>
}
```
- **Sous-composants**: RecipeHeader, IngredientsList, Instructions
- **Temps estimé**: 4 heures

**✅ Tâche 3.2.4**: YouTubePlayer.razor
- **Embed**: iframe YouTube responsive
- **Temps estimé**: 30 minutes

### 3.3 Composants Search (Jour 4-5)

**✅ Tâche 3.3.1**: SearchBar.razor
```razor
<div class="search-bar">
    <input type="search"
           class="form-control form-control-lg"
           placeholder="Rechercher une recette..."
           @bind="searchTerm"
           @bind:event="oninput"
           @bind:after="OnSearchChanged" />
    @if (isSearching)
    {
        <span class="search-spinner spinner-border spinner-border-sm"></span>
    }
</div>

@code {
    private string searchTerm = string.Empty;
    private bool isSearching;
    private System.Threading.Timer? debounceTimer;

    [Parameter] public EventCallback<string> OnSearch { get; set; }

    private void OnSearchChanged()
    {
        debounceTimer?.Dispose();
        debounceTimer = new Timer(async _ =>
        {
            isSearching = true;
            await InvokeAsync(StateHasChanged);
            await OnSearch.InvokeAsync(searchTerm);
            isSearching = false;
            await InvokeAsync(StateHasChanged);
        }, null, 500, Timeout.Infinite);
    }

    public void Dispose() => debounceTimer?.Dispose();
}
```
- **Debounce**: 500ms
- **Temps estimé**: 1.5 heures

**✅ Tâche 3.3.2**: SearchFilters.razor
- **Filtres**: Catégorie, Zone
- **Temps estimé**: 2 heures

**✅ Tâche 3.3.3**: SearchResults.razor (Page)
```razor
@page "/search"
@inject IMealDbApiService ApiService
@inject IAppStateService AppState

<div class="search-page">
    <SearchBar OnSearch="HandleSearch" />
    <SearchFilters OnFilterChanged="HandleFilterChange" />

    @if (isLoading)
    {
        <LoadingSpinner IsLoading="true" />
    }
    else if (recipes.Count == 0)
    {
        <p class="text-center text-muted">Aucune recette trouvée</p>
    }
    else
    {
        <RecipeList Recipes="@recipes" />
    }
</div>
```
- **Temps estimé**: 2 heures

### 3.4 Composants Favorites (Jour 5-6)

**✅ Tâche 3.4.1**: FavoriteButton.razor
```razor
<button class="btn btn-favorite @(isFavorite ? "active" : "")"
        @onclick="ToggleFavorite"
        @onclick:stopPropagation="true">
    <i class="bi @(isFavorite ? "bi-heart-fill" : "bi-heart")"></i>
</button>

@code {
    [Parameter, EditorRequired] public string RecipeId { get; set; } = null!;
    [Inject] private IFavoriteService FavoriteService { get; set; } = null!;

    private bool isFavorite;

    protected override async Task OnInitializedAsync()
    {
        isFavorite = await FavoriteService.IsFavoriteAsync(RecipeId);
        FavoriteService.OnChange += StateHasChanged;
    }

    private async Task ToggleFavorite()
    {
        // Implementation...
    }

    public void Dispose()
    {
        FavoriteService.OnChange -= StateHasChanged;
    }
}
```
- **Animation**: Heart pulse on click
- **Temps estimé**: 1 heure

**✅ Tâche 3.4.2**: FavoritesList.razor + Page
```razor
@page "/favorites"
@attribute [Authorize]
@inject IFavoriteService FavoriteService

<div class="favorites-page">
    <h1>Mes Favoris</h1>
    @if (favorites.Count == 0)
    {
        <EmptyState Message="Aucun favori pour le moment" />
    }
    else
    {
        <RecipeList Recipes="@favorites" />
    }
</div>
```
- **Temps estimé**: 1.5 heures

### 3.5 Composants Shopping (Jour 6-7)

**✅ Tâche 3.5.1**: ShoppingList.razor (Page)
```razor
@page "/shopping-list"
@attribute [Authorize]
@inject IShoppingListService ShoppingService

<div class="shopping-list-page">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1>Ma Liste de Courses</h1>
        <button class="btn btn-primary" @onclick="ExportList">
            <i class="bi bi-download"></i> Exporter
        </button>
    </div>

    @foreach (var item in shoppingList.Items)
    {
        <IngredientCheckbox Item="@item" OnToggle="HandleToggle" />
    }
</div>
```
- **Temps estimé**: 2 heures

**✅ Tâche 3.5.2**: IngredientCheckbox.razor
- **Check/Uncheck**: Toggle item
- **Style**: Strikethrough when checked
- **Temps estimé**: 45 minutes

**✅ Tâche 3.5.3**: Export fonctionnalité
- **Format**: Plain text
- **Download**: Blob + download
- **Temps estimé**: 1 heure

### 3.6 Tests Composants (Jour 7)

**✅ Tâche 3.6.1**: Configurer bUnit
```bash
cd menuMalin.Tests
dotnet add package bUnit
```
- **Temps estimé**: 15 minutes

**✅ Tâche 3.6.2**: Tests RecipeCard
```csharp
[Fact]
public void RecipeCard_RendersCorrectly()
{
    // Arrange
    var recipe = new Recipe { IdMeal = "1", StrMeal = "Test" };

    // Act
    var cut = RenderComponent<RecipeCard>(parameters =>
        parameters.Add(p => p.Recipe, recipe));

    // Assert
    cut.Find("h3").TextContent.Should().Be("Test");
}
```
- **Temps estimé**: 2 heures

### Livrables Phase 3
- ✅ Tous les composants UI créés
- ✅ Navigation complète fonctionnelle
- ✅ Recherche et filtres opérationnels
- ✅ Favoris et liste de courses fonctionnels
- ✅ Tests composants > 60% coverage

---

## 🚀 Phase 4: Tests + Polissage + Déploiement
**Durée**: 3-8 mars 2026 (6 jours)
**Objectif**: Finaliser, tester et déployer

### 4.1 Tests Complets (Jour 1-2)

**✅ Tâche 4.1.1**: Tests d'intégration API
- **Mock**: HttpClient avec responses
- **Temps estimé**: 2 heures

**✅ Tâche 4.1.2**: Tests E2E avec Playwright (optionnel)
```bash
dotnet new tool-manifest
dotnet tool install Microsoft.Playwright.CLI
playwright install
```
- **Scenarios**: Login, Search, Add Favorite, Shopping List
- **Temps estimé**: 3 heures

**✅ Tâche 4.1.3**: Améliorer coverage
- **Cible**: > 80% pour services
- **Temps estimé**: 2 heures

### 4.2 Polissage UI/UX (Jour 2-3)

**✅ Tâche 4.2.1**: Animations et transitions
- **Loading states** partout
- **Smooth transitions** entre pages
- **Temps estimé**: 2 heures

**✅ Tâche 4.2.2**: Responsive final check
- **Test**: iPhone, iPad, Desktop
- **Corrections**: Breakpoints
- **Temps estimé**: 1.5 heures

**✅ Tâche 4.2.3**: Accessibilité (ARIA labels)
- **Audit**: Lighthouse accessibility score
- **Cible**: > 90
- **Temps estimé**: 1.5 heures

### 4.3 Service Worker Production (Jour 3-4)

**✅ Tâche 4.3.1**: Implémenter cache strategy
- **Fichier**: `wwwroot/service-worker.published.js`
```javascript
const CACHE_NAME = 'menumalin-v1';
const urlsToCache = [
  '/',
  '/css/app.css',
  '/css/bootstrap/bootstrap.min.css',
  '/images/logo.png'
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(urlsToCache))
  );
});

self.addEventListener('fetch', event => {
  event.respondWith(
    caches.match(event.request)
      .then(response => response || fetch(event.request))
  );
});
```
- **Strategy**: Cache-first pour assets, Network-first pour API
- **Temps estimé**: 3 heures

**✅ Tâche 4.3.2**: Test offline capability
- **Temps estimé**: 1 heure

### 4.4 Performance Optimization (Jour 4-5)

**✅ Tâche 4.4.1**: Bundle size optimization
```xml
<PropertyGroup>
  <BlazorEnableCompression>true</BlazorEnableCompression>
  <PublishTrimmed>true</PublishTrimmed>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```
- **Temps estimé**: 30 minutes

**✅ Tâche 4.4.2**: Lazy loading routes
```csharp
@page "/favorites"
@attribute [StreamRendering(true)]
```
- **Temps estimé**: 1 heure

**✅ Tâche 4.4.3**: Image optimization
- **WebP conversion**: Logo et icons
- **Lazy loading**: `loading="lazy"` sur images
- **Temps estimé**: 1 heure

**✅ Tâche 4.4.4**: Lighthouse audit
- **Cibles**:
  - Performance: > 90
  - Accessibility: > 90
  - Best Practices: > 90
  - SEO: > 80
- **Temps estimé**: 2 heures (+ corrections)

### 4.5 Documentation (Jour 5)

**✅ Tâche 4.5.1**: README.md utilisateur
- **Sections**:
  - Comment utiliser l'app
  - Fonctionnalités principales
  - FAQ
- **Temps estimé**: 1 heure

**✅ Tâche 4.5.2**: Documentation technique finale
- **Architecture diagrams**
- **API documentation**
- **Deployment guide**
- **Temps estimé**: 2 heures

### 4.6 Déploiement (Jour 6)

**✅ Tâche 4.6.1**: Configuration Coolify
- **Docker**: Créer Dockerfile
```dockerfile
FROM nginx:alpine
COPY wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
```
- **Temps estimé**: 1 heure

**✅ Tâche 4.6.2**: Build production
```bash
dotnet publish -c Release -o ./publish
```
- **Test**: Build réussit sans erreur
- **Temps estimé**: 30 minutes

**✅ Tâche 4.6.3**: Deploy sur Coolify
- **Configuration**: Variables d'environnement
- **DNS**: Configurer domaine
- **SSL**: Certificat auto (Let's Encrypt)
- **Temps estimé**: 1.5 heures

**✅ Tâche 4.6.4**: Tests post-déploiement
- **Smoke tests**: Toutes fonctionnalités
- **Performance**: Mesurer temps de chargement
- **Temps estimé**: 1 heure

### Livrables Phase 4
- ✅ Application 100% fonctionnelle
- ✅ Tests complets passés
- ✅ Performance optimisée (Lighthouse > 90)
- ✅ Documentation complète
- ✅ Application déployée en production
- ✅ Projet prêt pour démonstration

---

## 📊 Métriques de Succès

### Coverage Cible
- **Services**: > 80%
- **Composants**: > 60%
- **Global**: > 70%

### Performance Cible
- **First Contentful Paint**: < 2s
- **Time to Interactive**: < 4s
- **Lighthouse Performance**: > 90
- **Bundle size (gzipped)**: < 2MB

### Qualité Code
- **0** bugs critiques
- **0** warnings compilation
- **Code style**: Conforme .editorconfig
- **Documentation**: Toutes méthodes publiques commentées

---

## ⚠️ Risques et Mitigation

### Risque 1: Retard sur Phase 2 (Services)
- **Impact**: Moyen
- **Probabilité**: Moyenne
- **Mitigation**: Prioriser MealDbApiService et FavoriteService, reporter logging

### Risque 2: Problèmes Auth0
- **Impact**: Élevé
- **Probabilité**: Faible
- **Mitigation**: Avoir documentation Auth0 à portée, forum communauté

### Risque 3: Performance WASM
- **Impact**: Moyen
- **Probabilité**: Faible
- **Mitigation**: Lazy loading, compression, CDN

### Risque 4: Bugs de dernière minute
- **Impact**: Moyen
- **Probabilité**: Moyenne
- **Mitigation**: Buffer de 1 jour avant livraison (7 mars au lieu de 8)

---

## 📝 Notes Importantes

1. **Timeboxing**: Respecter les temps estimés, ne pas sur-engineerer
2. **MVP First**: Fonctionnalités core avant polish
3. **Tests Continus**: Tester au fur et à mesure, pas à la fin
4. **Commits Réguliers**: Au moins 1 commit/jour avec messages clairs
5. **Documentation**: Documenter pendant le développement, pas après

---

**Ce plan est ambitieux mais réalisable en 4 semaines de travail focalisé.**
