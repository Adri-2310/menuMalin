# Architecture Technique - MenuMalin

## Vue d'ensemble

MenuMalin est construit sur une architecture **Blazor WebAssembly** moderne avec une séparation claire des responsabilités et une approche orientée composants.

## 🏗️ Modèle d'Hébergement

### Blazor WebAssembly (Standalone)

**Type**: Client-Side Blazor
**Exécution**: 100% dans le navigateur via WebAssembly
**Framework**: .NET 9.0

#### Caractéristiques

```
┌─────────────────────────────────────┐
│         Navigateur Client           │
│  ┌─────────────────────────────┐   │
│  │   WebAssembly Runtime       │   │
│  │   (.NET 9.0 WASM)          │   │
│  │                             │   │
│  │   ┌─────────────────────┐  │   │
│  │   │  MenuMalin App      │  │   │
│  │   │  (DLL assemblies)   │  │   │
│  │   └─────────────────────┘  │   │
│  └─────────────────────────────┘   │
│              ↕                      │
│    ┌──────────────────────┐        │
│    │   LocalStorage        │        │
│    └──────────────────────┘        │
└─────────────────────────────────────┘
              ↕
    ┌──────────────────────┐
    │   TheMealDB API      │
    │   (HTTPS externe)    │
    └──────────────────────┘
```

**Avantages pour le projet**:
- ✅ Pas de serveur backend nécessaire (coût réduit)
- ✅ Fonctionnement offline après chargement initial
- ✅ LocalStorage natif pour favoris/listes
- ✅ Déploiement sur hosting statique (GitHub Pages, Netlify, etc.)
- ✅ Pas de latence serveur pour interactions UI

**Inconvénients**:
- ⚠️ Temps de chargement initial plus long (~2-3s)
- ⚠️ Téléchargement des DLL .NET (~1.5MB gzippé)
- ⚠️ Pas de server-side rendering (SEO limité)

---

## 📐 Architecture en Couches

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Pages/     │  │   Layout/    │  │  Components/ │      │
│  │ - Index      │  │ - MainLayout │  │ - Recipe     │      │
│  │ - Favorites  │  │              │  │ - Search     │      │
│  │ - Shopping   │  │              │  │ - Shopping   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                    SERVICE LAYER                             │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Business Services                                     │   │
│  │ - RecipeService      : Logique métier recettes       │   │
│  │ - FavoriteService    : Gestion favoris               │   │
│  │ - ShoppingListService: Gestion liste courses         │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Infrastructure Services                               │   │
│  │ - MealDbApiService   : Communication API              │   │
│  │ - LocalStorageService: Persistance locale             │   │
│  │ - AppStateService    : Gestion état global            │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                    DATA LAYER                                │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Models/DTOs                                           │   │
│  │ - Recipe             : Modèle recette                 │   │
│  │ - RecipeResponse     : DTO API                        │   │
│  │ - ShoppingList       : Liste courses                  │   │
│  │ - UserPreferences    : Préférences utilisateur        │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↕
┌─────────────────────────────────────────────────────────────┐
│                 EXTERNAL RESOURCES                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  TheMealDB   │  │ LocalStorage │  │   Auth0      │      │
│  │     API      │  │   Browser    │  │    OIDC      │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

---

## 🧩 Architecture des Composants

### Hiérarchie des Composants

```
App.razor (Root)
├── Router
    ├── MainLayout
    │   ├── NavBar
    │   │   ├── Logo
    │   │   ├── NavMenu
    │   │   └── UserProfile (AuthorizeView)
    │   └── @Body (Page Content)
    │       ├── Index (/)
    │       │   ├── WelcomeCard (Authenticated)
    │       │   │   ├── QuickActions
    │       │   │   └── InspirationsSection
    │       │   └── HeroSection (Anonymous)
    │       │       ├── FeatureBoxes
    │       │       └── CTAButton
    │       ├── SearchResults (/search) [À implémenter]
    │       │   ├── SearchBar
    │       │   ├── SearchFilters
    │       │   └── RecipeList
    │       │       └── RecipeCard (multiple)
    │       ├── RecipeDetails (/recipe/:id) [À implémenter]
    │       │   ├── RecipeHeader
    │       │   ├── YouTubePlayer
    │       │   ├── IngredientsList
    │       │   ├── Instructions
    │       │   └── FavoriteButton
    │       ├── Favorites (/favorites) [À implémenter]
    │       │   └── FavoritesList
    │       │       └── RecipeCard (multiple)
    │       └── ShoppingList (/shopping-list) [À implémenter]
    │           ├── ShoppingListHeader
    │           └── IngredientCheckboxList
    └── NotFound (404)
```

