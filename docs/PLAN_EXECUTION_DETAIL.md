# 📋 PLAN D'EXÉCUTION DÉTAILLÉ - RecipeHub

**Version**: 1.0
**Date de création**: 08-02-2026
**Deadline**: 8 mars 2026
**Durée estimée**: ~95 heures
**Structure**: 4 phases × 19 sprints

---

## 📑 Table des matières

1. [Vue d'ensemble](#1-vue-densemble)
2. [Structure générale](#2-structure-générale)
3. [Phase 1: Backend Setup](#3-phase-1-backend-setup-sprints-1-5)
4. [Phase 2: Frontend Blazor](#4-phase-2-frontend-blazor-sprints-6-10)
5. [Phase 3: Tests](#5-phase-3-tests-sprints-11-15)
6. [Phase 4: Finalisation](#6-phase-4-finalisation-sprints-16-19)
7. [Checklist par milestone](#7-checklist-par-milestone)
8. [Points clés à retenir](#8-points-clés-à-retenir)

---

# 1. VUE D'ENSEMBLE

## 1.1 Qu'est-ce que vous devez construire

Une application web complète avec:
- **Frontend**: Interface Blazor WebAssembly (C#)
- **Backend**: API REST ASP.NET Core (C#)
- **Base de données**: MySQL pour les données
- **Authentification**: Auth0 pour les utilisateurs
- **Source de données**: TheMealDB API pour les recettes

## 1.2 Utilisateurs finaux

- **Anonyme**: Voir 6 recettes aléatoires
- **Connecté**: Gérer ses favoris, rechercher, contacter

## 1.3 Avant de commencer

**Vérifier que vous avez:**
- [ ] .NET 9 SDK installé
- [ ] Git installé
- [ ] Compte Auth0 créé
- [ ] MySQL installé et démarré
- [ ] Un éditeur (Visual Studio Code ou Visual Studio)
- [ ] Tous les guides de setup lus

---

# 2. STRUCTURE GÉNÉRALE

## 2.1 Les 4 phases du projet

```
Phase 1: Backend Setup (Sprints 1-5)
    ↓ (Après: API fonctionnelle)
Phase 2: Frontend Blazor (Sprints 6-10)
    ↓ (Après: UI complète)
Phase 3: Tests (Sprints 11-15)
    ↓ (Après: Couverture 75%+)
Phase 4: Finalisation (Sprints 16-19)
    ↓ (Après: Ready to submit)
```

## 2.2 Durée estimée par phase

| Phase | Sprints | Heures | Jours |
|-------|---------|--------|-------|
| 1 | 1-5 | 22h | ~5 jours |
| 2 | 6-10 | 18h | ~4 jours |
| 3 | 11-15 | 23h | ~5 jours |
| 4 | 16-19 | 20h | ~4 jours |
| **Total** | **1-19** | **~95h** | **~18 jours** |

---

# 3. PHASE 1: BACKEND SETUP (Sprints 1-5)

**Objectif**: Créer une API REST fonctionnelle avec authentification et accès BD

## Sprint 1: Créer la structure du projet

### Étape 1.1: Créer le dossier racine
1. Ouvrir un terminal
2. Naviguer vers votre dossier de projets
3. Créer un dossier `RecipeHub`
4. Entrer dans ce dossier

### Étape 1.2: Initialiser git
1. Initialiser un repository git
2. Créer un fichier `.gitignore` pour C# (exclure `bin/`, `obj/`, `.vs/`, `appsettings.json` secrets)
3. Faire un commit initial "Initial commit"

### Étape 1.3: Créer les projets .NET
1. **Créer project Client** (Blazor WebAssembly):
   - Nom: `RecipesApp.Client`
   - Type: Blazor WebAssembly App

2. **Créer project Server** (ASP.NET Core):
   - Nom: `RecipesApp.Server`
   - Type: ASP.NET Core Web API

3. **Créer project Shared** (Librairie):
   - Nom: `RecipesApp.Shared`
   - Type: Class Library

4. **Créer project Tests** (xUnit):
   - Nom: `RecipesApp.Tests`
   - Type: xUnit Test Project

5. **Créer project Tests Client** (BUnit):
   - Nom: `RecipesApp.Client.Tests`
   - Type: xUnit Test Project

### Étape 1.4: Vérifier la structure
```
RecipeHub/
├── RecipesApp.Client/
├── RecipesApp.Server/
├── RecipesApp.Shared/
├── RecipesApp.Tests/
├── RecipesApp.Client.Tests/
├── .git/
├── .gitignore
└── solution file
```

### Étape 1.5: Ajouter dépendances NuGet Backend

Pour le project `RecipesApp.Server`, ajouter:
- Pomelo.EntityFrameworkCore.MySql (pour MySQL)
- Auth0.ManagementApi (pour Auth0)
- Microsoft.AspNetCore.Authentication.OpenIdConnect
- Polly (pour retry logic)
- Serilog.AspNetCore (pour logging)
- FluentValidation (pour validation)

### Étape 1.6: Ajouter dépendances NuGet Frontend

Pour le project `RecipesApp.Client`, ajouter:
- Auth0.OidcClient.Blazor (pour Auth0 client)

### Étape 1.7: Ajouter dépendances NuGet Tests

Pour `RecipesApp.Tests`:
- xunit
- Moq
- FluentAssertions
- Testcontainers.MySql

Pour `RecipesApp.Client.Tests`:
- bunit
- xunit
- Moq

### Étape 1.8: Valider que tout compile
1. Ouvrir le terminal
2. Exécuter `dotnet build`
3. Vérifier qu'il n'y a pas d'erreurs

### Étape 1.9: Commit git
```
Commit message: "Sprint 1: Project structure setup"
Inclure: tous les projets créés, dépendances ajoutées
```

**Checklist Sprint 1:**
- [ ] 5 projets créés
- [ ] Tous les projets compilent
- [ ] Dépendances ajoutées
- [ ] Git commit fait
- [ ] `.gitignore` configuré

---

## Sprint 2: Database & EF Core Setup

### Étape 2.1: Créer la base de données MySQL
1. Ouvrir MySQL CLI (ou Workbench)
2. Créer une base de données nommée `recipes_db`
3. Créer un utilisateur `recipes_user` avec password
4. Assigner toutes les permissions sur `recipes_db` à cet utilisateur
5. Tester la connexion avec le nouvel utilisateur

### Étape 2.2: Créer le DbContext

Dans le project `RecipesApp.Server`:
1. Créer un dossier `Data`
2. Dans ce dossier, créer une classe `ApplicationDbContext`
3. Cette classe doit hériter de `DbContext`
4. Elle doit avoir des propriétés `DbSet` pour:
   - Users (table utilisateurs)
   - Favorites (table favoris)
   - ContactMessages (table messages contact)

### Étape 2.3: Créer les Entities (Models)

**🎯 APPROCHE HYBRID** : Le profil utilisateur (Name, Picture, Tokens) est stocké dans **localStorage** côté client, pas en BD!

Créer un dossier `Models/Entities` contenant:

**User.cs** (SIMPLIFIÉ):
- UserId (string UUID, PK)
- Auth0Id (string, unique) - Identifiant Auth0
- Email (string, unique) - Pour référence/recovery
- CreatedAt (DateTime) - Date de création du compte
- Relation: List<Favorite> Favorites
- Relation: List<ContactMessage> ContactMessages

**Remarque**: FirstName, LastName, Picture, ThemePreference → localStorage (client)
**Avantage**: Table Users ultra-légère, rapide, focus sur relations critiques

**Favorite.cs**:
- FavoriteId (string UUID, PK)
- UserId (string UUID, FK vers User) - Non nullable
- RecipeId (string UUID, FK vers Recipe)
- CreatedAt (DateTime)
- Relation: User User (Many-to-One)
- Relation: Recipe Recipe (Many-to-One)
- Contrainte: UNIQUE(UserId, RecipeId) - Pas de doublons

**ContactMessage.cs**:
- ContactId (string UUID, PK)
- UserId (string UUID, FK vers User) - **NULLABLE** (pour contacts anonymes)
- Email (string) - Email du contacteur
- Subject (string) - Sujet (Bug, Suggestion, Autre)
- Message (string) - Corps du message
- Status (enum: NEW, READ, RESPONDED, ARCHIVED)
- CreatedAt (DateTime)
- Relation: User User (Many-to-One, cascade delete)

**Recipe.cs** (Cache TheMealDB):
- RecipeId (string UUID, PK)
- Title (string)
- Description (longtext)
- Instructions (longtext)
- ImageUrl (string)
- MealDBId (string, unique) - Identifiant TheMealDB
- Category (string)
- Area (string)
- Tags (string)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
- Relation: List<Favorite> Favorites

### Étape 2.4: Configurer les contraintes BD

**Important**: Utiliser des **UUIDs (VARCHAR 36)** au lieu d'auto-increment integers!

Dans le DbContext `OnModelCreating()`:

1. **Types de clés**:
   - PK: UserId, RecipeId, FavoriteId, ContactId → `HasKey(e => e.UserId)` etc
   - Type: `HasColumnType("varchar(36)")` pour UUID (VARCHAR 36)

2. **Contraintes UNIQUE**:
   - User.Auth0Id → `HasIndex(e => e.Auth0Id).IsUnique()`
   - Recipe.MealDBId → `HasIndex(e => e.MealDBId).IsUnique()`
   - Favorite → `HasIndex(e => new { e.UserId, e.RecipeId }).IsUnique()`

3. **Index pour performance**:
   - User.Email → `HasIndex(e => e.Email)`
   - Favorite.UserId → `HasIndex(e => e.UserId)`
   - ContactMessage.UserId → `HasIndex(e => e.UserId)`
   - ContactMessage.CreatedAt → `HasIndex(e => e.CreatedAt)`

4. **Relations**:
   - User → Favorite: `HasMany(e => e.Favorites).WithOne(e => e.User).OnDelete(DeleteBehavior.Cascade)`
   - User → ContactMessage: `HasMany(e => e.ContactMessages).WithOne(e => e.User).OnDelete(DeleteBehavior.SetNull)`
   - Recipe → Favorite: `HasMany(e => e.Favorites).WithOne(e => e.Recipe).OnDelete(DeleteBehavior.Cascade)`

### Étape 2.5: Créer la connexion string

1. Ouvrir `appsettings.json` dans `RecipesApp.Server`
2. Ajouter la section `ConnectionStrings`:
   ```
   "DefaultConnection": "Server=localhost;Port=3306;Database=recipes_db;User Id=recipes_user;Password=YOUR_PASSWORD;"
   ```

### Étape 2.6: Configurer EF Core

Dans `Program.cs` du Backend:
1. Ajouter le DbContext au service DI
2. Spécifier que vous utilisez MySQL avec Pomelo
3. Spécifier la version MySQL (8.0.33 par défaut)

### Étape 2.7: Créer la première migration

1. Ouvrir le Package Manager Console
2. Sélectionner le project `RecipesApp.Server` comme "Default project"
3. Exécuter: `Add-Migration InitialCreate`
4. Une nouvelle classe de migration doit être créée dans `Migrations/`

### Étape 2.8: Appliquer la migration

1. Exécuter: `Update-Database`
2. Vérifier dans MySQL que les tables ont été créées
3. Vérifier la structure des tables:
   - Table `users` avec les colonnes attendues
   - Table `favorites` avec les colonnes et contraintes
   - Table `contact_messages` avec les colonnes

### Étape 2.9: Tester la connexion

1. Créer une simple requête de test dans un Controller pour vérifier:
   - La connexion à la BD fonctionne
   - Les tables existent
   - Les données peuvent être lues/écrites

### Étape 2.10: Commit git

```
Commit message: "Sprint 2: Database setup with EF Core migrations"
Inclure: DbContext, Entities, Migrations, appsettings
```

**Checklist Sprint 2:**
- [ ] Base de données MySQL créée
- [ ] Utilisateur MySQL créé
- [ ] DbContext créé
- [ ] 3 Entities créées
- [ ] Contraintes configurées
- [ ] Migration créée et appliquée
- [ ] Tables visibles dans MySQL
- [ ] Compile sans erreurs

---

## Sprint 3: Auth0 Configuration

### Étape 3.1: Créer un compte Auth0
1. Aller sur auth0.com
2. S'inscrire (créer un compte)
3. Créer un "Tenant" (domaine Auth0)
4. Retenir le "Tenant Domain" (ex: yourdomain.auth0.com)

### Étape 3.2: Créer une application Web

1. Aller à "Applications" → "Applications"
2. Cliquer "Create Application"
3. Sélectionner "Regular Web Application"
4. Donner un nom: "RecipeHub"
5. Sélectionner la technologie: "ASP.NET Core"
6. Créer l'application

### Étape 3.3: Récupérer les credentials

1. Aller dans les "Settings" de l'application
2. Copier et sauvegarder:
   - **Domain** (ex: yourdomain.auth0.com)
   - **Client ID** (ex: xxxxxxxxxxxxx)
   - **Client Secret** (ex: yyyyyyyyyyyyyyy)

3. Dans "Application URIs", configurer:
   - **Allowed Callback URLs**: `https://localhost:5001/callback`
   - **Allowed Logout URLs**: `https://localhost:5001`
   - **Allowed Web Origins**: `https://localhost:5001`

### Étape 3.4: Créer une API

1. Aller à "Applications" → "APIs"
2. Cliquer "Create API"
3. Donner un nom: "RecipeHub API"
4. Donner un identifiant: `https://recipehub-api`
5. Créer l'API

### Étape 3.5: Configurer appsettings.json

Dans `RecipesApp.Server/appsettings.json`:
```json
{
  "Auth0": {
    "Domain": "your-domain.auth0.com",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Audience": "https://recipehub-api"
  },
  "AllowedOrigins": "https://localhost:5001"
}
```

### Étape 3.6: Créer une classe Auth0Settings

Dans `RecipesApp.Server/Auth/`:
1. Créer une classe `Auth0Settings.cs`
2. Avec les propriétés: Domain, ClientId, ClientSecret, Audience

### Étape 3.7: Configurer l'authentification dans Program.cs

Dans `RecipesApp.Server/Program.cs`:
1. Ajouter le service d'authentification JWT
2. Spécifier Auth0 comme provider
3. Configurer la validation des tokens
4. Ajouter le middleware d'authentification et autorisation

### Étape 3.8: Tester Auth0

1. Ouvrir le Dashboard Auth0
2. Aller à "Test" tab de votre application
3. Cliquer "Try Classic Universal Login"
4. Vérifier que la page de login s'ouvre
5. Vous déconnecter (fermer la fenêtre)

### Étape 3.9: Valider l'intégration

1. Vérifier que le Backend compile
2. Vérifier que les erreurs de configuration ne surgissent pas

### Étape 3.10: Commit git

```
Commit message: "Sprint 3: Auth0 integration and JWT configuration"
Inclure: Auth0Settings, Program.cs changes, appsettings
```

**Checklist Sprint 3:**
- [ ] Compte Auth0 créé
- [ ] Application Web créée
- [ ] Credentials récupérées et stockées
- [ ] URLs Auth0 configurées
- [ ] API Auth0 créée
- [ ] appsettings.json configuré
- [ ] Auth0Settings class créée
- [ ] Program.cs configuré
- [ ] Backend compile sans erreurs

---

## Sprint 4: Services & Repositories Setup

### Étape 4.1: Créer les interfaces Repository

Dans `RecipesApp.Server/Repositories/`:

Créer `IRecipeRepository.cs`:
- Méthode GetById(id) - retourne une recette
- Méthode GetAll() - retourne toutes les recettes
- Méthode GetByUserId(userId) - retourne les recettes d'un user
- Méthode Add(recipe) - ajoute une recette
- Méthode Update(recipe) - met à jour une recette
- Méthode Delete(id) - supprime une recette
- Méthode Exists(mealId) - vérifie si existe

Créer `IFavoriteRepository.cs`:
- Méthode Add(favorite) - ajoute un favori
- Méthode Remove(favoriteId) - supprime un favori
- Méthode GetByUserId(userId) - récupère les favoris d'un user
- Méthode IsFavorite(userId, mealId) - vérifie si c'est un favori

### Étape 4.2: Implémenter les Repositories

Dans `RecipesApp.Server/Repositories/`:

Créer `RecipeRepository.cs`:
- Implémenter IRecipeRepository
- Injecter ApplicationDbContext dans le constructeur
- Implémenter chaque méthode

Créer `FavoriteRepository.cs`:
- Implémenter IFavoriteRepository
- Injecter ApplicationDbContext dans le constructeur
- Implémenter chaque méthode

### Étape 4.3: Créer les interfaces Services

Dans `RecipesApp.Server/Services/`:

Créer `IRecipeService.cs`:
- Méthode GetRandomRecipes(count) - recettes aléatoires
- Méthode SearchRecipes(query, category, cuisine) - recherche
- Méthode GetCategories() - liste catégories
- Méthode GetCuisines() - liste cuisines

Créer `IFavoriteService.cs`:
- Méthode AddFavorite(userId, recipeDto) - ajouter favori
- Méthode RemoveFavorite(userId, favoriteId) - supprimer favori
- Méthode GetUserFavorites(userId) - récupérer favoris

### Étape 4.4: Implémenter les Services

Dans `RecipesApp.Server/Services/`:

Créer `RecipeService.cs`:
- Implémenter IRecipeService
- Utiliser IRecipeRepository
- Implémenter la logique métier

Créer `FavoriteService.cs`:
- Implémenter IFavoriteService
- Utiliser IFavoriteRepository
- Implémenter la logique métier

### Étape 4.5: Créer les DTOs

Dans `RecipesApp.Shared/Models/Dtos/`:

Créer `RecipeDto.cs` avec:
- Id, Title, Description, ImageUrl, Category, Cuisine, Ingredients

Créer `IngredientDto.cs` avec:
- Name, Measure

Créer `MealDto.cs` pour l'API TheMealDB

### Étape 4.6: Enregistrer les services dans DI

Dans `Program.cs` du Backend:
1. Ajouter Repositories en tant que `Scoped`
2. Ajouter Services en tant que `Scoped`
3. Ajouter IMemoryCache pour le caching

### Étape 4.7: Tester la compilation

1. Exécuter `dotnet build`
2. Vérifier qu'il n'y a pas d'erreurs

### Étape 4.8: Commit git

```
Commit message: "Sprint 4: Services and Repositories layer setup"
Inclure: Repositories, Services, DTOs, Program.cs updates
```

**Checklist Sprint 4:**
- [ ] 2 interfaces Repository créées
- [ ] 2 Repositories implémentées
- [ ] 2 interfaces Service créées
- [ ] 2 Services implémentées
- [ ] DTOs créés dans Shared
- [ ] Services enregistrés en DI
- [ ] Backend compile sans erreurs

---

## Sprint 5: TheMealDB API Integration & Controllers

### Étape 5.1: Créer le service TheMealDB

Dans `RecipesApp.Server/Services/`:

Créer `ITheMealDBService.cs` avec méthodes:
- SearchByName(query) - recherche par nom
- GetRandom() - recette aléatoire
- GetByIdAsync(mealId) - détails recette
- GetCategories() - liste catégories
- GetAreas() - liste zones/cuisines
- FilterByCategory(category) - filtre catégorie
- FilterByArea(area) - filtre cuisine

Créer `TheMealDBService.cs`:
- Implémenter ITheMealDBService
- Injecter HttpClient
- Appeler l'API TheMealDB avec retry logic (Polly)

### Étape 5.2: Configurer Polly pour résilience

Dans `Program.cs`:
1. Ajouter une politique de retry avec Polly
2. Configurer 3 retries avec backoff exponentiel
3. Configurer timeout de 10 secondes
4. Associer la politique au HttpClient de TheMealDB

### Étape 5.3: Enregistrer TheMealDB Service

Dans `Program.cs`:
1. Enregistrer `ITheMealDBService` avec HttpClientFactory

### Étape 5.4: Créer les Controllers

Dans `RecipesApp.Server/Controllers/`:

Créer `RecipesController.cs`:
- GET /api/recipes/random - 6 recettes aléatoires
- GET /api/recipes/search - recherche
- GET /api/recipes/categories - catégories
- GET /api/recipes/cuisines - cuisines

Créer `FavoritesController.cs`:
- GET /api/favorites - tous les favoris
- POST /api/favorites - ajouter
- DELETE /api/favorites/{id} - supprimer
- GET /api/favorites/{id}/exists - vérifier

Créer `ContactController.cs`:
- POST /api/contact - envoyer message

### Étape 5.5: Implémenter les endpoints

Pour chaque controller:
1. Injecter les services nécessaires
2. Implémenter la logique
3. Gérer les erreurs
4. Retourner les codes HTTP appropriés

### Étape 5.6: Tester les endpoints

1. Installer Postman ou Thunder Client
2. Tester chaque endpoint:
   - Vérifier la réponse
   - Vérifier les codes HTTP
   - Vérifier les erreurs

### Étape 5.7: Configurer CORS

Dans `Program.cs`:
1. Ajouter le middleware CORS
2. Configurer pour accepter le Frontend Blazor (localhost:5001)

### Étape 5.8: Valider l'intégration TheMealDB

1. Appeler l'endpoint `/api/recipes/random`
2. Vérifier que 6 recettes sont retournées
3. Vérifier la structure des données

### Étape 5.9: Commit git

```
Commit message: "Sprint 5: TheMealDB API integration and REST controllers"
Inclure: TheMealDBService, Controllers, Polly configuration
```

**Checklist Sprint 5:**
- [ ] TheMealDBService créé
- [ ] Polly configué pour retry
- [ ] RecipesController implémenté
- [ ] FavoritesController implémenté
- [ ] ContactController implémenté
- [ ] CORS configuré
- [ ] Endpoints testés avec Postman
- [ ] Backend compile et fonctionne

**Fin de Phase 1 ✅**: Vous avez une API REST fonctionnelle!

---

# 4. PHASE 2: FRONTEND BLAZOR (Sprints 6-10)

**Objectif**: Créer une interface Blazor complète avec toutes les pages

## Sprint 6: Frontend Setup & Navigation

### Étape 6.1: Configurer Program.cs Frontend

Dans `RecipesApp.Client/Program.cs`:
1. Ajouter HttpClient configuré avec l'API Backend
2. Ajouter Auth0 OidcClient
3. Ajouter les services (RecipeService, FavoriteService, ThemeService, etc.)
4. Configurer le logging

### Étape 6.2: Créer le Layout principal

Dans `RecipesApp.Client/`:
1. Créer `App.razor` - conteneur principal
2. Créer `MainLayout.razor` - layout avec navbar
3. Configurer les routes

### Étape 6.3: Créer la NavBar

Dans `RecipesApp.Client/Components/Shared/`:
1. Créer `NavBar.razor` component
2. Afficher le logo/titre
3. Afficher les links de navigation (qui changent selon login/logout)
4. Ajouter le toggle de thème
5. Ajouter bouton login/logout

### Étape 6.4: Créer le Theme Toggle

Dans `RecipesApp.Client/Components/Shared/`:
1. Créer `ThemeToggle.razor` component
2. Toggle entre light et dark
3. Sauvegarder en LocalStorage via JavaScript Interop
4. Appliquer le thème au document
5. Au chargement, restaurer le thème depuis localStorage

### 🎯 Étape 6.4b: Créer LocalStorageService (localStorage pour profil)

**Important**: Blazor WebAssembly n'a pas d'accès direct à localStorage (API JavaScript)

Dans `RecipesApp.Client/Services/`:

1. **Créer `LocalStorageService.cs`**:
   - Utiliser `IJSRuntime` pour communiquer avec JavaScript
   - Méthodes:
     - `GetUserProfileAsync()` → Récupère profil utilisateur
     - `SetUserProfileAsync(profile)` → Sauvegarde profil
     - `ClearUserDataAsync()` → Vide les données utilisateur
     - `GetThemeAsync()` → Récupère le thème
     - `SetThemeAsync(theme)` → Sauvegarde le thème

2. **Format localStorage (RecipeHub)**:
```json
{
  "user:profile": {
    "name": "Jean Dupont",
    "email": "jean@example.com",
    "picture": "https://...",
    "sub": "auth0|123456"
  },
  "user:theme": "dark"
}
```

3. **Flux Auth0 avec localStorage**:
   - User login → Auth0 retourne ID token
   - Extraire profil du token (Name, Email, Picture)
   - **Appeler** `LocalStorageService.SetUserProfileAsync()` pour stocker
   - UserId créé en BD pour lier favoris
   - App utilise localStorage partout (pas de requête DB pour profil)

### Étape 6.5: Créer les fichiers CSS pour thèmes

Dans `RecipesApp.Client/wwwroot/css/`:
1. Créer `styles.css` avec variables CSS
2. Créer `light-theme.css` pour le thème light
3. Créer `dark-theme.css` pour le thème dark
4. Configurer les couleurs pour chaque thème

### Étape 6.6: Créer les services Frontend

Dans `RecipesApp.Client/Services/`:

Créer `IHttpApiService.cs`:
- GetAsync<T>(url)
- PostAsync<T>(url, data)
- DeleteAsync(url)

Créer `HttpApiService.cs`:
- Implémenter les méthodes HTTP
- Gérer les erreurs
- Ajouter les headers

Créer `IThemeService.cs` et `ThemeService.cs`:
- IsDarkMode()
- SetDarkMode(isDark)
- Sauvegarder en LocalStorage

### Étape 6.7: Valider la compilation

1. Exécuter `dotnet build`
2. Vérifier qu'il n'y a pas d'erreurs

### Étape 6.8: Commit git

```
Commit message: "Sprint 6: Frontend setup with navigation and theming"
Inclure: Program.cs, Layout, NavBar, ThemeToggle, Services
```

**Checklist Sprint 6:**
- [ ] Program.cs configuré
- [ ] App.razor créé
- [ ] MainLayout créé
- [ ] NavBar créé
- [ ] ThemeToggle créé
- [ ] CSS thèmes créés
- [ ] Services frontend créés
- [ ] Frontend compile sans erreurs

---

## Sprint 7: Frontend Services & HTTP Client

### Étape 7.1: Créer IRecipeService

Dans `RecipesApp.Client/Services/`:

Créer `IRecipeService.cs`:
- GetRandomRecipes(count)
- SearchRecipes(query, category, cuisine)
- GetCategories()
- GetCuisines()
- GetRecipeDetails(id)

### Étape 7.2: Implémenter RecipeService

Créer `RecipeService.cs`:
1. Injecter IHttpApiService
2. Ajouter IMemoryCache pour caching
3. Implémenter chaque méthode
4. Appeler les endpoints Backend
5. Cacher les résultats (30 minutes)

### Étape 7.3: Créer IFavoriteService

Créer `IFavoriteService.cs`:
- AddFavorite(recipe)
- RemoveFavorite(id)
- IsFavorite(id)
- GetFavorites()

### Étape 7.4: Implémenter FavoriteService

Créer `FavoriteService.cs`:
1. Injecter IHttpApiService
2. Implémenter chaque méthode
3. Appeler les endpoints Backend

### Étape 7.5: Créer IContactService

Créer `IContactService.cs`:
- SendMessage(contactRequest)

### Étape 7.6: Implémenter ContactService

Créer `ContactService.cs`:
1. Injecter IHttpApiService
2. Implémenter SendMessage

### Étape 7.7: Enregistrer les services en DI

Dans `Program.cs` Frontend:
1. Ajouter RecipeService
2. Ajouter FavoriteService
3. Ajouter ContactService
4. Ajouter IMemoryCache

### Étape 7.8: Tester la compilation

1. Exécuter `dotnet build`
2. Vérifier qu'il n'y a pas d'erreurs

### Étape 7.9: Commit git

```
Commit message: "Sprint 7: Frontend services with API communication"
Inclure: RecipeService, FavoriteService, ContactService, DI setup
```

**Checklist Sprint 7:**
- [ ] IRecipeService créé et implémenté
- [ ] IFavoriteService créé et implémenté
- [ ] IContactService créé et implémenté
- [ ] Caching configuré
- [ ] Services enregistrés en DI
- [ ] Frontend compile sans erreurs

---

## Sprint 8: Home & Search Pages

### Étape 8.1: Créer la page Home

Dans `RecipesApp.Client/Pages/`:

Créer `Home.razor`:
1. Route: `/` et `/home`
2. Ajouter AuthorizeView pour afficher contenu différent
3. Anonyme: Afficher 6 recettes aléatoires
4. Connecté: Idem + salutation personnalisée
5. Ajouter bouton "Rafraîchir"
6. Ajouter button login/signup pour anonymes

### Étape 8.2: Créer le component RecipeCard

Dans `RecipesApp.Client/Components/Recipe/`:

Créer `RecipeCard.razor`:
1. Afficher image recette
2. Afficher titre
3. Afficher catégorie et cuisine
4. Ajouter bouton "Détails"
5. Ajouter bouton cœur (favori toggle)
6. Gérer les clics

### Étape 8.3: Créer le component RecipeModal

Créer `RecipeModal.razor`:
1. Modal popup pour détails
2. Afficher image, titre, description
3. Afficher ingrédients
4. Afficher instructions
5. Bouton fermer

### Étape 8.4: Créer la page Search

Créer `Search.razor`:
1. Route: `/search`
2. Ajouter [Authorize] pour protéger la page
3. Ajouter input search
4. Ajouter dropdowns pour catégorie et cuisine
5. Afficher les résultats en grille
6. Ajouter pagination
7. Ajouter bouton "Réinitialiser filtres"

### Étape 8.5: Ajouter logique de recherche

Dans Search.razor:
1. Debounce l'input search (300ms)
2. Appeler SearchRecipes du service
3. Afficher les résultats
4. Gérer les erreurs (message "Aucun résultat")

### Étape 8.6: Créer le component grid de résultats

Créer `RecipeGrid.razor`:
1. Afficher les recettes en grille
2. Pagination 12 résultats par page
3. Boutons précédent/suivant

### Étape 8.7: Tester les pages

1. Lancer le Frontend
2. Tester Home page (anonyme et connecté)
3. Tester Search page
4. Vérifier que les appels API fonctionnent
5. Vérifier que les données s'affichent

### Étape 8.8: Commit git

```
Commit message: "Sprint 8: Home and Search pages with components"
Inclure: Home.razor, Search.razor, RecipeCard, RecipeGrid, RecipeModal
```

**Checklist Sprint 8:**
- [ ] Home.razor créé
- [ ] RecipeCard.razor créé
- [ ] RecipeModal.razor créé
- [ ] RecipeGrid.razor créé
- [ ] Search.razor créé
- [ ] Pages testées
- [ ] Données affichées correctement

---

## Sprint 9: Favorites & Contact Pages

### Étape 9.1: Créer la page MyRecipes (Favoris)

Dans `RecipesApp.Client/Pages/`:

Créer `MyRecipes.razor`:
1. Route: `/my-recipes`
2. Ajouter [Authorize] pour protéger
3. Afficher tous les favoris de l'utilisateur
4. Ajouter filtres (catégorie, cuisine)
5. Ajouter tri (par date, par nom)
6. Afficher en grille avec boutons supprimer
7. Ajouter pagination
8. Message si aucun favori

### Étape 9.2: Ajouter logique favoris

Dans MyRecipes.razor:
1. Charger les favoris au démarrage
2. Implémenter la suppression
3. Implémenter les filtres
4. Implémenter le tri

### Étape 9.3: Créer la page Contact

Créer `Contact.razor`:
1. Route: `/contact`
2. **Pas de [Authorize]** - accessible à tous
3. Afficher différent contenu selon login/logout

**Logout (anonyme):**
- Email statique: admin@recipehub.com
- Sujet dropdown
- Message input
- Bouton Envoyer

**Login (connecté):**
- Nom pré-rempli depuis Auth0
- Email pré-rempli depuis Auth0
- Sujet dropdown
- Message input
- Checkbox newsletter
- Bouton Envoyer

### Étape 9.4: Ajouter logique formulaire contact

Dans Contact.razor:
1. Détecter si utilisateur est connecté
2. Pré-remplir les données depuis Auth0
3. Implémenter la validation
4. Implémenter l'envoi du message
5. Afficher toast de succès/erreur

### Étape 9.5: Créer component ContactForm (optionnel)

Créer `ContactForm.razor`:
1. Form réutilisable
2. Afficher les champs appropriés selon login/logout
3. Gérer la soumission

### Étape 9.6: Tester les pages

1. Tester MyRecipes (ajouter/supprimer favoris)
2. Tester Contact anonyme
3. Tester Contact connecté
4. Vérifier que les données sont envoyées au Backend

### Étape 9.7: Commit git

```
Commit message: "Sprint 9: Favorites and Contact pages"
Inclure: MyRecipes.razor, Contact.razor, ContactForm
```

**Checklist Sprint 9:**
- [ ] MyRecipes.razor créé et testé
- [ ] Contact.razor créé
- [ ] Formulaire contact valide (anonyme et connecté)
- [ ] ContactForm component créé
- [ ] Pages testées avec Backend

---

## Sprint 10: Auth0 Integration Frontend

### Étape 10.1: Créer la page Login

Dans `RecipesApp.Client/Pages/`:

Créer `Login.razor`:
1. Route: `/login`
2. Rediriger vers Auth0 pour login
3. Récupérer le code de retour
4. Stocker le JWT

### Étape 10.2: Créer la page Callback avec localStorage

Créer `LoginCallback.razor`:
1. Route: `/login/callback`
2. Traiter la réponse Auth0
3. Échanger le code contre JWT (ID token)
4. **🎯 NOUVEAU - Stocker profil dans localStorage**:
   - Extraire le profil du ID token (Name, Email, Picture, Sub)
   - Appeler `LocalStorageService.SetUserProfileAsync(profile)`
   - Stocker aussi les tokens (ID token, Access token)
5. Créer/mapper UserId en BD via l'API Backend
6. Rediriger vers `/home`

**Flux Auth0 avec localStorage**:
```
Auth0 retourne ID token
       ↓
Parser le token (jwt-decode ou Claims)
       ↓
Extraire: Name, Email, Picture, Sub
       ↓
localStorage.setItem("user:profile", JSON.stringify({...}))
       ↓
Créer UserId en BD via API /api/auth/register-user
       ↓
Rediriger vers /home
```

### Étape 10.3: Créer la logique Logout

Dans `NavBar.razor`:
1. Ajouter bouton Logout
2. Implémenter la déconnexion Auth0
3. **Effacer les données localStorage**:
   - Appeler `LocalStorageService.ClearUserDataAsync()`
   - Cela vide user:profile et tokens
4. Effacer le JWT
5. Rediriger vers `/`

### Étape 10.4: Configurer AuthorizeView

Dans les pages protégées:
1. Ajouter `<AuthorizeView>`
2. Afficher contenu pour "Authorized"
3. Afficher contenu pour "NotAuthorized"

### Étape 10.5: Protéger les routes

Pour les pages qui nécessitent login:
- `/search`
- `/my-recipes`
- `/contact` (optionnel car c'est une page publique)

### Étape 10.6: Récupérer les données utilisateur depuis localStorage

**🎯 IMPORTANT**: Les données utilisateur sont dans localStorage, pas dans les Claims!

Dans les composants:
1. Injecter `LocalStorageService` (créé à l'étape 6.4b)
2. Appeler `GetUserProfileAsync()` pour récupérer le profil
3. Utiliser le profil pour pré-remplir les formulaires:
   - Nom → `userProfile.Name`
   - Email → `userProfile.Email`
   - Picture → `userProfile.Picture` (pour avatar)
   - Sub → `userProfile.Sub` (Auth0 ID)

**Exemple: Contact Form**
```
@inject LocalStorageService LocalStorageService

@if (isAuthenticated)
{
    <input value="@userProfile?.Name" />
    <input value="@userProfile?.Email" />
}
else
{
    <input placeholder="admin@recipehub.com" disabled />
}

@code {
    private UserProfile? userProfile;

    protected override async Task OnInitializedAsync()
    {
        userProfile = await LocalStorageService.GetUserProfileAsync();
    }
}
```

### Étape 10.7: Tester le flux Auth0

1. Cliquer "Login" dans la navbar
2. Remplir les credentials Auth0
3. Vérifier la redirection vers `/home`
4. Vérifier que le nom s'affiche
5. Tester Logout
6. Vérifier la redirection vers `/`

### Étape 10.8: Tester les pages protégées

1. Tenter d'accéder à `/search` anonyme
2. Vérifier que c'est bloqué
3. Login
4. Tenter d'accéder à `/search` connecté
5. Vérifier que c'est accessible

### Étape 10.9: Commit git

```
Commit message: "Sprint 10: Auth0 frontend integration and page protection"
Inclure: Login.razor, LoginCallback.razor, AuthorizeView updates
```

**Checklist Sprint 10:**
- [ ] Login.razor créé
- [ ] LoginCallback.razor créé
- [ ] Logout implémenté
- [ ] AuthorizeView configuré
- [ ] Pages protégées
- [ ] Données utilisateur pré-remplies
- [ ] Auth0 flow testé

**Fin de Phase 2 ✅**: Vous avez un Frontend Blazor complet!

---

# 5. PHASE 3: TESTS (Sprints 11-15)

**Objectif**: Atteindre 75% de couverture de tests

## Sprint 11: xUnit Services Tests

### Étape 11.1: Tester RecipeService

Dans `RecipesApp.Tests/Unit/Services/`:

Créer tests pour:
1. GetRandomRecipes retourne le bon nombre
2. SearchRecipes avec query valide retourne résultats
3. SearchRecipes avec query invalide retourne vide
4. GetCategories cache les résultats
5. GetCuisines cache les résultats

### Étape 11.2: Tester FavoriteService

Créer tests pour:
1. AddFavorite ajoute correctement
2. RemoveFavorite supprime correctement
3. IsFavorite retourne true/false correct
4. GetFavorites retourne la liste

### Étape 11.3: Tester TheMealDBService

Créer tests pour:
1. SearchByName retourne résultats
2. GetRandom retourne une recette
3. Gestion des erreurs réseau (Polly retry)
4. Gestion des timeouts

### Étape 11.4: Tester ContactService

Créer tests pour:
1. SendMessage envoie correctement
2. Validation des champs
3. Gestion des erreurs

### Étape 11.5: Utiliser Moq pour les mocks

1. Mocker les dépendances (HttpClient, DB)
2. Utiliser `.Setup()` pour configurer les mocks
3. Utiliser `.Verify()` pour vérifier les appels

### Étape 11.6: Utiliser FluentAssertions

1. Utiliser `.Should()` pour les assertions
2. Utiliser `.Should().Be()`, `.Should().Contain()`, etc.

### Étape 11.7: Exécuter les tests

1. Ouvrir le Terminal
2. Exécuter: `dotnet test RecipesApp.Tests`
3. Vérifier que tous les tests passent

### Étape 11.8: Commit git

```
Commit message: "Sprint 11: Unit tests for services (30+ tests)"
Inclure: Services tests, Moq setup, FluentAssertions
```

**Checklist Sprint 11:**
- [ ] RecipeServiceTests créés (5+ tests)
- [ ] FavoriteServiceTests créés (4+ tests)
- [ ] TheMealDBServiceTests créés (4+ tests)
- [ ] ContactServiceTests créés (3+ tests)
- [ ] Tous les tests passent (GREEN)
- [ ] Coverage visible

---

## Sprint 12: xUnit Repository Tests

### Étape 12.1: Configurer Testcontainers

1. Ajouter Testcontainers.MySql
2. Créer une fixture qui lance MySQL dans un container Docker

### Étape 12.2: Tester RecipeRepository

Créer tests pour:
1. GetById retourne une recette
2. GetAll retourne toutes les recettes
3. Add ajoute correctement
4. Update met à jour correctement
5. Delete supprime correctement
6. Exists retourne true/false

### Étape 12.3: Tester FavoriteRepository

Créer tests pour:
1. Add ajoute un favori
2. Remove supprime un favori
3. GetByUserId retourne les favoris
4. Contrainte d'unicité fonctionne

### Étape 12.4: Utiliser des transactions dans les tests

1. Chaque test utilise une transaction
2. Rollback automatique après chaque test
3. Isolation des données

### Étape 12.5: Exécuter les tests

1. Exécuter: `dotnet test RecipesApp.Tests`
2. Vérifier que les tests passent
3. Vérifier que MySQL est bien isolé

### Étape 12.6: Commit git

```
Commit message: "Sprint 12: Repository tests with Testcontainers (20+ tests)"
Inclure: Repository tests, Testcontainers configuration
```

**Checklist Sprint 12:**
- [ ] RecipeRepositoryTests créés (6+ tests)
- [ ] FavoriteRepositoryTests créés (4+ tests)
- [ ] Testcontainers configuré
- [ ] Tous les tests passent (GREEN)
- [ ] Données isolées par test

---

## Sprint 13: BUnit Component Tests

### Étape 13.1: Tester RecipeCard

Dans `RecipesApp.Client.Tests/Components/`:

Créer tests pour:
1. RecipeCard affiche le titre
2. RecipeCard affiche l'image
3. Click bouton détails ouvre modal
4. Click cœur appelle AddFavorite
5. Cœur affiche bon état (vide/plein)

### Étape 13.2: Tester NavBar

Créer tests pour:
1. Logo affiche le titre
2. Menu affiche les links corrects (login/logout)
3. ThemeToggle visible
4. Click logout déconnecte

### Étape 13.3: Tester ThemeToggle

Créer tests pour:
1. Bouton change l'apparence
2. LocalStorage est mis à jour
3. Changement appliqué au document

### Étape 13.4: Tester Home page

Créer tests pour:
1. Affiche 6 recettes (anonyme)
2. Bouton Rafraîchir charge nouvelles recettes
3. Affiche salutation (connecté)

### Étape 13.5: Utiliser BUnit

1. `RenderComponent<T>()` pour renderer un component
2. `Find()` pour trouver des éléments DOM
3. `ClickAsync()` pour simuler des clics
4. Mocker les services injectés

### Étape 13.6: Exécuter les tests

1. Exécuter: `dotnet test RecipesApp.Client.Tests`
2. Vérifier que tous les tests passent

### Étape 13.7: Commit git

```
Commit message: "Sprint 13: BUnit component tests (15+ tests)"
Inclure: Component tests, BUnit setup, mocked services
```

**Checklist Sprint 13:**
- [ ] RecipeCardComponentTests créés (5+ tests)
- [ ] NavBarComponentTests créés (4+ tests)
- [ ] ThemeToggleComponentTests créés (3+ tests)
- [ ] HomePageTests créés (3+ tests)
- [ ] Tous les tests passent (GREEN)

---

## Sprint 14: BUnit Page Tests & Integration

### Étape 14.1: Tester Search page

Créer tests pour:
1. Input search déclenche recherche
2. Résultats affichés correctement
3. Filtres appliqués
4. Pagination fonctionne
5. Message "Aucun résultat"

### Étape 14.2: Tester Contact page

Créer tests pour:
1. Formulaire anonyme affiche email statique
2. Formulaire connecté affiche email pré-rempli
3. Validation du formulaire
4. Soumission du formulaire
5. Toast de succès

### Étape 14.3: Tester MyRecipes page

Créer tests pour:
1. Affiche les favoris
2. Filtre par catégorie
3. Bouton supprimer fonctionne
4. Message "Aucun favori"

### Étape 14.4: Tests d'intégration Frontend-Backend

Créer tests pour:
1. Appel à RecipeService → endpoint Backend
2. Appel à FavoriteService → endpoint Backend
3. Appel à ContactService → endpoint Backend

### Étape 14.5: Exécuter les tests

1. Exécuter: `dotnet test`
2. Vérifier que tous les tests passent

### Étape 14.6: Commit git

```
Commit message: "Sprint 14: Page tests and integration tests (15+ tests)"
Inclure: Page tests, Integration tests, Complex scenarios
```

**Checklist Sprint 14:**
- [ ] SearchPageTests créés (5+ tests)
- [ ] ContactPageTests créés (5+ tests)
- [ ] MyRecipesPageTests créés (5+ tests)
- [ ] Tous les tests passent (GREEN)
- [ ] Tests d'intégration décents

---

## Sprint 15: Coverage Analysis & Reports

### Étape 15.1: Générer le rapport de coverage

1. Exécuter: `dotnet test /p:CollectCoverage=true`
2. Générer rapport: `dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover`

### Étape 15.2: Analyser la couverture

1. Identifier les zones non couvertes
2. Cibler 75%+ de couverture
3. Ajouter des tests pour les gaps

### Étape 15.3: Tests des edge cases

Ajouter des tests pour:
1. Erreurs réseau
2. Données invalides
3. Timeouts
4. Edge cases (liste vide, valeur null, etc.)

### Étape 15.4: Documenter la stratégie de test

Créer un document `TESTING_REPORT.md`:
1. Coverage par layer (Services, Repos, Components)
2. Types de tests effectués
3. Stratégies utilisées
4. Points d'amélioration

### Étape 15.5: Vérifier la qualité

1. Tous les tests passent (GREEN)
2. Coverage > 75%
3. Pas de flaky tests
4. Tests bien nommés et documentés

### Étape 15.6: Commit git

```
Commit message: "Sprint 15: Coverage analysis and final test improvements"
Inclure: Coverage reports, Additional tests, Testing documentation
```

**Checklist Sprint 15:**
- [ ] Coverage > 75%
- [ ] Tous les tests passent (GREEN)
- [ ] Edge cases couverts
- [ ] Documentation de test créée

**Fin de Phase 3 ✅**: Vous avez une couverture de test complète!

---

# 6. PHASE 4: FINALISATION (Sprints 16-19)

**Objectif**: Préparer le projet pour la remise

## Sprint 16: Documentation

### Étape 16.1: Ajouter XML Comments

Pour chaque classe/méthode publique:
1. Ajouter `/// <summary>` avec description
2. Ajouter `/// <param>` pour les paramètres
3. Ajouter `/// <returns>` pour le retour
4. Ajouter `/// <exception>` pour les exceptions

### Étape 16.2: Créer ARCHITECTURE.md

1. Expliquer l'architecture générale
2. Diagrammes en ASCII
3. Pattern utilisés (Repository, Service, Clean Architecture)
4. Flux des données

### Étape 16.3: Créer API_DOCUMENTATION.md

1. Lister tous les endpoints
2. Paramètres
3. Réponses
4. Codes d'erreur

### Étape 16.4: Créer CONTRIBUTING.md

1. Comment clone le repo
2. Comment setup l'environnement
3. Comment exécuter les tests
4. Comment faire un commit

### Étape 16.5: Mettre à jour README.md

1. Vérifier que tout est à jour
2. Vérifier les chemins
3. Vérifier les instructions

### Étape 16.6: Créer CHANGELOG.md

1. Lister les features implémentées
2. Lister les bugs fixés
3. Version actuelle

### Étape 16.7: Commit git

```
Commit message: "Sprint 16: Documentation and code comments"
Inclure: XML comments, Architecture docs, API docs, Contributing guide
```

**Checklist Sprint 16:**
- [ ] XML comments ajoutés
- [ ] ARCHITECTURE.md créé
- [ ] API_DOCUMENTATION.md créé
- [ ] CONTRIBUTING.md créé
- [ ] README.md mis à jour
- [ ] CHANGELOG.md créé

---

## Sprint 17: Polish & Bug Fixes

### Étape 17.1: Tester l'application complet

1. Scénario 1: Utilisateur anonyme
   - Voir Home page
   - Voir 6 recettes
   - Cliquer refresh
   - Voir nouvelles recettes

2. Scénario 2: Login
   - Cliquer Login
   - Remplir Auth0
   - Être redirigé vers Home connecté
   - Voir le nom

3. Scénario 3: Recherche
   - Aller à Search
   - Taper un nom
   - Voir résultats
   - Ajouter un favori

4. Scénario 4: Favoris
   - Aller à My Recipes
   - Voir les favoris
   - Supprimer un favori

5. Scénario 5: Contact
   - Aller à Contact
   - Remplir le formulaire (anonyme et connecté)
   - Envoyer

6. Scénario 6: Thème
   - Cliquer le toggle thème
   - Vérifier le changement
   - Refresh la page
   - Vérifier la persistance

### Étape 17.2: Tester responsive design

1. Desktop (1920x1080)
2. Tablet (768x1024)
3. Mobile (375x667)

### Étape 17.3: Vérifier l'UX

1. Boutons cliquables
2. Inputs validés
3. Messages d'erreur clairs
4. Animations fluides
5. Pas de console errors

### Étape 17.4: Fixer les bugs

1. Créer des issues pour chaque bug
2. Fixer chaque bug
3. Tester le fix
4. Commit le fix

### Étape 17.5: Optimiser la performance

1. Images lazy-loaded
2. Caching en place
3. Pas de fuites mémoire
4. Requêtes optimisées

### Étape 17.6: Tester en production mode

1. Build Release: `dotnet build -c Release`
2. Vérifier que tout fonctionne

### Étape 17.7: Commit git

```
Commit message: "Sprint 17: Polish, bug fixes and performance optimization"
Inclure: Bug fixes, Performance improvements, UX enhancements
```

**Checklist Sprint 17:**
- [ ] Tous les scénarios testés
- [ ] Responsive OK
- [ ] UX smooth
- [ ] Pas de console errors
- [ ] Pas de bugs critiques

---

## Sprint 18: Final Testing & Release Build

### Étape 18.1: Exécuter tous les tests

1. Exécuter: `dotnet test`
2. Vérifier tous les tests passent (GREEN)
3. Vérifier coverage > 75%

### Étape 18.2: Exécuter le build Release

1. Exécuter: `dotnet build -c Release`
2. Vérifier qu'il n'y a pas d'erreurs
3. Vérifier qu'il n'y a pas de warnings critiques

### Étape 18.3: Tester le workflow complet

1. Démarrer Backend: `dotnet run --project RecipesApp.Server -c Release`
2. Démarrer Frontend: `dotnet run --project RecipesApp.Client -c Release`
3. Tester chaque workflow (voir Sprint 17)

### Étape 18.4: Vérifier la remise

Checklist finale:
- [ ] Git repository propre
- [ ] Tous les commits significatifs
- [ ] Tous les fichiers committed
- [ ] Pas de fichiers secrets (appsettings secrets)
- [ ] README.md clair
- [ ] Tests documentés
- [ ] Code bien commenté

### Étape 18.5: Préparer les credentials

1. Documenter comment configurer:
   - Auth0 (domain, client ID, secret)
   - MySQL (connection string)
   - appsettings.json

### Étape 18.6: Créer un script de setup

1. Script pour installer les dépendances
2. Script pour créer la DB
3. Script pour lancer l'application

### Étape 18.7: Commit final

```
Commit message: "Sprint 18: Final release build and comprehensive testing"
Inclure: Release build, Final tests, Production checklist
```

**Checklist Sprint 18:**
- [ ] Tous les tests passent (GREEN)
- [ ] Build Release sans erreurs
- [ ] Workflow complet testé
- [ ] Repository propre
- [ ] Credentials documentés
- [ ] Ready to submit

---

## Sprint 19: Presentation & Final Review

### Étape 19.1: Créer une présentation

1. Slide 1: Vue d'ensemble du projet
2. Slide 2: Tech stack utilisé
3. Slide 3: Architecture globale (diagramme)
4. Slide 4: Features clés
5. Slide 5: Demo (plan de démo)
6. Slide 6: Challenges & solutions
7. Slide 7: Tests & Quality
8. Slide 8: Lessons learned

### Étape 19.2: Préparer une démo

Script de démo:
1. Ouvrir l'app
2. Voir Home (anonyme)
3. Login avec Auth0
4. Voir Home (connecté)
5. Rechercher une recette
6. Ajouter un favori
7. Aller à My Recipes
8. Envoyer un message contact
9. Changer le thème
10. Logout

### Étape 19.3: Nettoyer le code

1. Supprimer les commentaires de debug
2. Supprimer les fichiers inutilisés
3. Formater le code (C# standard)
4. Pas de TODOs laissés

### Étape 19.4: Vérifier tous les docs

1. README.md - Complet et à jour
2. ARCHITECTURE.md - Clair et détaillé
3. API_DOCUMENTATION.md - Complet
4. CONTRIBUTING.md - Instructif
5. CHANGELOG.md - À jour
6. XML comments - Partout

### Étape 19.5: Git history clean

1. Commits bien nommés
2. Commits logiques (pas de "fix" multiple)
3. Pas de commits de debug
4. Histoire cohérente et lisible

### Étape 19.6: Final verification

Vérifier:
- [ ] Tous les features work
- [ ] Tests tous passent
- [ ] Docs complets
- [ ] Code propre
- [ ] Git history clean
- [ ] Prêt à présenter

### Étape 19.7: Final commit

```
Commit message: "Sprint 19: Final polish and preparation for submission"
Inclure: Cleanup, Documentation, Presentation materials
```

### Étape 19.8: Tag la version

1. Créer un tag: `git tag v1.0.0`
2. Pousser le tag

**Checklist Sprint 19:**
- [ ] Présentation créée
- [ ] Démo script préparé
- [ ] Code nettoyé
- [ ] Docs vérifiés
- [ ] Git history clean
- [ ] Tag v1.0.0 créé

**Fin de Phase 4 ✅**: Vous êtes prêt à soumettre!

---

# 7. CHECKLIST PAR MILESTONE

## ✅ Fin Phase 1 (Après Sprint 5)

**Backend API fonctionnelle**

- [ ] Solution créée avec tous les projects
- [ ] Base de données MySQL connectée
- [ ] EF Core migrations appliquées
- [ ] Auth0 configuré et intégré
- [ ] Repositories implémentés
- [ ] Services implémentés
- [ ] TheMealDB intégré
- [ ] Controllers créés et testés
- [ ] Endpoints fonctionnent
- [ ] CORS configuré
- [ ] Backend compile sans erreurs

**Résultat**: Vous avez une API REST complètement fonctionnelle

---

## ✅ Fin Phase 2 (Après Sprint 10)

**Frontend Blazor complet**

- [ ] Structure Blazor setup
- [ ] Navigation et layout
- [ ] Tous les services frontend créés
- [ ] Home page implémentée
- [ ] Search page implémentée
- [ ] MyRecipes page implémentée
- [ ] Contact page implémentée
- [ ] Auth0 login/logout
- [ ] Theme dark/light
- [ ] Pages protégées avec [Authorize]
- [ ] Frontend compile sans erreurs
- [ ] Application fonctionnelle côté utilisateur

**Résultat**: Vous avez un Frontend Blazor complet et fonctionnel

---

## ✅ Fin Phase 3 (Après Sprint 15)

**Tests complets et couverture 75%+**

- [ ] Services tests créés (30+ tests)
- [ ] Repository tests créés (20+ tests)
- [ ] Component tests créés (15+ tests)
- [ ] Page tests créés (15+ tests)
- [ ] Coverage > 75%
- [ ] Tous les tests passent (GREEN)
- [ ] Testcontainers configuré
- [ ] BUnit configuré
- [ ] Moq utilisé correctement
- [ ] FluentAssertions utilisé
- [ ] Coverage report généré

**Résultat**: Vous avez une excellente couverture de tests et confiance en la qualité

---

## ✅ Fin Phase 4 (Après Sprint 19)

**Prêt pour la remise**

- [ ] XML comments ajoutés
- [ ] ARCHITECTURE.md créé
- [ ] API_DOCUMENTATION.md créé
- [ ] CONTRIBUTING.md créé
- [ ] README.md mis à jour
- [ ] Tous les bugs fixés
- [ ] UX lissée
- [ ] Performance optimisée
- [ ] Build Release créé
- [ ] Git history clean
- [ ] Présentation créée
- [ ] Démo préparée
- [ ] Ready to submit

**Résultat**: Vous êtes prêt à soumettre votre projet avec confiance!

---

# 8. POINTS CLÉS À RETENIR

## 🎯 Avant de commencer

1. **Lire tous les guides** (TheMealDB, Auth0, MySQL, Blazor, Architecture, Tests, EF Core)
2. **Setup l'environnement** (.NET SDK, MySQL, Git, Auth0 account)
3. **Commencer par le Backend** (difficile en premier)

## 🛠️ Pendant le développement

1. **Commit régulièrement** (après chaque sprint)
2. **Tester régulièrement** (ne pas laisser les tests pour la fin)
3. **Compiler souvent** (repérer les erreurs tôt)
4. **Consulter les guides** (référence rapide)

## 🔍 Points critiques

1. **Phase 1**: Si le Backend ne marche pas, tout échoue
2. **Phase 2**: Tester les appels API au Backend
3. **Phase 3**: Les tests donnent confiance pour les phases 4
4. **Phase 4**: La documentation est aussi importante que le code

## ⚡ Si vous êtes en retard

1. **Prioriser** au lieu de tout faire
2. **Couverture tests**: Au minimum Services et Controllers
3. **Documentation**: README + ARCHITECTURE suffisent
4. **Features**: Focus sur les requirements, pas les extras

## 📝 Avant la remise

1. **Vérifier git** est clean et les commits sont logiques
2. **Vérifier les tests** passent tous
3. **Vérifier la build** Release compile
4. **Vérifier le workflow** complet fonctionne
5. **Lire le README** - s'il est confus, les autres aussi

---

## VOUS ÊTES PRÊT! 🚀

Suivez ce plan étape par étape, et vous aurez un projet professionnel prêt à présenter.

**Bonne chance!**

---

# 🚀 EXTENSIONS POST-1.0.0

## Sprint 20 : User Recipes (Créer ses propres recettes)

**Objectif** : Permettre aux utilisateurs authentifiés de créer et partager leurs propres recettes

**État** : 🔄 En cours (Tâche 4/16)

### Résumé du Sprint

Ajout de la fonctionnalité **User Recipes** permettant :
- ✅ Créer des recettes personnalisées (titre, instructions, ingrédients, etc.)
- ✅ Toggle public/privé pour partager avec la communauté
- ✅ Voir ses propres recettes créées
- ✅ Supprimer ses recettes (propriétaire seulement)
- ✅ Découvrir les recettes de la communauté dans Search

### Architecture

**Pattern maintenu** :
- Entity → Repository → Service → Controller (backend)
- IHttpApiService → ServiceFrontend → Page (frontend)

**Nouvelles entités** :
- `UserRecipe` : Entité stockant les recettes utilisateur
- `UserRecipeDto` : DTO pour les transferts
- `CreateUserRecipeRequest` : DTO pour la création

### Endpoints API

```
POST   /api/user-recipes              [Authorize]  Créer une recette
GET    /api/user-recipes/my           [Authorize]  Mes recettes créées
GET    /api/user-recipes/public       [Authorize]  Toutes les recettes publiques
DELETE /api/user-recipes/{id}         [Authorize]  Supprimer (propriétaire seulement)
PATCH  /api/user-recipes/{id}/visibility [Authorize] Toggle public/privé
```

### Tâches et Progress

| Tâche # | Description | Statut | Notes |
|---------|-------------|--------|-------|
| 1 | Créer DTOs Shared | ✅ | UserRecipeDto, CreateUserRecipeRequest |
| 2 | Créer Entity UserRecipe | ✅ | Avec FK vers User.Auth0Id |
| 3 | Modifier ApplicationDbContext.cs | ✅ | DbSet + config EF |
| 4 | Migrations EF | ✅ | Commit: d2a84a7 - Table UserRecipes créée |
| 5 | Repository IUserRecipeRepository | ✅ | Commit: 06952e9 |
| 6 | Service IUserRecipeService (backend) | ⏳ | À implémenter |
| 7 | Controller UserRecipesController | ⏳ | À implémenter |
| 8 | DI Program.cs (backend) | ⏳ | À enregistrer |
| 9 | Service IUserRecipeService (frontend) | ⏳ | HTTP client |
| 10 | DI Program.cs (frontend) | ⏳ | À enregistrer |
| 11 | Page CreateRecipe.razor | ⏳ | Formulaire création |
| 12 | Page MyCreatedRecipes.razor | ⏳ | Liste et gestion |
| 13 | Modifier MainLayout.razor | ⏳ | Ajouter liens nav |
| 14 | Modifier Search.razor | ⏳ | Section communauté |
| 15 | Vérification et tests manuels | ⏳ | 6 scénarios |
| 16 | Commit Sprint 20 | ⏳ | Message: "Sprint 20 : User Recipes..." |

### Détails des fichiers complétés

#### ✅ Tâche 1 : DTOs Shared

**`menuMalin.Shared/Models/Dtos/UserRecipeDto.cs`**
```csharp
public class UserRecipeDto
{
    public string UserRecipeId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Area { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> Ingredients { get; set; } = new();
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**`menuMalin.Shared/Models/Requests/CreateUserRecipeRequest.cs`**
```csharp
public class CreateUserRecipeRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Area { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> Ingredients { get; set; } = new();
    public bool IsPublic { get; set; } = false;
}
```

#### ✅ Tâche 2 : Entity UserRecipe

**`menuMalin.Server/Models/Entities/UserRecipe.cs`**
- PK : UserRecipeId (UUID varchar(36))
- FK : UserId → User.Auth0Id (cascade delete)
- Champs : Title*, Instructions*, Category?, Area?, ImageUrl?, IngredientsJson (JSON)
- Métadonnées : IsPublic (bool), CreatedAt, UpdatedAt

#### ✅ Tâche 3 : DbContext

**Modifications à ApplicationDbContext.cs** :
1. Ajout `DbSet<UserRecipe> UserRecipes`
2. Configuration EF dans OnModelCreating :
   - Clé primaire et types de colonnes
   - Indexes sur UserId et IsPublic
   - Relation HasOne(User) avec FK sur Auth0Id

---

### Tâches restantes à compléter

#### ⏳ Tâche 4-8 : Backend (Repository, Service, Controller)

**À faire** :
1. `IUserRecipeRepository.cs` avec méthodes CRUD
2. `UserRecipeRepository.cs` avec logique EF
3. `IUserRecipeService.cs` + `UserRecipeService.cs`
4. `UserRecipesController.cs` avec les 5 endpoints
5. Enregistrement DI dans `Program.cs`

**Référence pattern** : Calqué sur FavoriteRepository/FavoriteService

#### ⏳ Tâche 9-10 : Frontend HTTP Service

**À faire** :
1. `menuMalin/Services/IUserRecipeService.cs` (HTTP client)
2. `menuMalin/Services/UserRecipeService.cs`
3. Enregistrement DI dans `Program.cs`

**Appels HTTP** :
- POST `/api/user-recipes`
- GET `/api/user-recipes/my`
- GET `/api/user-recipes/public`
- DELETE `/api/user-recipes/{id}`
- PATCH `/api/user-recipes/{id}/visibility`

#### ⏳ Tâche 11-14 : Pages Frontend

**CreateRecipe.razor** :
- Route: `/create-recipe`
- `@attribute [Authorize]`
- Formulaire : titre*, catégorie, cuisine, instructions*, image URL
- Ingrédients dynamiques (liste avec +/× buttons)
- Toggle Bootstrap : "Rendre ma recette publique"
- Submit → POST `/api/user-recipes` → redirect `/my-created-recipes`

**MyCreatedRecipes.razor** :
- Route: `/my-created-recipes`
- `@attribute [Authorize]`
- GET `/api/user-recipes/my`
- Affiche : titre, catégorie, badge "Public 🌍" / "Privé 🔒"
- Boutons : toggle visibilité, supprimer
- Message si aucune recette

**MainLayout.razor** (modifications) :
- Ajouter dans `<Authorized>` :
  ```razor
  <NavLink href="my-created-recipes">Mes Recettes</NavLink>
  <NavLink href="create-recipe">Créer une Recette</NavLink>
  ```

**Search.razor** (ajout en bas) :
- Section "Recettes de la communauté"
- GET `/api/user-recipes/public`
- Filtre côté frontend par searchTerm
- Affichage en grille (RecipeCard-like)

#### ⏳ Tâche 15 : Tests manuels

**6 scénarios à valider** :

1. **Create Recipe**
   - [ ] Formulaire visible et accessible
   - [ ] Validation des champs requis
   - [ ] Soumission crée une recette
   - [ ] Redirection vers `/my-created-recipes`

2. **My Created Recipes**
   - [ ] Liste les recettes créées
   - [ ] Badge "Public/Privé" correct
   - [ ] Bouton toggle change le badge
   - [ ] Bouton supprimer fonctionne

3. **Visibility Toggle**
   - [ ] Recette privée n'apparaît pas en public
   - [ ] Recette publique apparaît dans Search
   - [ ] Toggle change l'état

4. **Community Search**
   - [ ] Section "Communauté" dans Search
   - [ ] Affiche recettes publiques
   - [ ] Filtre par searchTerm fonctionne

5. **Authorization**
   - [ ] Non-propriétaire ne peut pas supprimer (403)
   - [ ] Non-propriétaire ne peut pas modifier visibilité (403)
   - [ ] Pages protégées par `[Authorize]`

6. **Navigation**
   - [ ] Liens visibles quand authentifié
   - [ ] Liens cachés quand anonyme

#### ⏳ Tâche 16 : Commit final

```bash
git add .
git commit -m "Sprint 20 : User Recipes - Création et partage de recettes personnalisées"
```

---

### Commandes clés

```bash
# Migration EF
dotnet ef migrations add AddUserRecipes --project menuMalin.Server

# Appliquer migration
dotnet ef database update --project menuMalin.Server

# Build verification
dotnet build menuMalin.Server
dotnet build menuMalin

# Tests
dotnet test

# Run projects
dotnet run --project menuMalin.Server
dotnet run --project menuMalin
```

---

### Important à retenir

⚠️ **UserId** fonctionne avec `User.Auth0Id` (pas UUID)
⚠️ **Ingrédients** stockés en JSON dans `IngredientsJson`, sérialisés/désérialisés au niveau du service
⚠️ **Visibilité** contrôlée par le booléen `IsPublic`
⚠️ **Autorisation** : Vérifier que seul le propriétaire peut modifier/supprimer

---

**Dernière mise à jour** : 2026-02-24 | Tâche 4 en progress

