# 🏗️ Architecture - menuMalin

**Version:** 1.0
**Date:** 24 février 2026

---

## 📋 Vue d'ensemble

menuMalin est une application web complète suivant les principes de **Clean Architecture** avec une séparation claire entre frontend (Blazor WebAssembly) et backend (ASP.NET Core Web API).

```
┌─────────────────────────────────────────────────────────────┐
│                   CLIENT (BLAZOR WASM)                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Pages (Index, Search, MyRecipes, Contact)           │   │
│  │ Components (RecipeCard, RecipeGrid, RecipeModal)    │   │
│  │ Services (RecipeService, FavoriteService)           │   │
│  │ Auth (Auth0 + OIDC)                                 │   │
│  └──────────────────────────────────────────────────────┘   │
│                    ↓ HTTP Calls ↓                           │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                  SERVER (ASP.NET CORE)                      │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ Controllers (RecipesController, FavoritesController)│   │
│  │ Services (RecipeService, FavoriteService)           │   │
│  │ Repositories (RecipeRepository, FavoriteRepository) │   │
│  │ Data Access (EF Core + MySQL)                       │   │
│  └──────────────────────────────────────────────────────┘   │
│                    ↓ External APIs ↓                        │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│              EXTERNAL SERVICES & DATA                       │
│  ┌──────────────────────────────────────────────────────┐   │
│  │ TheMealDB API (Recipe Data)                         │   │
│  │ Auth0 (Authentication)                              │   │
│  │ MySQL Database (Persistent Storage)                 │   │
│  │ LocalStorage (Client-side State)                    │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

---

## 🏛️ Patterns & Principes

### 1. Clean Architecture
- **Separation of Concerns**: Chaque couche a une responsabilité unique
- **Dependency Injection**: Services injectés via constructeurs
- **Repository Pattern**: Abstraction de la couche données
- **Service Pattern**: Logique métier centralisée

### 2. Couches

#### Couche Présentation (Frontend)
- **Pages Razor**: Index, Search, MyRecipes, Contact
- **Composants**: RecipeCard, RecipeGrid, RecipeModal, ContactForm
- **Services Client**: RecipeService, FavoriteService, ContactService, HttpApiService
- **Authentification**: Blazor OIDC avec Auth0

#### Couche Application (Backend)
- **Controllers**: RecipesController, FavoritesController, ContactController, AuthController
- **Services**: RecipeService, FavoriteService, ContactService
- **DTOs**: RecipeResponse, CategoryResponse, AreaResponse
- **Interfaces**: IRecipeService, IFavoriteService, IContactService

#### Couche Data Access
- **DbContext**: ApplicationDbContext (EF Core)
- **Repositories**: RecipeRepository, FavoriteRepository
- **Entities**: Recipe, User, Favorite, ContactMessage
- **Migrations**: EF Core Migrations pour MySQL

---

## 🔄 Flux des Données

### Scénario 1: Recherche de Recette

```
User Input (Search.razor)
    ↓
RecipeService.SearchRecipesAsync()
    ↓
HttpApiService.GetAsync()
    ↓
HTTP GET /api/recipes/search?query=...
    ↓
RecipesController.SearchRecipesAsync()
    ↓
RecipeService.SearchRecipesAsync() [Backend]
    ↓
TheMealDB API Call
    ↓
Mapper Results to Recipe DTO
    ↓
JSON Response
    ↓
Parse + Display RecipeGrid
```

### Scénario 2: Ajouter un Favori

```
User Click "❤️" (RecipeCard.razor)
    ↓
FavoriteService.AddFavoriteAsync()
    ↓
Save to LocalStorage (Client-side)
    ↓
Recipe Stored
    ↓
Update Heart Icon State
```

### Scénario 3: Envoyer un Message

```
User Submit (Contact.razor)
    ↓
ContactService.SendMessageAsync()
    ↓
HttpApiService.PostAsync()
    ↓
HTTP POST /api/contact
    ↓
ContactController.SendMessage()
    ↓
ContactService.SaveMessage() [Backend]
    ↓
Save to MySQL (ContactMessage table)
    ↓
Return Success/Failure
    ↓
Display Toast Message
```

---

## 📁 Structure des Fichiers

### Backend (menuMalin.Server)
```
Controllers/
  ├── RecipesController.cs
  ├── FavoritesController.cs
  ├── ContactController.cs
  └── AuthController.cs

Services/
  ├── IRecipeService.cs
  ├── RecipeService.cs
  ├── IFavoriteService.cs
  ├── FavoriteService.cs
  ├── IContactService.cs
  ├── ContactService.cs
  └── TheMealDBService.cs

