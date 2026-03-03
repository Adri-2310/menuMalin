# 🍳 PROJET APPLICATION RECETTES - DOCUMENTATION COMPLÈTE

**Version**: 1.0
**Date de création**: 08-02-2026
**Deadline de remise**: 8 mars 2026
**Stack**: C# 9 | Blazor WebAssembly | ASP.NET Core | MySQL | Auth0 | xUnit | BUnit

---

## 📑 TABLE DES MATIÈRES

1. [Vue d'ensemble du projet](#1-vue-densemble-du-projet)
2. [Exigences minimales (Exam)](#2-exigences-minimales-exam)
3. [Architecture technique](#3-architecture-technique)
4. [Structure détaillée des features](#4-structure-détaillée-des-features)
5. [Modèle de données](#5-modèle-de-données)
6. [Intégrations externes](#6-intégrations-externes)
7. [Plan d'implémentation](#8-plan-dimplémentation)
8. [Tests (xUnit + BUnit)](#9-tests-xunit--bunit)
9. [Checklist finale](#10-checklist-finale)
10. [Améliorations futures](#11-améliorations-futures-roadmap-post-exam)

---

# 1. VUE D'ENSEMBLE DU PROJET

## 1.1 Objectif
Application **Blazor WebAssembly** de découverte et gestion de recettes de cuisine avec:
- Affichage de données d'une API externe (TheMealDB)
- Authentification utilisateur (Auth0)
- Gestion des favoris en base de données MySQL
- Tests complets (xUnit + BUnit)
- Documentation professionnelle

## 1.2 Valeur client
- **Utilisateurs anonymes**: Découvrir 6 recettes aléatoires sans inscription
- **Utilisateurs authentifiés**:
  - Créer/gérer/supprimer leurs favoris
  - Rechercher des recettes par critères
  - Envoyer des messages de contact
- **Tous les utilisateurs**: Thème dark/light persistant

## 1.3 Tech Stack
```
Frontend:     Blazor WebAssembly (C# pur)
Backend:      ASP.NET Core 9 (C#)
Database:     MySQL 8.0+
ORM:          Entity Framework Core
Auth:         Auth0 + OpenID Connect
External API: TheMealDB (gratuit, public)
Tests:        xUnit + BUnit + Moq
```

---

# 2. EXIGENCES MINIMALES (EXAM)

## 2.1 Requirements obligatoires

✅ **Développer une application en Blazor**
- WebAssembly (client-side) ou Server (à confirmer)
- C# pour tout le code

✅ **Afficher données venant d'une API**
- TheMealDB API intégrée
- Affichage dynamique de recettes
- Filtres et recherche

✅ **Utilisateur authentifié peut:**
- ✅ Créer des favoris localement (MySQL)
- ✅ Modifier les données (favoris)
- ✅ Gérer ses favoris
- ✅ Supprimer ses favoris

✅ **Tester, documenter, présenter**
- Tests xUnit + BUnit
- Documentation README + ARCHITECTURE
- Presentation slides/demo

## 2.2 Critères d'acceptation
```
- [ ] Application compile sans erreurs
- [ ] Utilisateur anonyme voit 6 recettes aléatoires
- [ ] Utilisateur peut se connecter via Auth0
- [ ] Utilisateur authentifié peut ajouter/supprimer favoris
- [ ] Favoris sauvegardés en DB MySQL
- [ ] Tests > 75% coverage
- [ ] Documentation complète
- [ ] Démo workflow fonctionnel
```

---

# 3. ARCHITECTURE TECHNIQUE

## 3.1 Architecture globale

```
┌─────────────────────────────────────────────┐
│  BLAZOR WEBASSEMBLY (Frontend - C#)        │
│  ├─ Pages: Home, Search, MyRecipes, Contact│
│  ├─ Components: RecipeCard, NavBar, etc.  │
│  ├─ Services: Recipe, Favorite, Http API  │
│  └─ State Management (AppState)           │
└──────────────┬──────────────────────────────┘
               │ HTTP/REST API
               ▼
┌─────────────────────────────────────────────┐
│  ASP.NET CORE BACKEND (C#)                 │
│  ├─ Controllers: Recipes, Favorites, etc. │
│  ├─ Services: Business logic               │
│  ├─ Repositories: Data access             │
│  └─ EF Core: MySQL ORM                     │
└──────────────┬──────────────────────────────┘
               │
       ┌───────┼───────┐
       ▼       ▼       ▼
    MySQL   Auth0   TheMealDB
     DB      API      API
```

## 3.2 Structure du projet

```
RecipesApp/
├── RecipesApp.Client/                      # Blazor WebAssembly Frontend
│   ├── Pages/
│   │   ├─ Home.razor                      # Page d'accueil
│   │   ├─ Search.razor                    # Recherche recettes
│   │   ├─ MyRecipes.razor                 # Favoris utilisateur
│   │   ├─ Contact.razor                   # Formulaire contact
│   │   └─ Index.razor
│   ├── Components/
│   │   ├─ Shared/
│   │   │   ├─ NavBar.razor
│   │   │   ├─ ThemeToggle.razor
│   │   │   └─ Layout.razor
│   │   ├─ Recipe/
│   │   │   ├─ RecipeCard.razor
│   │   │   └─ RecipeModal.razor
│   │   └─ Contact/
│   │       └─ ContactForm.razor
│   ├── Services/
│   │   ├─ IHttpApiService.cs
│   │   ├─ HttpApiService.cs
│   │   ├─ IRecipeService.cs
│   │   ├─ RecipeService.cs
│   │   ├─ IFavoriteService.cs
│   │   ├─ FavoriteService.cs
│   │   ├─ IThemeService.cs
│   │   └─ ThemeService.cs
│   ├── State/
│   │   └─ AppState.cs
│   ├── Models/
│   │   └─ Dtos/
│   ├── wwwroot/
│   │   ├─ css/
│   │   │   ├─ styles.css
│   │   │   └─ themes.css
│   │   └─ index.html
│   ├── App.razor
│   ├── Program.cs
│   └── RecipesApp.Client.csproj
│
├── RecipesApp.Server/                      # ASP.NET Core Backend
│   ├── Controllers/
│   │   ├─ RecipesController.cs
│   │   ├─ FavoritesController.cs
│   │   └─ ContactController.cs
│   ├── Services/
│   │   ├─ IRecipeService.cs
│   │   ├─ RecipeService.cs
│   │   ├─ IFavoriteService.cs
│   │   ├─ FavoriteService.cs
│   │   ├─ ITheMealDBService.cs
│   │   ├─ TheMealDBService.cs
│   │   ├─ IContactService.cs
│   │   └─ ContactService.cs
│   ├── Repositories/
│   │   ├─ IRecipeRepository.cs
│   │   ├─ RecipeRepository.cs
│   │   ├─ IFavoriteRepository.cs
│   │   └─ FavoriteRepository.cs
│   ├── Models/
│   │   ├─ Entities/
│   │   │   ├─ User.cs
│   │   │   ├─ Favorite.cs
│   │   │   └─ ContactMessage.cs
│   │   └─ Dtos/ (shared with client)
│   ├── Data/
│   │   ├─ ApplicationDbContext.cs
│   │   └─ Migrations/
│   ├── Auth/
│   │   └─ Auth0Settings.cs
│   ├── appsettings.json
│   ├── Program.cs
│   └─ RecipesApp.Server.csproj
│
├── RecipesApp.Shared/                      # Models & DTOs partagés
│   ├─ Dtos/
│   │   ├─ RecipeDto.cs
│   │   ├─ MealDto.cs
│   │   ├─ UserDto.cs
│   │   └─ ContactRequest.cs
│   └─ RecipesApp.Shared.csproj
│
├── RecipesApp.Tests/                       # Tests unitaires xUnit
│   ├─ Unit/
│   │   ├─ Services/
│   │   │   ├─ RecipeServiceTests.cs
│   │   │   ├─ FavoriteServiceTests.cs
│   │   │   ├─ TheMealDBServiceTests.cs
│   │   │   └─ ContactServiceTests.cs
│   │   └─ Repositories/
│   │       └─ RecipeRepositoryTests.cs
│   └─ RecipesApp.Tests.csproj
│
└── RecipesApp.Client.Tests/                # Tests Blazor BUnit
    ├─ Components/
    │   ├─ RecipeCardComponentTests.cs
    │   └─ NavBarComponentTests.cs
    ├─ Pages/
    │   ├─ HomePageTests.cs
    │   └─ SearchPageTests.cs
    └─ RecipesApp.Client.Tests.csproj
```

## 3.3 Dépendances NuGet

### Backend (RecipesApp.Server)
```xml
<!-- Core -->
<PackageReference Include="Microsoft.AspNetCore.App" Version="9.0" />

<!-- Auth0 -->
<PackageReference Include="Auth0.ManagementApi" Version="7.x" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="9.0" />

<!-- Database -->
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0" />

<!-- HTTP & Resilience -->
<PackageReference Include="Refit" Version="7.x" />
<PackageReference Include="Polly" Version="8.x" />

<!-- Logging & Utilities -->
<PackageReference Include="Serilog.AspNetCore" Version="8.x" />
<PackageReference Include="FluentValidation" Version="11.x" />
```

### Frontend (RecipesApp.Client)
```xml
<!-- Core Blazor -->
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0" />

<!-- Auth0 -->
<PackageReference Include="Auth0.OidcClient.Blazor" Version="1.x" />

<!-- Authentication -->
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.Authentication" Version="9.0" />
```

### Tests
```xml
<!-- xUnit -->
<PackageReference Include="xunit" Version="2.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.x" />

<!-- Mocking -->
<PackageReference Include="Moq" Version="4.x" />
<PackageReference Include="FluentAssertions" Version="6.x" />

<!-- BUnit (Blazor Components) -->
<PackageReference Include="bunit" Version="1.x" />

<!-- Database Testing -->
<PackageReference Include="Testcontainers" Version="3.x" />
<PackageReference Include="Testcontainers.MySql" Version="3.x" />
```

---

# 4. STRUCTURE DÉTAILLÉE DES FEATURES

## 4.1 Page: Accueil (Home.razor)

### État anonyme
```
┌─────────────────────────────────────┐
│  🍳 Découvrez des Recettes         │
├─────────────────────────────────────┤
│  Vous n'êtes pas connecté           │
│  [Connexion] [S'inscrire]           │
├─────────────────────────────────────┤
│  📌 6 RECETTES ALÉATOIRES           │
├─────────────────────────────────────┤
│  [Recette 1]  [Recette 2]  [...]   │
│  [Recette 4]  [Recette 5]  [...]   │
├─────────────────────────────────────┤
│  [🔄 Rafraîchir]                   │
└─────────────────────────────────────┘
```

**Fonctionnalités:**
- 6 recettes aléatoires de TheMealDB
- Bouton "Rafraîchir" pour nouvelles recettes
- Cards cliquables pour détails
- Liens Connexion/Inscription

### État connecté
```
┌─────────────────────────────────────┐
│  👋 Bienvenue, {Nom} !              │
├─────────────────────────────────────┤
│  📌 6 RECETTES ALÉATOIRES           │
├─────────────────────────────────────┤
│  [Recette 1] ❤️  [Recette 2] ❤️     │
│  [Recette 3] ❤️  [Recette 4] ❤️     │
├─────────────────────────────────────┤
│  [🔄 Rafraîchir]                   │
└─────────────────────────────────────┘
```

**Fonctionnalités supplémentaires:**
- Cœur cliquable pour ajouter/retirer favoris
- Salutation personnalisée

---

## 4.2 Page: Recherche (Search.razor)

```
┌────────────────────────────────────────┐
│  🔍 Trouver l'Inspiration              │
├────────────────────────────────────────┤
│  [🔍 Rechercher une recette...]        │
│  Catégorie: [Dropdown]                 │
│  Cuisine: [Dropdown]                   │
│  [Réinitialiser] [Chercher]           │
├────────────────────────────────────────┤
│  📌 12 résultats trouvés              │
├────────────────────────────────────────┤
│  [Recette 1] ❤️  [Recette 2] ❤️       │
│  [Recette 3] ❤️  [Recette 4] ❤️       │
├────────────────────────────────────────┤
│  [< Précédent] [1 2 3] [Suivant >]   │
└────────────────────────────────────────┘
```

**Fonctionnalités:**
- Recherche par nom (debounce 300ms)
- Filtres catégorie/cuisine
- Pagination 12 résultats/page
- Cœur pour ajouter aux favoris
- Cliquable pour modal détails

---

## 4.3 Page: Mes Recettes (MyRecipes.razor)

```
┌──────────────────────────────────────┐
│  Mes Recettes Favorites              │
├──────────────────────────────────────┤
│  Catégorie: [Dropdown]               │
│  Cuisine: [Dropdown]                 │
│  [Réinitialiser]                     │
├──────────────────────────────────────┤
│  ❤️ 8 recettes en favoris             │
├──────────────────────────────────────┤
│  [Recette 1] ❌  [Recette 2] ❌      │
│  [Recette 3] ❌  [Recette 4] ❌      │
├──────────────────────────────────────┤
│  [< Précédent] [1] [Suivant >]      │
└──────────────────────────────────────┘
```

**Fonctionnalités:**
- [Authorize] protection
- Affiche favoris de l'utilisateur
- Filtres par catégorie/cuisine
- Bouton ❌ pour supprimer
- Pagination
- Message si aucune recette

---

## 4.4 Page: Contact (Contact.razor) - TOUJOURS ACCESSIBLE

### URL: `/contact`
- **Pas de [Authorize]** - Page publique accessible anonymes ET connectés
- Contenu adapté selon l'état utilisateur

---

### État anonyme (Logout)
```
┌────────────────────────────────────┐
│  📧 Nous Contacter                 │
├────────────────────────────────────┤
│  Email: admin@recipehub.com        │
│  (statique - email du site)        │
├────────────────────────────────────┤
│  Sujet: [Dropdown]                 │
│  Message: [_________________]      │
│  [Envoyer]                         │
├────────────────────────────────────┤
│  ✅ Message envoyé!                │
└────────────────────────────────────┘
```

**Formulaire SIMPLIFIÉ (Logout):**
- ✅ Email statique: `admin@recipehub.com` (non modifiable)
- ✅ Sujet requis (Bug, Suggestion, Autre) - Dropdown
- ✅ Message 20-2000 caractères - TextArea
- ✅ Bouton [Envoyer]
- ❌ Pas de pré-remplissage utilisateur
- ❌ Pas de newsletter consent
- ❌ Pas de Nom/Email personnalisé
- **DB**: Enregistré avec `UserId = NULL` (contact anonyme)

---

### État connecté (Login)
```
┌────────────────────────────────────┐
│  👋 Bienvenue, {Prénom}!           │
│  📧 Nous Contacter                 │
├────────────────────────────────────┤
│  Nom: [Jean Dupont]                │
│  Email: [jean@email.com]           │
│  Sujet: [Dropdown]                 │
│  Message: [_________________]      │
│  ☐ J'accepte les emails             │
│  [Envoyer] [Annuler]              │
├────────────────────────────────────┤
│  ✅ Message envoyé!                │
└────────────────────────────────────┘
```

**Formulaire COMPLET (Login):**
- ✅ Nom pré-rempli depuis Auth0: `User.FindFirst(ClaimTypes.Name)?.Value`
- ✅ Email pré-rempli depuis Auth0: `User.FindFirst(ClaimTypes.Email)?.Value`
- ✅ Sujet requis (Bug, Suggestion, Autre) - Dropdown
- ✅ Message 20-2000 caractères - TextArea
- ✅ Checkbox newsletter consent
- ✅ Boutons [Envoyer] [Annuler]
- ✅ Salutation personnalisée en haut
- **DB**: Enregistré avec `UserId` de l'utilisateur (contact traçable)

---

## 4.5 Navigation globale

```
┌─────────────────────────────────────────┐
│  🍳 RecipeHub  [Recherche]  🌙  [Menu] │
└─────────────────────────────────────────┘
```

**Anonyme:**
- [Accueil] [Connexion] [Inscription]

**Connecté:**
- [Accueil] [Recherche] [Mes Recettes] [Contact] [Déconnexion]

**Tous:**
- Logo cliquable → Home
- Thème toggle (🌙/☀️)
- Recherche (caché si anonyme)

---

# 5. MODÈLE DE DONNÉES

## 5.1 Schéma MySQL

### 🎯 Architecture Hybrid: localStorage + Database

**Philosophie:**
- **localStorage**: Stocke le profil utilisateur complet (Name, Photo, Préférences, Token)
- **MySQL**: Stocke seulement les données critiques pour les relations (UserId, Auth0Id, Email)
- **Avantage**: Performance optimale + Persistance + Sécurité

**Flux utilisateur:**
1. User se login → Auth0 retourne ID token + profil
2. Profil complet stocké dans localStorage (rapide, persistant navigateur)
3. UserId créé/mappé en BD pour lier les favoris et contacts
4. localStorage utilisé partout dans l'app (pas de requête DB pour le profil)
5. Theme preference persisté en localStorage (pas en BD)

---

### Table: users (SIMPLIFIÉE)
```sql
CREATE TABLE users (
  user_id VARCHAR(36) PRIMARY KEY,
  auth0_id VARCHAR(255) UNIQUE NOT NULL,
  email VARCHAR(255) UNIQUE NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_auth0id (Auth0Id),
  INDEX idx_email (Email)
);
```

**Explications:**
- **user_id**: UUID (VARCHAR 36) - Pas d'auto-increment
- **auth0_id**: Identifiant unique Auth0 (UNIQUE)
- **email**: Email unique pour référence et recovery
- **created_at**: Timestamp de création du compte
- **Supprimé**: first_name, last_name, theme_preference → localStorage
- **Avantage**: Table ultra-légère, rapide, focus sur les relations

### Table: favorites
```sql
CREATE TABLE favorites (
  id INT PRIMARY KEY AUTO_INCREMENT,
  user_id INT NOT NULL,
  meal_id VARCHAR(50) NOT NULL,
  meal_name VARCHAR(255),
  meal_image_url VARCHAR(500),
  category VARCHAR(100),
  cuisine VARCHAR(100),
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  UNIQUE KEY unique_user_meal (user_id, meal_id),
  INDEX idx_user_id (user_id)
);
```

### Table: contact_messages
```sql
CREATE TABLE contact_messages (
  id INT PRIMARY KEY AUTO_INCREMENT,
  user_id INT NOT NULL,
  name VARCHAR(255),
  email VARCHAR(255),
  subject VARCHAR(100),
  message TEXT,
  newsletter_consent BOOLEAN DEFAULT FALSE,
  status ENUM('new', 'read', 'responded') DEFAULT 'new',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  INDEX idx_user_id (user_id)
);
```

## 5.2 Models C# Domain

### Entité User (SIMPLIFIÉE)
Représente le mapping Auth0 pour tracer les favoris et contacts

**Champs critiques (en BD):**
- **UserId**: UUID (VARCHAR 36) - Clé primaire
- **Auth0Id**: Identifiant unique d'Auth0 (UNIQUE)
- **Email**: Email pour référence et recovery
- **CreatedAt**: Timestamp du compte

**Profil utilisateur (en localStorage):**
- Name, Picture, Email (depuis Auth0 ID token)
- Theme preference (light/dark)
- Tokens (ID token, Access token)
- ⚠️ **Jamais en BD** - Stocké côté client

**Relations:**
- 1-N avec Favorites (UserId)
- 1-N avec ContactMessages (UserId nullable)

### Entité Favorite
Représente une recette marquée comme favorite par un utilisateur
- **Id**: Clé primaire auto-incrémentée
- **UserId**: Clé étrangère vers User (CASCADE DELETE)
- **MealId**: Identifiant de la recette TheMealDB (VARCHAR)
- **MealName/MealImageUrl**: Données en cache depuis l'API
- **Category/Cuisine**: Métadonnées recette
- **CreatedAt**: Date d'ajout aux favoris
- **Contrainte UNIQUE**: (UserId, MealId) - Pas de doublons

### Entité ContactMessage
Représente un message de contact (accessible TOUJOURS - login ET logout)

**Champs:**
- **Id**: Clé primaire auto-incrémentée
- **UserId**: Clé étrangère vers User
  - ✅ EN LOGIN: ID utilisateur (pour traçabilité)
  - ✅ EN LOGOUT: **NULL** (contact anonyme)
  - Foreign Key: CASCADE DELETE (si user supprimé)
- **Name**: Nom du contacteur
  - EN LOGIN: Pré-rempli depuis Auth0 (non modifiable mais lisible)
  - EN LOGOUT: Affiche "admin@recipehub.com" (statique)
- **Email**: Email du contacteur
  - EN LOGIN: Pré-rempli depuis Auth0 (non modifiable mais lisible)
  - EN LOGOUT: Toujours "admin@recipehub.com" (statique)
- **Subject**: Sujet du message (Bug, Suggestion, Autre)
  - Obligatoire - Dropdown
- **Message**: Contenu du message (20-2000 caractères)
  - Obligatoire - TextArea
- **NewsletterConsent**: Consentement newsletter (BOOLEAN)
  - EN LOGIN: Checkbox optionnelle
  - EN LOGOUT: **Toujours FALSE** (pas proposé)
- **Status**: "new", "read", "responded" (pour admin)
  - Défaut: "new"
- **CreatedAt**: Timestamp du message
- **Index sur UserId** pour requêtes rapides admin

**Distinction login vs logout en DB:**
```
LOGIN:  UserId=42, Name="Jean", Email="jean@mail.com", NewsletterConsent=TRUE
LOGOUT: UserId=NULL, Name="admin", Email="admin@recipehub.com", NewsletterConsent=FALSE
```

### DTOs (Data Transfer Objects)
**RecipeDto**: Utilisé pour communiquer recettes entre frontend/backend
- Id, Title, Description, ImageUrl, Category, Cuisine, Ingredients

**MealDto**: Réponse de l'API TheMealDB
- IdMeal, Name, Category, Cuisine, ImageUrl, Instructions

**IngredientDto**: Représente un ingrédient
- Name, Measure

**ContactRequest**: Payload du formulaire de contact
- Name, Email, Subject, Message, NewsletterConsent

---

# 6. INTÉGRATIONS EXTERNES

## 6.1 TheMealDB API

**Base URL**: `https://www.themealdb.com/api/json/v1/1`

### Endpoints utilisés

| Endpoint | Méthode | Usage |
|----------|---------|-------|
| `/random.php` | GET | Recette aléatoire |
| `/search.php?s={query}` | GET | Recherche par nom |
| `/list.php?c=list` | GET | Liste catégories |
| `/list.php?a=list` | GET | Liste cuisines |
| `/filter.php?c={category}` | GET | Filtre catégorie |
| `/filter.php?a={area}` | GET | Filtre cuisine |

### Réponse exemple
```json
{
  "meals": [
    {
      "idMeal": "52772",
      "strMeal": "Spaghetti Carbonara",
      "strCategory": "Pasta",
      "strArea": "Italian",
      "strMealThumb": "https://...",
      "strInstructions": "...",
      "strIngredient1": "Spaghetti",
      "strMeasure1": "500g"
    }
  ]
}
```

**Caractéristiques:**
- ✅ Gratuit, pas d'authentification
- ✅ Pas de limite documentée
- ✅ HTTPS + JSON
- ⚠️ Timeout recommandé: 10s

---

## 6.2 Auth0

### Configuration
1. Créer application "Regular Web Application"
2. Configurer:
   - Callback URL: `https://localhost:5001/callback`
   - Logout URL: `https://localhost:5001`
   - CORS Origins: `https://localhost:5001`
3. Récupérer: Domain, ClientId, ClientSecret

### Flow
```
User → [Login Button]
     → Auth0 /authorize?...
     → User login
     → Auth0 → Callback /callback?code=...
     → Backend: Exchange code → JWT
     → Set Cookie + Redirect /home
```

---

# 8. PLAN D'IMPLÉMENTATION

**Deadline**: 8 mars 2026
**À compléter avant**: 8 mars 2026
**Heures estimées**: ~95 heures total

## 8.1 Phase 1: Backend Setup

### Sprint 1: Project Setup (4h)
- [ ] Créer solution Blazor WebAssembly
- [ ] Créer projets tests
- [ ] Ajouter NuGet dependencies
- [ ] Git setup + premier commit

**Checklist:**
- [ ] Solution compile ✅
- [ ] Projects created ✅
- [ ] Dependencies added ✅

### Sprint 2: Database & EF Core (4h)
- [ ] Créer ApplicationDbContext
- [ ] Définir Entities (User, Favorite, ContactMessage)
- [ ] Première migration
- [ ] MySQL setup + test connexion
- [ ] Créer DTOs

**Checklist:**
- [ ] DB créée ✅
- [ ] Migrations OK ✅
- [ ] Entities defined ✅

### Sprint 3: Auth0 Setup (4h)
- [ ] Configurer Auth0 dashboard
- [ ] Créer Auth0Settings class
- [ ] Configurer appsettings.json
- [ ] Implémenter Program.cs Auth0
- [ ] Tester Auth0 flow basique

**Checklist:**
- [ ] Auth0 app créé ✅
- [ ] Credentials stockés ✅
- [ ] Program.cs OK ✅

### Sprint 4: Services & Repositories (5h)
- [ ] Créer interfaces + implémentations Repositories
- [ ] Créer interfaces Services
- [ ] Implémenter Services (logique métier)
- [ ] Enregistrer DI

**Checklist:**
- [ ] Repositories OK ✅
- [ ] Services skeleton OK ✅
- [ ] DI OK ✅

### Sprint 5: TheMealDB & Controllers (5h)
- [ ] Implémenter TheMealDBService
- [ ] Ajouter Polly resilience
- [ ] Créer Controllers
- [ ] Implémenter endpoints
- [ ] Tester avec Postman

**Checklist:**
- [ ] TheMealDB service OK ✅
- [ ] Controllers working ✅
- [ ] API testable ✅
- [ ] **Phase 1 DONE ✅**

---

## 8.2 Phase 2: Frontend Blazor

### Sprint 6: Frontend Setup (4h)
- [ ] Configurer Program.cs Blazor
- [ ] Créer Layout principal
- [ ] Créer NavBar component
- [ ] Créer ThemeToggle component
- [ ] CSS Light/Dark themes

**Checklist:**
- [ ] Blazor compiles ✅
- [ ] Layout OK ✅
- [ ] Theme working ✅

### Sprint 7: Frontend Services (4h)
- [ ] Créer IHttpApiService + impl
- [ ] Créer IRecipeService + impl
- [ ] Créer IFavoriteService + impl
- [ ] Créer IThemeService + impl
- [ ] Caching setup

**Checklist:**
- [ ] Services created ✅
- [ ] HTTP calls working ✅
- [ ] Caching OK ✅

### Sprint 8: Home & Search Pages (5h)
- [ ] Créer Home.razor page
- [ ] Créer Search.razor page
- [ ] Créer RecipeCard component
- [ ] Créer RecipeModal component
- [ ] Tests manuels

**Checklist:**
- [ ] Home affiche recettes ✅
- [ ] Search working ✅
- [ ] RecipeCard clickable ✅

### Sprint 9: Favoris & Contact (5h)
- [ ] Créer MyRecipes.razor page
- [ ] Créer Contact.razor page
- [ ] Créer ContactForm component
- [ ] Implémenter IContactService
- [ ] Validation forms

**Checklist:**
- [ ] MyRecipes OK ✅
- [ ] Contact form OK ✅
- [ ] Backend POST OK ✅

### Sprint 10: Auth0 Frontend (4h)
- [ ] Créer Login page
- [ ] Créer Logout logic
- [ ] Implémenter Callback
- [ ] Tester Auth0 flow complet
- [ ] Protéger pages [Authorize]

**Checklist:**
- [ ] Login working ✅
- [ ] Logout working ✅
- [ ] JWT stored ✅
- [ ] **Phase 2 DONE ✅**

---

## 8.3 Phase 3: Tests

### Sprint 11: xUnit Services Tests (5h)
- [ ] RecipeServiceTests
- [ ] FavoriteServiceTests
- [ ] TheMealDBServiceTests
- [ ] ContactServiceTests
- [ ] > 30 tests

**Checklist:**
- [ ] 30+ tests ✅
- [ ] All GREEN ✅
- [ ] Coverage > 70% ✅

### Sprint 12: xUnit Repository Tests (4h)
- [ ] RecipeRepositoryTests
- [ ] FavoriteRepositoryTests
- [ ] Testcontainers MySQL
- [ ] Transaction tests

**Checklist:**
- [ ] 20+ tests ✅
- [ ] DB testing OK ✅
- [ ] All GREEN ✅

### Sprint 13: BUnit Components (5h)
- [ ] RecipeCardComponentTests
- [ ] NavBarComponentTests
- [ ] ThemeToggleComponentTests
- [ ] HomePageTests
- [ ] > 15 tests

**Checklist:**
- [ ] 15+ tests ✅
- [ ] BUnit renders OK ✅
- [ ] All GREEN ✅

### Sprint 14: BUnit Pages (5h)
- [ ] SearchPageTests
- [ ] ContactPageTests
- [ ] MyRecipesPageTests
- [ ] Integration tests

**Checklist:**
- [ ] 15+ tests ✅
- [ ] Complex flows tested ✅
- [ ] All GREEN ✅

### Sprint 15: Coverage & Reports (4h)
- [ ] Run coverage analysis
- [ ] Target > 75%
- [ ] Fix gaps
- [ ] Coverage report
- [ ] Test documentation

**Checklist:**
- [ ] Coverage > 75% ✅
- [ ] All tests GREEN ✅
- [ ] Docs updated ✅
- [ ] **Phase 3 DONE ✅**

---

## 8.4 Phase 4: Finalisation

### Sprint 16: Documentation (4h)
- [ ] XML comments all public methods
- [ ] README.md complet
- [ ] ARCHITECTURE.md
- [ ] TESTING.md
- [ ] API docs

**Checklist:**
- [ ] All methods documented ✅
- [ ] README complete ✅
- [ ] Architecture clear ✅

### Sprint 17: Polish & Bug Fixes (5h)
- [ ] Full workflow test
- [ ] Responsive design check
- [ ] Dark/Light theme polish
- [ ] Error handling review
- [ ] Performance optimization

**Checklist:**
- [ ] No console errors ✅
- [ ] Responsive OK ✅
- [ ] Performance good ✅
- [ ] UX smooth ✅

### Sprint 18: Final Testing (4h)
- [ ] Release build
- [ ] Test complete workflow
- [ ] Validate requirements
- [ ] Bug fixes
- [ ] Ready for demo

**Checklist:**
- [ ] Release builds ✅
- [ ] All workflows tested ✅
- [ ] Requirements met ✅
- [ ] No critical bugs ✅

### Sprint 19: Presentation (3h)
- [ ] Create presentation slides
- [ ] Prepare demo
- [ ] Git history clean
- [ ] Push final version
- [ ] Test all links

**Checklist:**
- [ ] Presentation ready ✅
- [ ] Demo works ✅
- [ ] Docs complete ✅
- [ ] **READY TO SUBMIT ✅**

---

# 9. TESTS STRATEGY

## 9.1 Tests Unitaires (xUnit)

### Services à tester:
- **RecipeServiceTests**: GetRandom, Search, GetCategories, GetCuisines
- **FavoriteServiceTests**: Add, Remove, Get, IsFavorite
- **TheMealDBServiceTests**: API calls, Polly resilience, error handling
- **ContactServiceTests**: Message validation, storage

### Repositories à tester:
- **RecipeRepositoryTests**: CRUD operations, queries
- **FavoriteRepositoryTests**: Add/Remove, GetByUser, uniqueness

**Objectif**: > 30 tests, coverage > 70%

## 9.2 Tests Composants (BUnit)

### Components à tester:
- **RecipeCard**: Display, favorite toggle, modal
- **NavBar**: Menu visibility, theme toggle
- **ThemeToggle**: Dark/Light switch, persistence
- **RecipeModal**: Details display, close action

### Pages à tester:
- **Home**: Random recipes display, refresh
- **Search**: Query input, results, pagination
- **MyRecipes**: Favorites display, delete
- **Contact**: Form validation, submit

**Objectif**: > 15 tests, all GREEN ✅

---

# 10. CHECKLIST FINALE

## Functional Requirements
- [ ] ✅ Application Blazor WebAssembly créée
- [ ] ✅ Affiche données API TheMealDB
- [ ] ✅ Utilisateur peut se connecter (Auth0)
- [ ] ✅ Utilisateur peut créer favoris
- [ ] ✅ Utilisateur peut modifier/voir favoris
- [ ] ✅ Utilisateur peut supprimer favoris
- [ ] ✅ Pages protégées par [Authorize]

## Testing
- [ ] xUnit tests > 30 tests
- [ ] BUnit tests > 15 tests
- [ ] Code coverage > 75%
- [ ] All tests GREEN ✅

## Documentation
- [ ] README.md complet
- [ ] ARCHITECTURE.md explicite
- [ ] TESTING.md instructions
- [ ] XML comments sur public APIs
- [ ] Inline comments pour logique complexe

## Code Quality
- [ ] Pas d'erreurs compilation
- [ ] Pas de warnings critiques
- [ ] Code formaté (C# standards)
- [ ] SOLID principles respectés
- [ ] DRY code

## Presentation
- [ ] Demo workflow complet
- [ ] Expliquer tech stack
- [ ] Montrer tests
- [ ] Architecture slide

## Submission
- [ ] Git history clean
- [ ] All code committed
- [ ] README accessible
- [ ] Demo works ✅

---

# 11. AMÉLIORATIONS FUTURES (Roadmap Post-Exam)

## 11.1 Dashboard Administrateur

### Page: Admin Dashboard (`/admin/dashboard`)

**Accès**: [Authorize(Roles = "Admin")] - Role-based access control

```
┌──────────────────────────────────────────┐
│  👨‍💼 Dashboard Administrateur             │
├──────────────────────────────────────────┤
│  📊 STATISTIQUES                         │
│  • Utilisateurs: 42                      │
│  • Favoris total: 156                    │
│  • Messages contact: 18 (5 non-lus)     │
├──────────────────────────────────────────┤
│  💬 MESSAGES RÉCENTS (non-lus)          │
│  ┌────────────────────────────────────┐ │
│  │ De: Jean (jean@email.com)          │ │
│  │ Sujet: Bug - Images ne charge pas  │ │
│  │ Date: 2026-02-18                   │ │
│  │ [Marquer lus] [Répondre]          │ │
│  └────────────────────────────────────┘ │
│  ┌────────────────────────────────────┐ │
│  │ De: Marie (marie@email.com)        │ │
│  │ Sujet: Suggestion - Dark mode      │ │
│  │ Date: 2026-02-17                   │ │
│  │ [Marquer lus] [Répondre]          │ │
│  └────────────────────────────────────┘ │
├──────────────────────────────────────────┤
│  [Voir tous les messages] [Exportez CSV]│
└──────────────────────────────────────────┘
```

### Fonctionnalités Dashboard:
- **Vue tous les messages de contact** avec filtres:
  - Status (new, read, responded)
  - Date range
  - Recherche par email/nom
- **Tableau de bord statistiques**:
  - Nombre utilisateurs actifs
  - Favoris les plus populaires
  - Messages non-lus
- **Actions sur messages**:
  - Marquer comme lus
  - Répondre directement
  - Supprimer
  - Exporter en CSV
- **Gestion utilisateurs** (optionnel):
  - Voir tous les users
  - Activer/désactiver
  - Voir leurs favoris

### Tables/Views requises:
- `message_admin_view`: Vue avec COUNT messages par status
- `popular_recipes_view`: Recettes les plus favoritées

---

## 11.2 Autres améliorations futures

**Phase 2** (Après examen):
- ✅ Ajouter Admin role en Auth0
- ✅ Implémenter Dashboard admin
- ✅ Notification email sur nouveau message
- ✅ Système de réponses automatiques
- ✅ Export données (CSV, PDF)
- ✅ Graphiques statistiques (Chart.js)

**Phase 3** (Long terme):
- ✅ Créer ses propres recettes
- ✅ Partager recettes avec d'autres users
- ✅ Commentaires sur recettes
- ✅ Système de notation (stars)
- ✅ Recettes sauvegardées (meal plans)
- ✅ Recherche par nutrition (API externe)

---

# 📞 CONTACT & SUPPORT

For issues or questions about this project documentation:
- Check README.md first
- Review ARCHITECTURE.md for design patterns
- Check test files for usage examples

---

**✅ PROJET COMPLET - READY TO START!**

**Date de création**: 08-02-2026
**Deadline de remise**: 8 mars 2026
**Heures estimées**: ~95 heures total
**Commencez maintenant! 🚀**