### Composants Réutilisables Planifiés

#### 1. **RecipeCard.razor**
```razor
@* Affichage carte recette *@
<div class="recipe-card" @onclick="NavigateToDetails">
    <img src="@Recipe.StrMealThumb" alt="@Recipe.StrMeal" />
    <h3>@Recipe.StrMeal</h3>
    <span class="badge">@Recipe.StrCategory</span>
    <FavoriteButton RecipeId="@Recipe.IdMeal" />
</div>
```

#### 2. **SearchBar.razor**
```razor
@* Barre de recherche avec debounce *@
<input type="search"
       @bind="SearchTerm"
       @bind:event="oninput"
       @bind:after="OnSearchChanged"
       placeholder="Rechercher une recette..." />
```

#### 3. **LoadingSpinner.razor**
```razor
@* Spinner de chargement global *@
@if (IsLoading)
{
    <div class="loading-overlay">
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Chargement...</span>
        </div>
    </div>
}
```

---

## 🔄 Flux de Données

### Pattern: Unidirectional Data Flow

```
┌──────────────────────────────────────────────────────────┐
│  1. User Action (Click, Input)                           │
└──────────────────────────────────────────────────────────┘
                        ↓
┌──────────────────────────────────────────────────────────┐
│  2. Component Event Handler                               │
│     - SearchBar.OnSearchChanged()                         │
│     - RecipeCard.OnFavoriteClicked()                      │
└──────────────────────────────────────────────────────────┘
                        ↓
┌──────────────────────────────────────────────────────────┐
│  3. Service Layer Call                                    │
│     - await RecipeService.SearchByNameAsync(term)         │
│     - await FavoriteService.AddFavoriteAsync(recipe)      │
└──────────────────────────────────────────────────────────┘
                        ↓
┌──────────────────────────────────────────────────────────┐
│  4. External Call (API ou Storage)                        │
│     - HTTP GET to TheMealDB                               │
│     - LocalStorage.SetItemAsync()                         │
└──────────────────────────────────────────────────────────┘
                        ↓
┌──────────────────────────────────────────────────────────┐
│  5. State Update                                          │
│     - AppStateService.SetSearchResults(recipes)           │
│     - Component.StateHasChanged()                         │
└──────────────────────────────────────────────────────────┘
                        ↓
┌──────────────────────────────────────────────────────────┐
│  6. UI Re-render                                          │
│     - Blazor reconciliation                               │
│     - DOM update                                          │
└──────────────────────────────────────────────────────────┘
```

---

## 🔐 Architecture d'Authentification

### OIDC Flow (Auth0)

```
┌────────────────┐                           ┌────────────────┐
│   MenuMalin    │                           │     Auth0      │
│   (Client)     │                           │   (Provider)   │
└────────────────┘                           └────────────────┘
        │                                             │
        │  1. Clic "Se connecter"                    │
        │────────────────────────────────────────────>
        │                                             │
        │  2. Redirect → Auth0 login page            │
        │<────────────────────────────────────────────
        │                                             │
        │  3. User entre credentials                 │
        │────────────────────────────────────────────>
        │                                             │
        │  4. Auth0 valide & génère code             │
        │<────────────────────────────────────────────
        │                                             │
        │  5. Redirect vers app + authorization code │
        │<────────────────────────────────────────────
        │                                             │
        │  6. Échange code contre tokens (PKCE)      │
        │────────────────────────────────────────────>
        │                                             │
        │  7. Retourne access_token + id_token       │
        │<────────────────────────────────────────────
        │                                             │
        │  8. Stocke tokens (session storage)        │
        │                                             │
        │  9. User authentifié                       │
        │                                             │
```