Repositories/
  ├── IRecipeRepository.cs
  ├── RecipeRepository.cs
  ├── IFavoriteRepository.cs
  └── FavoriteRepository.cs

Models/
  ├── Entities/
  │   ├── Recipe.cs
  │   ├── User.cs
  │   ├── Favorite.cs
  │   └── ContactMessage.cs
  └── DTOs/
      ├── RecipeResponse.cs
      ├── CategoryResponse.cs
      └── AreaResponse.cs

Data/
  └── ApplicationDbContext.cs
```

### Frontend (menuMalin)
```
Pages/
  ├── Index.razor
  ├── Search.razor
  ├── MyRecipes.razor
  ├── Contact.razor
  ├── Authentication.razor
  ├── Favorites.razor
  ├── Shopping-list.razor
  └── RecipeDetails.razor

Components/
  ├── Recipe/
  │   ├── RecipeCard.razor
  │   ├── RecipeGrid.razor
  │   ├── RecipeModal.razor
  │   └── SearchBar.razor
  └── Contact/
      └── ContactForm.razor

Services/
  ├── IRecipeService.cs
  ├── RecipeService.cs
  ├── IFavoriteService.cs
  ├── FavoriteService.cs
  ├── IContactService.cs
  ├── ContactService.cs
  ├── IHttpApiService.cs
  ├── HttpApiService.cs
  ├── IThemeService.cs
  └── ThemeService.cs

Layouts/
  └── MainLayout.razor
```

---

## 🔐 Authentification & Autorisation

### Auth0 Integration
- **Provider**: Auth0 OIDC
- **Client Type**: Blazor WebAssembly (SPA)
- **Scopes**: `openid profile email`
- **Callback URLs**: `localhost:7777/authentication/login-callback`

### Protected Routes
```csharp
[Authorize]
public partial class Search { }
```

### Role-Based Access (Futur)
- `Admin`: Gestion complète
- `User`: Accès complet aux features
- `Anonymous`: Accès lectures uniquement

---

## 🗄️ Base de Données

### Schema MySQL
```sql
Users
  ├── UserId (UUID)
  ├── Auth0Id
  ├── Email
  └── CreatedAt

Recipes
  ├── RecipeId (UUID)
  ├── MealDBId
  ├── Title
  ├── Category
  ├── Area
  └── CreatedAt

Favorites
  ├── FavoriteId (UUID)
  ├── UserId (FK)
  ├── RecipeId (FK)
  └── CreatedAt

ContactMessages
  ├── Id (UUID)
  ├── Email
  ├── Subject
  ├── Message
  └── CreatedAt
```

### Relations
- User 1:N Favorite
- User 1:N ContactMessage
- Recipe 1:N Favorite

---

## 🔌 Dépendances Externes

### TheMealDB API
- **Base URL**: `https://www.themealdb.com/api/json/v1/1/`
- **Endpoints**:
  - `random.php` - Recette aléatoire
  - `search.php?s=` - Recherche par nom
  - `list.php?c=list` - Catégories
  - `filter.php?c=` - Filtre par catégorie

### Auth0
- **Domain**: `{domain}.auth0.com`
- **Client ID**: Configuré dans appsettings.json
- **Redirect URI**: `https://localhost:7777/authentication/login-callback`

### LocalStorage (Client)
- **Theme**: Dark/Light mode preference
- **User Profile**: Nom et email (stockés côté client)
- **Favorites**: Liste des IDs (mirroir du serveur)

---

## ⚡ Performance Considerations

1. **Caching**: Favoris cachés en LocalStorage
2. **Lazy Loading**: Images lazy-loaded
3. **HTTP Timeout**: 10 secondes
4. **Pagination**: RecipeGrid 6 items/page
5. **Compression**: GZIP enabled

---

## 🔄 Deployment Architecture

### Local Development
```
http://localhost:5266  (Backend)
https://localhost:7777 (Frontend)
localhost:3306         (MySQL)
```

### Production (Futur)
```
Azure App Service (Frontend + Backend)
Azure Database for MySQL
Auth0 Production Realm
CDN pour assets statiques
```

---

## 🚀 Future Improvements

1. **API Gateway**: Centralize routing
2. **Caching Layer**: Redis/Memcached
3. **Message Queue**: Background jobs
4. **Microservices**: Separated services
5. **GraphQL**: Alternative to REST
6. **WebSockets**: Real-time features
7. **Machine Learning**: Recipe recommendations

---

*Last Updated: 2026-02-24*
