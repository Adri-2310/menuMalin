# Guide d'Implémentation Auth0 OIDC pour MenuMalin

**Projet:** MenuMalin - Blazor WebAssembly
**Framework:** .NET 9.0
**Protocole:** OpenID Connect (OIDC)
**Provider:** Auth0
**Date:** Février 2026

---

## Table des Matières

1. [Configuration Auth0](#1-configuration-auth0)
2. [Intégration Blazor WebAssembly](#2-intégration-blazor-webassembly)
3. [Protection des Routes et Composants](#3-protection-des-routes-et-composants)
4. [Gestion des Tokens et du State](#4-gestion-des-tokens-et-du-state)
5. [Checklist de Sécurité OIDC](#5-checklist-de-sécurité-oidc)
6. [Tests et Validation](#6-tests-et-validation)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. Configuration Auth0

### 1.1 Création du Tenant Auth0

#### Étape 1: Créer un compte Auth0
1. Accédez à [auth0.com](https://auth0.com)
2. Créez un compte gratuit (Free Tier suffit pour le développement)
3. Choisissez une région (EU pour la conformité RGPD)
4. Notez votre **Domain** (ex: `menumalin.eu.auth0.com`)

#### Étape 2: Créer une Application
1. Dans le dashboard Auth0, allez dans **Applications** → **Applications**
2. Cliquez sur **Create Application**
3. Paramètres:
   - **Name:** MenuMalin WASM
   - **Type:** Single Page Web Applications
   - **Technology:** Blazor WebAssembly
4. Cliquez sur **Create**

### 1.2 Configuration de l'Application

#### URLs de Callback
Dans les **Application Settings**, configurez:

```
Allowed Callback URLs:
https://localhost:7123/authentication/login-callback
https://menumalin.votredomaine.com/authentication/login-callback

Allowed Logout URLs:
https://localhost:7123/authentication/logout-callback
https://menumalin.votredomaine.com/authentication/logout-callback

Allowed Web Origins:
https://localhost:7123
https://menumalin.votredomaine.com

Allowed Origins (CORS):
https://localhost:7123
https://menumalin.votredomaine.com
```

#### Advanced Settings

**OAuth**
- JsonWebToken Signature Algorithm: `RS256`
- OIDC Conformant: `Activé`

**Grant Types**
- Autorisation Code
- Refresh Token (optionnel, pour sessions longues)

**ID Token Expiration:** 36000 secondes (10 heures)

#### Récupération des Informations

Notez ces valeurs (vous en aurez besoin):
- **Domain:** `menumalin.eu.auth0.com`
- **Client ID:** (ex: `AbCd1234EfGh5678IjKl`)
- **Client Secret:** N/A (pas nécessaire pour SPA)

### 1.3 Configuration des APIs (Optionnel)

Si vous créez une API backend ultérieurement:

1. Allez dans **Applications** → **APIs**
2. Créez une nouvelle API:
   - **Name:** MenuMalin API
   - **Identifier:** `https://api.menumalin.com`
   - **Signing Algorithm:** RS256

### 1.4 Configuration des Scopes

Dans **APIs** → **Auth0 Management API** → **Settings**:

Activez les scopes suivants pour votre application:
- `openid` (obligatoire)
- `profile` (récupérer nom, photo)
- `email` (récupérer email)

### 1.5 Personnalisation de l'Expérience Utilisateur

#### Login Page Branding
1. Allez dans **Branding** → **Universal Login**
2. Personnalisez:
   - Logo de l'application
   - Couleurs primaires
   - Langue par défaut (Français)

#### Email Templates
1. Allez dans **Branding** → **Email Templates**
2. Personnalisez les templates:
   - Welcome Email
   - Verification Email
   - Password Reset

### 1.6 Configuration des Règles (Rules/Actions)

#### Ajouter le User ID au Token

1. Allez dans **Auth Pipeline** → **Rules** (ou **Actions** dans les nouvelles versions)
2. Créez une nouvelle règle **Add user_id to token**:

```javascript
function addUserIdToToken(user, context, callback) {
  const namespace = 'https://menumalin.com/';
  context.idToken[namespace + 'user_id'] = user.user_id;
  context.accessToken[namespace + 'user_id'] = user.user_id;
  callback(null, user, context);
}
```

Cette règle permet d'accéder au `user_id` (sub) dans votre application pour filtrer les favoris.

### 1.7 Configuration de la Sécurité

#### Anomaly Detection
1. Allez dans **Security** → **Attack Protection**
2. Activez:
   - **Breached Password Detection:** Bloquer les mots de passe compromis
   - **Brute Force Protection:** Bloquer après 10 tentatives échouées
   - **Suspicious IP Throttling:** Limiter les requêtes par IP

#### Multi-Factor Authentication (MFA)
Pour une sécurité renforcée (optionnel):
1. Allez dans **Security** → **Multi-Factor Auth**
2. Activez **Push Notifications** ou **SMS**
3. Politique: "Toujours" ou "Adaptative" selon vos besoins

---

## 2. Intégration Blazor WebAssembly

### 2.1 Packages NuGet Requis

Les packages sont déjà installés dans votre projet:

```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="9.0.11"/>
```

### 2.2 Configuration appsettings.json

Modifiez `C:\Users\warse\Documents\myCode\csharp\menuMalin\menuMalin\wwwroot\appsettings.json`:

```json
{
  "Auth0": {
    "Authority": "https://menumalin.eu.auth0.com",
    "ClientId": "VOTRE_CLIENT_ID_ICI",
    "ResponseType": "code",
    "Scope": "openid profile email"
  },
  "TheMealDB": {
    "BaseUrl": "https://www.themealdb.com/api/json/v1/1/"
  }
}
```

### 2.3 Configuration appsettings.Development.json

Pour le développement local avec HTTPS:

```json
{
  "Auth0": {
    "Authority": "https://menumalin.eu.auth0.com",
    "ClientId": "VOTRE_CLIENT_ID_ICI",
    "ResponseType": "code",
    "Scope": "openid profile email",
    "RedirectUri": "https://localhost:7123/authentication/login-callback",
    "PostLogoutRedirectUri": "https://localhost:7123/"
  },
  "TheMealDB": {
    "BaseUrl": "https://www.themealdb.com/api/json/v1/1/"
  }
}
```

### 2.4 Modification du Program.cs

Mettez à jour `C:\Users\warse\Documents\myCode\csharp\menuMalin\menuMalin\Program.cs`:

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using menuMalin;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration du HttpClient pour TheMealDB API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["TheMealDB:BaseUrl"]!) });

// Configuration Auth0 OIDC
builder.Services.AddOidcAuthentication(options =>
{
    // Lier la configuration Auth0 depuis appsettings.json
    builder.Configuration.Bind("Auth0", options.ProviderOptions);

    // Optionnel: Configuration supplémentaire
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
});

await builder.Build().RunAsync();
```

### 2.5 Variables d'Environnement (Sécurité)

Pour ne pas exposer le ClientId dans le code source:

#### Créer un fichier .env (ignoré par Git)

Ajoutez à `.gitignore`:
```
appsettings.Development.json
*.env
```

#### Utiliser les User Secrets (Recommandé)

```bash
dotnet user-secrets init --project menuMalin
dotnet user-secrets set "Auth0:ClientId" "VOTRE_CLIENT_ID" --project menuMalin
dotnet user-secrets set "Auth0:Authority" "https://menumalin.eu.auth0.com" --project menuMalin
```

---

## 3. Protection des Routes et Composants

### 3.1 Protection Globale avec App.razor

Votre fichier `App.razor` est déjà configuré correctement:

```razor
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    @if (context.User.Identity?.IsAuthenticated != true)
                    {
                        <RedirectToLogin/>
                    }
                    else
                    {
                        <p role="alert">Vous n'êtes pas autorisé à accéder à cette ressource.</p>
                    }
                </NotAuthorized>
            </AuthorizeRouteView>
            <FocusOnNavigate RouteData="@routeData" Selector="h1"/>
        </Found>
        <NotFound>
            <PageTitle>Non trouvé</PageTitle>
            <LayoutView Layout="@typeof(MainLayout)">
                <p role="alert">Désolé, cette page n'existe pas.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```

### 3.2 Protection des Pages Individuelles

#### Pages Publiques (Recherche)

```razor
@page "/search"
@* Pas d'attribut [Authorize] = accessible sans connexion *@

<PageTitle>Recherche de Recettes</PageTitle>

<h3>Rechercher des Recettes</h3>
@* Contenu accessible à tous *@
```

#### Pages Protégées (Favoris)

```razor
@page "/favorites"
@attribute [Authorize]
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization

<PageTitle>Mes Favoris</PageTitle>

<AuthorizeView>
    <Authorized>
        <h3>Mes Recettes Favorites</h3>
        <p>Connecté en tant que: @context.User.Identity?.Name</p>
        @* Affichage des favoris *@
    </Authorized>
    <NotAuthorized>
        <p>Vous devez être connecté pour voir vos favoris.</p>
        <a href="authentication/login">Se connecter</a>
    </NotAuthorized>
</AuthorizeView>
```

### 3.3 Protection des Composants

#### Bouton "Ajouter aux Favoris" conditionnel

```razor
@inject AuthenticationStateProvider AuthenticationStateProvider

<AuthorizeView>
    <Authorized>
        <button class="btn btn-primary" @onclick="AddToFavorites">
            Ajouter aux Favoris
        </button>
    </Authorized>
    <NotAuthorized>
        <a href="authentication/login" class="btn btn-secondary">
            Connectez-vous pour ajouter aux favoris
        </a>
    </NotAuthorized>
</AuthorizeView>

@code {
    [Parameter] public Recipe Recipe { get; set; }

    private async Task AddToFavorites()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirst(c => c.Type == "sub")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            Recipe.OwnerId = userId;
            // Sauvegarder dans LocalStorage
        }
    }
}
```

### 3.4 Redirection Personnalisée

Créez `C:\Users\warse\Documents\myCode\csharp\menuMalin\menuMalin\Layout\RedirectToLogin.razor`:

```razor
@inject NavigationManager Navigation
@code {
    protected override void OnInitialized()
    {
        Navigation.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(Navigation.Uri)}");
    }
}
```

---

## 4. Gestion des Tokens et du State

### 4.1 Récupération du User ID (sub)

#### Service UserService

Créez `C:\Users\warse\Documents\myCode\csharp\menuMalin\menuMalin\Services\UserService.cs`:

```csharp
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace menuMalin.Services;

public interface IUserService
{
    Task<string?> GetUserIdAsync();
    Task<string?> GetUserEmailAsync();
    Task<string?> GetUserNameAsync();
    Task<bool> IsAuthenticatedAsync();
}

public class UserService : IUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public UserService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string?> GetUserIdAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // Le claim 'sub' contient l'identifiant unique Auth0
        return user.FindFirst(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public async Task<string?> GetUserEmailAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        return user.FindFirst(c => c.Type == "email" || c.Type == ClaimTypes.Email)?.Value;
    }

    public async Task<string?> GetUserNameAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        return user.FindFirst(c => c.Type == "name" || c.Type == ClaimTypes.Name)?.Value;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity?.IsAuthenticated ?? false;
    }
}
```

#### Enregistrement dans Program.cs

```csharp
// Ajouter après AddOidcAuthentication
builder.Services.AddScoped<IUserService, UserService>();
```

### 4.2 Service de Gestion des Favoris

Créez `C:\Users\warse\Documents\myCode\csharp\menuMalin\menuMalin\Services\FavoriteService.cs`:

```csharp
using Blazored.LocalStorage;
using menuMalin.Models;

namespace menuMalin.Services;

public interface IFavoriteService
{
    Task<List<Recipe>> GetUserFavoritesAsync(string userId);
    Task AddFavoriteAsync(Recipe recipe, string userId);
    Task RemoveFavoriteAsync(string recipeId, string userId);
    Task<bool> IsFavoriteAsync(string recipeId, string userId);
    Task UpdateRecipeAsync(Recipe recipe, string userId);
}

public class FavoriteService : IFavoriteService
{
    private readonly ILocalStorageService _localStorage;
    private const string FAVORITES_KEY = "menumalin_favorites";

    public FavoriteService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<List<Recipe>> GetUserFavoritesAsync(string userId)
    {
        var allFavorites = await GetAllFavoritesAsync();
        return allFavorites.Where(r => r.OwnerId == userId).ToList();
    }

    public async Task AddFavoriteAsync(Recipe recipe, string userId)
    {
        recipe.OwnerId = userId;
        var favorites = await GetAllFavoritesAsync();

        // Vérifier si la recette n'existe pas déjà
        if (!favorites.Any(r => r.IdMeal == recipe.IdMeal && r.OwnerId == userId))
        {
            favorites.Add(recipe);
            await SaveAllFavoritesAsync(favorites);
        }
    }

    public async Task RemoveFavoriteAsync(string recipeId, string userId)
    {
        var favorites = await GetAllFavoritesAsync();
        favorites.RemoveAll(r => r.IdMeal == recipeId && r.OwnerId == userId);
        await SaveAllFavoritesAsync(favorites);
    }

    public async Task<bool> IsFavoriteAsync(string recipeId, string userId)
    {
        var favorites = await GetUserFavoritesAsync(userId);
        return favorites.Any(r => r.IdMeal == recipeId);
    }

    public async Task UpdateRecipeAsync(Recipe recipe, string userId)
    {
        var favorites = await GetAllFavoritesAsync();
        var existingRecipe = favorites.FirstOrDefault(r => r.IdMeal == recipe.IdMeal && r.OwnerId == userId);

        if (existingRecipe != null)
        {
            // Mettre à jour les propriétés modifiables
            existingRecipe.StrMeal = recipe.StrMeal;
            existingRecipe.StrInstructions = recipe.StrInstructions;
            existingRecipe.UserNote = recipe.UserNote;

            await SaveAllFavoritesAsync(favorites);
        }
    }

    private async Task<List<Recipe>> GetAllFavoritesAsync()
    {
        return await _localStorage.GetItemAsync<List<Recipe>>(FAVORITES_KEY) ?? new List<Recipe>();
    }

    private async Task SaveAllFavoritesAsync(List<Recipe> favorites)
    {
        await _localStorage.SetItemAsync(FAVORITES_KEY, favorites);
    }
}
```

#### Installation du package Blazored.LocalStorage

```bash
dotnet add package Blazored.LocalStorage --version 4.5.0
```

#### Enregistrement dans Program.cs

```csharp
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
```

### 4.3 Gestion du Token Lifecycle

#### Configuration du Refresh Token (Optionnel)

Dans `Program.cs`:

```csharp
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);

    // Activer le refresh automatique des tokens
    options.UserOptions.RoleClaim = "role";

    // Personnaliser les messages d'erreur
    options.AuthenticationPaths.LogInPath = "authentication/login";
    options.AuthenticationPaths.LogInCallbackPath = "authentication/login-callback";
    options.AuthenticationPaths.LogInFailedPath = "authentication/login-failed";
    options.AuthenticationPaths.LogOutPath = "authentication/logout";
    options.AuthenticationPaths.LogOutCallbackPath = "authentication/logout-callback";
    options.AuthenticationPaths.LogOutFailedPath = "authentication/logout-failed";
    options.AuthenticationPaths.LogOutSucceededPath = "/";
});
```

#### Gestion de l'Expiration du Token

Créez `C:\Users\warse\Documents\myCode\csharp\menuMalin\menuMalin\Services\TokenExpirationService.cs`:

```csharp
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Timers;

namespace menuMalin.Services;

public class TokenExpirationService : IDisposable
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly NavigationManager _navigationManager;
    private System.Timers.Timer? _timer;

    public TokenExpirationService(
        AuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task StartMonitoringAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var expClaim = user.FindFirst("exp")?.Value;
            if (long.TryParse(expClaim, out var exp))
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                var timeUntilExpiration = expirationTime - DateTimeOffset.UtcNow;

                if (timeUntilExpiration.TotalMinutes > 5)
                {
                    // Avertir 5 minutes avant l'expiration
                    var warningTime = timeUntilExpiration - TimeSpan.FromMinutes(5);
                    _timer = new System.Timers.Timer(warningTime.TotalMilliseconds);
                    _timer.Elapsed += OnTokenExpiring;
                    _timer.Start();
                }
            }
        }
    }

    private void OnTokenExpiring(object? sender, ElapsedEventArgs e)
    {
        // Rediriger vers la page de login
        _navigationManager.NavigateTo("authentication/login", forceLoad: true);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
```

### 4.4 Stockage Sécurisé des Données Utilisateur

#### Modèle Recipe avec OwnerId

```csharp
namespace menuMalin.Models;

public class Recipe
{
    public string IdMeal { get; set; } = string.Empty;
    public string StrMeal { get; set; } = string.Empty;
    public string StrCategory { get; set; } = string.Empty;
    public string StrArea { get; set; } = string.Empty;
    public string StrInstructions { get; set; } = string.Empty;
    public string? StrMealThumb { get; set; }
    public string? StrYoutube { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new();
    public string? UserNote { get; set; }

    // Auth0 User ID (claim 'sub')
    public string OwnerId { get; set; } = string.Empty;

    // Métadonnées
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModified { get; set; }
}

public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public string Measure { get; set; } = string.Empty;
    public bool IsMissing { get; set; } = false;
}
```

---

## 5. Checklist de Sécurité OIDC

### 5.1 Configuration Auth0

- [ ] **Tenant créé** dans une région appropriée (EU pour RGPD)
- [ ] **Application SPA** configurée avec le bon type
- [ ] **Callback URLs** correctement définies (HTTPS uniquement en production)
- [ ] **Logout URLs** correctement définies
- [ ] **CORS** configuré pour les domaines autorisés
- [ ] **Response Type** configuré sur `code` (Authorization Code Flow)
- [ ] **Scopes** minimaux requis: `openid`, `profile`, `email`
- [ ] **Token Expiration** configurée (10 heures max recommandé)

### 5.2 Sécurité des Tokens

- [ ] **Jamais stocker les tokens** dans localStorage (géré automatiquement par Blazor WASM)
- [ ] **Utiliser HTTPS** en production (obligatoire pour OIDC)
- [ ] **Valider les tokens côté client** (automatique avec OIDC)
- [ ] **Signature RS256** activée dans Auth0
- [ ] **Claim 'sub'** utilisé comme identifiant unique utilisateur
- [ ] **Expiration des tokens** surveillée
- [ ] **Refresh Token** désactivé (pas nécessaire pour SPA simple)

### 5.3 Protection des Routes

- [ ] **CascadingAuthenticationState** enveloppe l'application
- [ ] **AuthorizeRouteView** utilisé dans le Router
- [ ] **[Authorize]** attribut sur les pages protégées
- [ ] **RedirectToLogin** composant implémenté
- [ ] **Pages publiques** sans attribut [Authorize]
- [ ] **Gestion des erreurs** d'authentification

### 5.4 Données Utilisateur

- [ ] **OwnerId (sub)** associé à chaque recette favorite
- [ ] **Filtrage côté client** des favoris par OwnerId
- [ ] **Pas de données sensibles** dans localStorage
- [ ] **Validation des données** avant sauvegarde
- [ ] **Isolation des données** entre utilisateurs

### 5.5 Configuration Environnement

- [ ] **appsettings.json** ne contient PAS de secrets
- [ ] **ClientId** externalisé (user-secrets en dev)
- [ ] **Variables d'environnement** pour la production
- [ ] **.gitignore** exclut les fichiers sensibles
- [ ] **HTTPS** activé en développement et production

### 5.6 Attaques et Protections

- [ ] **CSRF Protection** (géré par OIDC Authorization Code Flow)
- [ ] **XSS Protection** (pas de innerHTML avec données utilisateur)
- [ ] **Clickjacking** (Auth0 envoie X-Frame-Options)
- [ ] **Brute Force Protection** activée dans Auth0
- [ ] **Breached Password Detection** activée
- [ ] **Suspicious IP Throttling** activée

### 5.7 Conformité et Logs

- [ ] **RGPD**: Politique de confidentialité affichée
- [ ] **Logs Auth0** activés et surveillés
- [ ] **Anomaly Detection** activée
- [ ] **MFA optionnelle** pour comptes sensibles
- [ ] **Audit Trail** des connexions utilisateurs

### 5.8 Tests de Sécurité

- [ ] **Test de connexion/déconnexion** fonctionnel
- [ ] **Test de redirection** après login
- [ ] **Test d'accès non autorisé** aux favoris
- [ ] **Test d'isolation des données** entre utilisateurs
- [ ] **Test d'expiration de token** (forcer expiration)
- [ ] **Test de CORS** depuis domaines non autorisés

---

## 6. Tests et Validation

### 6.1 Tests Unitaires (xUnit)

#### Test du UserService

Créez `C:\Users\warse\Documents\myCode\csharp\menuMalin.Tests\Services\UserServiceTests.cs`:

```csharp
using Xunit;
using Moq;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using menuMalin.Services;

namespace menuMalin.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetUserIdAsync_ReturnsSubClaim()
    {
        // Arrange
        var expectedUserId = "auth0|123456789";
        var claims = new List<Claim>
        {
            new Claim("sub", expectedUserId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(user);

        var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
        mockAuthStateProvider.Setup(x => x.GetAuthenticationStateAsync())
            .ReturnsAsync(authState);

        var userService = new UserService(mockAuthStateProvider.Object);

        // Act
        var result = await userService.GetUserIdAsync();

        // Assert
        Assert.Equal(expectedUserId, result);
    }

    [Fact]
    public async Task IsAuthenticatedAsync_ReturnsTrue_WhenUserAuthenticated()
    {
        // Arrange
        var claims = new List<Claim> { new Claim("sub", "user123") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(user);

        var mockAuthStateProvider = new Mock<AuthenticationStateProvider>();
        mockAuthStateProvider.Setup(x => x.GetAuthenticationStateAsync())
            .ReturnsAsync(authState);

        var userService = new UserService(mockAuthStateProvider.Object);

        // Act
        var result = await userService.IsAuthenticatedAsync();

        // Assert
        Assert.True(result);
    }
}
```

#### Test du FavoriteService

```csharp
using Xunit;
using Moq;
using Blazored.LocalStorage;
using menuMalin.Services;
using menuMalin.Models;

namespace menuMalin.Tests.Services;

public class FavoriteServiceTests
{
    [Fact]
    public async Task AddFavoriteAsync_AddsRecipeWithOwnerId()
    {
        // Arrange
        var mockLocalStorage = new Mock<ILocalStorageService>();
        mockLocalStorage.Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>()))
            .ReturnsAsync(new List<Recipe>());

        var favoriteService = new FavoriteService(mockLocalStorage.Object);
        var recipe = new Recipe { IdMeal = "52772", StrMeal = "Test Recipe" };
        var userId = "auth0|123";

        // Act
        await favoriteService.AddFavoriteAsync(recipe, userId);

        // Assert
        Assert.Equal(userId, recipe.OwnerId);
        mockLocalStorage.Verify(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<List<Recipe>>()), Times.Once);
    }

    [Fact]
    public async Task GetUserFavoritesAsync_FiltersRecipesByOwnerId()
    {
        // Arrange
        var userId1 = "auth0|111";
        var userId2 = "auth0|222";

        var allRecipes = new List<Recipe>
        {
            new Recipe { IdMeal = "1", StrMeal = "Recipe 1", OwnerId = userId1 },
            new Recipe { IdMeal = "2", StrMeal = "Recipe 2", OwnerId = userId2 },
            new Recipe { IdMeal = "3", StrMeal = "Recipe 3", OwnerId = userId1 }
        };

        var mockLocalStorage = new Mock<ILocalStorageService>();
        mockLocalStorage.Setup(x => x.GetItemAsync<List<Recipe>>(It.IsAny<string>()))
            .ReturnsAsync(allRecipes);

        var favoriteService = new FavoriteService(mockLocalStorage.Object);

        // Act
        var result = await favoriteService.GetUserFavoritesAsync(userId1);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.Equal(userId1, r.OwnerId));
    }
}
```

### 6.2 Tests de Composants (bUnit)

```bash
dotnet add package bUnit --version 1.30.3
```

```csharp
using Bunit;
using Xunit;
using Microsoft.AspNetCore.Components.Authorization;
using menuMalin.Layout;

namespace menuMalin.Tests.Components;

public class LoginDisplayTests
{
    [Fact]
    public void LoginDisplay_ShowsLoginLink_WhenNotAuthenticated()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetNotAuthorized();

        // Act
        var cut = ctx.RenderComponent<LoginDisplay>();

        // Assert
        var link = cut.Find("a[href='authentication/login']");
        Assert.NotNull(link);
        Assert.Contains("Log in", link.TextContent);
    }

    [Fact]
    public void LoginDisplay_ShowsLogoutButton_WhenAuthenticated()
    {
        // Arrange
        using var ctx = new TestContext();
        var authContext = ctx.AddTestAuthorization();
        authContext.SetAuthorized("testuser@example.com");

        // Act
        var cut = ctx.RenderComponent<LoginDisplay>();

        // Assert
        var button = cut.Find("button");
        Assert.NotNull(button);
        Assert.Contains("Log out", button.TextContent);
    }
}
```

### 6.3 Tests d'Intégration

#### Test du Flux OIDC complet

1. **Démarrer l'application en local**
2. **Cliquer sur "Log in"**
3. **Vérifier la redirection vers Auth0**
4. **Se connecter avec des credentials de test**
5. **Vérifier le retour à l'application**
6. **Vérifier l'affichage du nom d'utilisateur**
7. **Vérifier l'accès aux favoris**
8. **Cliquer sur "Log out"**
9. **Vérifier la déconnexion**

---

## 7. Troubleshooting

### 7.1 Erreurs Courantes

#### Erreur: "Invalid redirect_uri"

**Cause:** L'URL de callback n'est pas configurée dans Auth0

**Solution:**
1. Vérifiez dans Auth0 Dashboard → Applications → Settings → Allowed Callback URLs
2. Ajoutez exactement: `https://localhost:7123/authentication/login-callback`
3. En production: `https://votredomaine.com/authentication/login-callback`

#### Erreur: "Access denied"

**Cause:** Scopes non autorisés ou problème de configuration

**Solution:**
1. Vérifiez que les scopes dans `appsettings.json` correspondent à ceux autorisés dans Auth0
2. Vérifiez que l'application est activée dans Auth0
3. Vérifiez les logs Auth0 pour plus de détails

#### Erreur: "CORS policy"

**Cause:** Domaine non autorisé dans Auth0

**Solution:**
1. Ajoutez votre domaine dans Auth0 → Applications → Settings → Allowed Web Origins
2. Ajoutez également dans Allowed Origins (CORS)

#### Erreur: "Token expired"

**Cause:** Token expiré et pas de refresh token

**Solution:**
1. L'utilisateur doit se reconnecter
2. Implémentez le TokenExpirationService pour avertir l'utilisateur
3. Optionnel: Activez les refresh tokens dans Auth0

### 7.2 Debugging

#### Activer les Logs Détaillés

Dans `Program.cs`:

```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Components.WebAssembly.Authentication", LogLevel.Trace);
```

#### Inspecter les Claims

Créez une page de debug:

```razor
@page "/debug-auth"
@attribute [Authorize]
@using Microsoft.AspNetCore.Components.Authorization
@inject AuthenticationStateProvider AuthenticationStateProvider

<h3>Debug Authentication</h3>

@if (authState != null)
{
    <h4>User Claims:</h4>
    <ul>
        @foreach (var claim in authState.User.Claims)
        {
            <li><strong>@claim.Type:</strong> @claim.Value</li>
        }
    </ul>

    <h4>Identity:</h4>
    <p>Authenticated: @authState.User.Identity?.IsAuthenticated</p>
    <p>Name: @authState.User.Identity?.Name</p>
}

@code {
    private AuthenticationState? authState;

    protected override async Task OnInitializedAsync()
    {
        authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
    }
}
```

### 7.3 Vérification de la Configuration

#### Script PowerShell de Vérification

```powershell
# Vérifier la configuration Auth0
Write-Host "Vérification de la configuration Auth0..." -ForegroundColor Cyan

$appsettingsPath = "menuMalin\wwwroot\appsettings.json"
$config = Get-Content $appsettingsPath | ConvertFrom-Json

Write-Host "Authority: $($config.Auth0.Authority)" -ForegroundColor Green
Write-Host "ClientId: $($config.Auth0.ClientId)" -ForegroundColor Green
Write-Host "Scopes: $($config.Auth0.Scope)" -ForegroundColor Green

# Vérifier que HTTPS est utilisé
if ($config.Auth0.Authority -notlike "https://*") {
    Write-Host "ATTENTION: Authority doit utiliser HTTPS!" -ForegroundColor Red
}

# Vérifier que le ClientId n'est pas vide
if ([string]::IsNullOrEmpty($config.Auth0.ClientId) -or $config.Auth0.ClientId -eq "VOTRE_CLIENT_ID_ICI") {
    Write-Host "ATTENTION: ClientId non configuré!" -ForegroundColor Red
}

Write-Host "Vérification terminée." -ForegroundColor Cyan
```

---

## Annexes

### A. Commandes Utiles

```bash
# Restaurer les packages
dotnet restore

# Compiler le projet
dotnet build

# Exécuter en mode développement
dotnet run --project menuMalin

# Exécuter les tests
dotnet test

# Créer un build de production
dotnet publish -c Release

# Initialiser les user secrets
dotnet user-secrets init --project menuMalin

# Ajouter un secret
dotnet user-secrets set "Auth0:ClientId" "VOTRE_CLIENT_ID" --project menuMalin

# Lister les secrets
dotnet user-secrets list --project menuMalin
```

### B. Ressources

- [Documentation Auth0](https://auth0.com/docs)
- [Blazor WASM Authentication](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/)
- [OIDC Specification](https://openid.net/specs/openid-connect-core-1_0.html)
- [Auth0 Community](https://community.auth0.com/)
- [TheMealDB API](https://www.themealdb.com/api.php)

### C. Template appsettings.json Final

```json
{
  "Auth0": {
    "Authority": "https://menumalin.eu.auth0.com",
    "ClientId": "VOTRE_CLIENT_ID_ICI",
    "ResponseType": "code",
    "Scope": "openid profile email"
  },
  "TheMealDB": {
    "BaseUrl": "https://www.themealdb.com/api/json/v1/1/"
  },
  "LocalStorage": {
    "FavoritesKey": "menumalin_favorites",
    "ShoppingListKey": "menumalin_shopping_list"
  }
}
```

---

## Conclusion

Ce guide couvre l'ensemble du processus d'intégration Auth0 avec OIDC dans votre application Blazor WebAssembly MenuMalin. En suivant ces étapes, vous aurez une authentification sécurisée, conforme aux standards OIDC, et respectant les meilleures pratiques de sécurité.

**Prochaines étapes:**
1. Créer un compte Auth0 et configurer votre tenant
2. Mettre à jour les fichiers de configuration avec vos valeurs Auth0
3. Tester le flux de connexion/déconnexion
4. Implémenter les services UserService et FavoriteService
5. Créer les pages de favoris et de recherche
6. Écrire les tests unitaires et d'intégration
7. Déployer sur Coolify avec Docker

**Points d'attention:**
- Toujours utiliser HTTPS en production
- Ne jamais commiter de secrets dans Git
- Tester l'isolation des données entre utilisateurs
- Surveiller les logs Auth0 pour détecter les anomalies
- Mettre en place une politique de rétention des données conforme au RGPD

Bonne implémentation!