### Composants d'Authentification

1. **App.razor**: `<CascadingAuthenticationState>`
2. **MainLayout.razor**: `<AuthorizeView>`
3. **Pages protégées**: `@attribute [Authorize]`
4. **RedirectToLogin.razor**: Redirect vers Auth0

---

## 💾 Architecture de Persistance

### LocalStorage Strategy

```
┌─────────────────────────────────────────────────────────────┐
│                  Browser LocalStorage                        │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Key: "menumalin_favorites"                          │   │
│  │  Value: [                                            │   │
│  │    {                                                 │   │
│  │      "idMeal": "52772",                              │   │
│  │      "strMeal": "Teriyaki Chicken",                  │   │
│  │      "modifiedIngredients": [...],                   │   │
│  │      "userNotes": "Ajouter plus de sauce"            │   │
│  │    },                                                │   │
│  │    ...                                               │   │
│  │  ]                                                   │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Key: "menumalin_shopping_list"                      │   │
│  │  Value: {                                            │   │
│  │    "items": [                                        │   │
│  │      {                                               │   │
│  │        "ingredient": "Poulet",                       │   │
│  │        "quantity": "500g",                           │   │
│  │        "checked": false,                             │   │
│  │        "recipeId": "52772"                           │   │
│  │      },                                              │   │
│  │      ...                                             │   │
│  │    ],                                                │   │
│  │    "lastUpdated": "2026-02-09T10:30:00Z"             │   │
│  │  }                                                   │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Key: "menumalin_user_preferences"                   │   │
│  │  Value: {                                            │   │
│  │    "theme": "light",                                 │   │
│  │    "language": "fr"                                  │   │
│  │  }                                                   │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

### Limite et Gestion

- **Quota**: 5-10MB selon navigateur
- **Stratégie**: Pagination des favoris (max 50 recettes)
- **Nettoyage**: Suppression automatique des items non utilisés > 90 jours
- **Backup**: Export JSON pour sauvegarde manuelle

---

## 🌐 Architecture PWA

### Service Worker Strategy

```
Network Request
      ↓
┌─────────────┐
│   Service   │
│   Worker    │
└─────────────┘
      ↓
 Cache First? ←─── Static assets (CSS, JS, images)
      │
      ├─ YES → Return from Cache
      │
      └─ NO → Network First
              ↓
         API Calls (TheMealDB)
              ↓
         Cache Response (TTL: 1h)
              ↓
         Return to Client
```

### Manifest Configuration

```json
{
  "name": "MenuMalin",
  "short_name": "MenuMalin",
  "start_url": "/",
  "display": "standalone",
  "theme_color": "#2d6a4f",
  "background_color": "#ffffff",
  "icons": [
    {
      "src": "icon-192.png",
      "sizes": "192x192",
      "type": "image/png"
    },
    {
      "src": "icon-512.png",
      "sizes": "512x512",
      "type": "image/png"
    }
  ]
}
```

---

## 🎨 Architecture CSS

### Stratégie de Styling

```
┌─────────────────────────────────────────────────────────────┐
│  Layer 1: Bootstrap 5.3 (CDN)                                │
│  - Grid system, utilities, components                        │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│  Layer 2: Global Styles (app.css)                            │
│  - CSS variables, typography, form validation                │
└─────────────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────────────┐
│  Layer 3: Component Scoped CSS (.razor.css)                  │
│  - MainLayout.razor.css : Navigation, layout                 │
│  - Index.razor.css : Hero, feature boxes, animations         │
│  - RecipeCard.razor.css : Card styling, hover effects        │
└─────────────────────────────────────────────────────────────┘
```

### CSS Custom Properties

```css
:root {
  /* Brand Colors */
  --mm-green: #2d6a4f;
  --mm-green-dark: #1b4332;
  --mm-green-light: #40916c;

  /* Semantic Colors */
  --mm-success: #52b788;
  --mm-danger: #d62828;
  --mm-info: #4361ee;

  /* Spacing */
  --mm-spacing-xs: 0.5rem;
  --mm-spacing-sm: 1rem;
  --mm-spacing-md: 1.5rem;
  --mm-spacing-lg: 2rem;
  --mm-spacing-xl: 3rem;

  /* Shadows */
  --mm-shadow-sm: 0 2px 4px rgba(0,0,0,0.1);
  --mm-shadow-md: 0 4px 8px rgba(0,0,0,0.12);
  --mm-shadow-lg: 0 8px 16px rgba(0,0,0,0.15);
}
```

---

## 📦 Dependency Injection

### Configuration dans Program.cs

```csharp
// HttpClients
builder.Services.AddHttpClient("MealDbAPI", client => {
    client.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
});

// Business Services (Scoped)
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();

// Infrastructure Services
builder.Services.AddScoped<IMealDbApiService, MealDbApiService>();
builder.Services.AddBlazoredLocalStorage(); // Scoped

// State Management (Singleton pour état global)
builder.Services.AddSingleton<IAppStateService, AppStateService>();

// Authentication
builder.Services.AddOidcAuthentication(options => {
    builder.Configuration.Bind("Authentication:Oidc", options.ProviderOptions);
});
```

---

## 🧪 Architecture de Tests

### Pyramide de Tests

```
                  ┌──────┐
                  │  E2E │  (5%)
                  │Tests │  Playwright
                  └──────┘
                ┌──────────┐
                │Integration│  (15%)
                │   Tests   │  WebApplicationFactory
                └──────────┘
          ┌──────────────────┐
          │  Component Tests  │  (30%)
          │      (bUnit)      │
          └──────────────────┘
    ┌────────────────────────────┐
    │      Unit Tests (xUnit)     │  (50%)
    │   Services, Models, Utils   │
    └────────────────────────────┘
```

### Organisation des Tests

```
menuMalin.Tests/
├── Unit/
│   ├── Services/
│   │   ├── RecipeServiceTests.cs
│   │   ├── FavoriteServiceTests.cs
│   │   └── MealDbApiServiceTests.cs
│   └── Models/
│       └── RecipeTests.cs
├── Integration/
│   └── Api/
│       └── MealDbApiIntegrationTests.cs
├── Component/
│   └── RecipeCardTests.cs
└── E2E/
    └── UserJourneyTests.cs
```

---

## 🚀 Architecture de Déploiement

### Options de Déploiement

#### Option 1: Hosting Statique (Recommandé)
```
GitHub Pages / Netlify / Vercel
├── index.html
├── _framework/ (Blazor WASM runtime)
├── css/
├── js/
└── images/
```

#### Option 2: Coolify (Self-hosted)
```
Docker Container
└── nginx
    ├── Static files
    └── nginx.conf (SPA routing)
```

### Configuration nginx pour SPA

```nginx
server {
    listen 80;
    server_name menumalin.example.com;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location ~ \.css$ {
        add_header Content-Type text/css;
    }

    location ~ \.js$ {
        add_header Content-Type application/javascript;
    }

    gzip on;
    gzip_types text/css application/javascript application/json;
}
```

---

## 📊 Diagramme de Séquence: Ajout aux Favoris

```
User         RecipeCard     FavoriteService   LocalStorage   AppState
 │               │                │                │            │
 │ Click ❤️      │                │                │            │
 │──────────────>│                │                │            │
 │               │ AddFavorite()  │                │            │
 │               │───────────────>│                │            │
 │               │                │ GetItem()      │            │
 │               │                │───────────────>│            │
 │               │                │<───────────────│            │
 │               │                │  (favorites)   │            │
 │               │                │                │            │
 │               │                │ SetItem()      │            │
 │               │                │───────────────>│            │
 │               │                │<───────────────│            │
 │               │                │   (success)    │            │
 │               │                │                │            │
 │               │                │ NotifyChanged()│            │
 │               │                │────────────────────────────>│
 │               │                │                │  OnChange  │
 │               │<───────────────│                │            │
 │               │   (success)    │                │            │
 │               │                │                │            │
 │ StateHasChanged()              │                │            │
 │<──────────────│                │                │            │
 │ UI Updated    │                │                │            │
```

---

Cette architecture est conçue pour être **scalable**, **maintenable** et **performante** tout en restant simple à comprendre pour un projet académique.
