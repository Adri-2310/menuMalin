# 🍽️ menuMalin - Recipe Discovery & Favorites Management App

> Une application web moderne pour découvrir des recettes, gérer vos favoris, et rester en contact

**Version:** 1.0.0
**Date:** Février 2026
**Statut:** Phase 3 Complétée (78.9% - 15/19 sprints)

---

## 🌟 Vue d'ensemble

menuMalin est une application web complète construite avec **Blazor WebAssembly** (frontend) et **ASP.NET Core** (backend), permettant aux utilisateurs de :

- 🔍 **Découvrir des recettes** via TheMealDB (6 aléatoires, recherche, filtres)
- ❤️ **Gérer ses favoris** (ajouter, supprimer, lister)
- 🔐 **S'authentifier** via Auth0 (SSO)
- 📧 **Envoyer des messages** via formulaire de contact
- 🎨 **Basculer en dark/light mode**
- 📱 **Interface responsive** (mobile, tablet, desktop)

---

## 🚀 Démarrage Rapide

### Prérequis

- **.NET 9 SDK** - [Télécharger](https://dotnet.microsoft.com/download)
- **MySQL 8.0+** - [Docker recommandé](https://www.docker.com)
- **Git**
- **Auth0 Account** - [Créer gratuitement](https://auth0.com)

### Installation locale (5 minutes)

```bash
# 1. Cloner le repository
git clone https://github.com/yourusername/menumalin.git
cd menumalin

# 2. Configurer la base de données (Docker)
docker run --name mysql-menumalin \
  -e MYSQL_ROOT_PASSWORD=root \
  -e MYSQL_DATABASE=menumalin_db \
  -e MYSQL_USER=menumalin_user \
  -e MYSQL_PASSWORD=password \
  -p 3306:3306 \
  -d mysql:8.0

# 3. Appliquer les migrations
cd menuMalin.Server
dotnet ef database update
cd ..

# 4. Terminal 1 - Lancer le backend
cd menuMalin.Server
dotnet run
# → API disponible sur http://localhost:5266/api

# 5. Terminal 2 - Lancer le frontend
cd menuMalin
dotnet run
# → App disponible sur https://localhost:7777
```

> 📌 **Premier accès**: Cliquez sur "Login with Auth0" → Complétez l'inscription → Explorez!

---

## 📚 Documentation

| Document | Description |
|----------|-------------|
| **[ARCHITECTURE.md](docs/ARCHITECTURE.md)** | Architecture système, data flow, patterns |
| **[API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md)** | Endpoints API détaillés avec exemples |
| **[CONTRIBUTING.md](docs/CONTRIBUTING.md)** | Guide de contribution et code style |
| **[TESTING_REPORT.md](docs/TESTING_REPORT.md)** | Couverture de tests (41 tests, 100% pass) |
| **[PROGRESS.md](docs/PROGRESS.md)** | Progression du projet (15/19 sprints) |
| **[CHANGELOG.md](docs/CHANGELOG.md)** | Historique des features implémentées |

---

## 🏗️ Architecture

### Stack Technologique

```
┌─────────────────────────────────────┐
│  FRONTEND (Blazor WebAssembly)      │
│  - Pages Razor                      │
│  - Composants réactifs              │
│  - Auth0 OIDC                       │
│  - LocalStorage (profil user)       │
└─────────────────────────────────────┘
           ↓ HTTP API ↓
┌─────────────────────────────────────┐
│  BACKEND (ASP.NET Core 9)           │
│  - Controllers REST                 │
│  - Services (Clean Architecture)    │
│  - Repositories (EF Core)           │
│  - JWT Bearer Auth                  │
└─────────────────────────────────────┘
           ↓ External APIs ↓
┌─────────────────────────────────────┐
│  EXTERNAL SERVICES                  │
│  - TheMealDB (Recipe data)          │
│  - Auth0 (Authentication)           │
│  - MySQL Database                   │
└─────────────────────────────────────┘
```

### Ports de Développement

| Service | Port | URL |
|---------|------|-----|
| **Frontend** | 7777 | https://localhost:7777 |
| **Backend API** | 5266 | http://localhost:5266/api |
| **MySQL** | 3306 | localhost:3306 |

---

## 🧪 Tests

**41 tests** avec taux de réussite **100%** ✅

```bash
# Exécuter tous les tests
dotnet test

# Tests spécifiques
dotnet test --filter "Category=Services"
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=EdgeCase"

# Avec couverture de code
dotnet test /p:CollectCoverage=true /p:CoverageFormat=json

# Watch mode (re-exécute à chaque changement)
dotnet watch test
```

**Couverture par catégorie:**
- ✅ Services Frontend: **100%** (RecipeService, FavoriteService, ContactService, HttpApiService)
- ✅ Services Backend: **100%** (Tous les endpoints testés)
- ✅ Integration Tests: **9 tests** (Frontend ↔ Backend, Cross-service)
- ✅ Edge Cases: **9 tests** (Erreurs, concurrence, données volumineuses)

Voir [TESTING_REPORT.md](docs/TESTING_REPORT.md) pour les détails complets.

---

## 📡 API Endpoints

### Publics (sans authentification)
```
GET    /api/recipes/random           - 6 recettes aléatoires
GET    /api/recipes/search?query=    - Rechercher par nom
GET    /api/recipes/{mealId}         - Détails recette
GET    /api/recipes/categories/list  - Toutes catégories
GET    /api/recipes/areas/list       - Toutes zones/cuisines
GET    /api/recipes/filter/category  - Filtre par catégorie
GET    /api/recipes/filter/area      - Filtre par zone
POST   /api/contact                  - Envoyer message contact
GET    /api/auth/health              - Health check
```

### Protégés [Authorize]
```
GET    /api/favorites                - Récupérer favoris
POST   /api/favorites                - Ajouter favori
DELETE /api/favorites/{recipeId}     - Supprimer favori
GET    /api/favorites/{recipeId}/exists - Vérifier si favori
GET    /api/auth/me                  - Infos utilisateur connecté
```

> 📖 Voir [API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md) pour les réponses JSON complètes

---

## 🗂️ Structure du Projet

```
menumalin/
│
├── menuMalin/                          # Frontend (Blazor WASM)
│   ├── Pages/
│   │   ├── Index.razor                 # Accueil (6 recettes aléatoires)
│   │   ├── Search.razor                # Recherche avancée [Authorize]
│   │   ├── MyRecipes.razor             # Mes favoris [Authorize]
│   │   ├── Contact.razor               # Formulaire contact (public)
│   │   └── Authentication.razor        # Flux Auth0
│   │
│   ├── Components/
│   │   ├── Recipe/
│   │   │   ├── RecipeCard.razor        # Carte recette
│   │   │   ├── RecipeGrid.razor        # Grille 3 colonnes + pagination
│   │   │   ├── RecipeModal.razor       # Modal détails
│   │   │   └── SearchBar.razor         # Barre recherche
│   │   └── Contact/
│   │       └── ContactForm.razor       # Formulaire réutilisable
│   │
│   ├── Services/
│   │   ├── RecipeService.cs            # Accès API recettes
│   │   ├── FavoriteService.cs          # Gestion favoris LocalStorage
│   │   ├── ContactService.cs           # Envoi messages
│   │   ├── HttpApiService.cs           # Communication API
│   │   ├── LocalStorageService.cs      # Profil utilisateur
│   │   └── ThemeService.cs             # Dark/light mode
│   │
│   ├── Layouts/
│   │   └── MainLayout.razor            # Layout principal
│   │
│   └── wwwroot/
│       ├── app.css                     # Styles globaux
│       ├── app.js                      # Scripts frontend
│       └── appsettings.json            # Config Auth0
│
├── menuMalin.Server/                   # Backend (ASP.NET Core)
│   ├── Controllers/
│   │   ├── RecipesController.cs        # 6 endpoints recettes
│   │   ├── FavoritesController.cs      # 4 endpoints favoris [Authorize]
│   │   ├── ContactController.cs        # 1 endpoint contact
│   │   └── AuthController.cs           # Auth endpoints
│   │
│   ├── Services/
│   │   ├── IRecipeService.cs           # Interface
│   │   ├── RecipeService.cs            # Logique métier
│   │   ├── IFavoriteService.cs         # Interface
│   │   ├── FavoriteService.cs          # Logique métier
│   │   ├── IContactService.cs          # Interface
│   │   ├── ContactService.cs           # Logique métier
│   │   ├── ITheMealDBService.cs        # Interface TheMealDB
│   │   └── TheMealDBService.cs         # Appels API TheMealDB
│   │
│   ├── Repositories/
│   │   ├── IRecipeRepository.cs        # Interface
│   │   ├── RecipeRepository.cs         # EF Core
│   │   ├── IFavoriteRepository.cs      # Interface
│   │   └── FavoriteRepository.cs       # EF Core
│   │
│   ├── Models/
│   │   ├── Entities/
│   │   │   ├── Recipe.cs               # Entity DB
│   │   │   ├── User.cs                 # Entity DB
│   │   │   ├── Favorite.cs             # Entity DB
│   │   │   └── ContactMessage.cs       # Entity DB
│   │   └── DTOs/
│   │       ├── RecipeDto.cs            # Transfer object
│   │       ├── MealDto.cs              # TheMealDB response
│   │       ├── CategoryResponse.cs     # Catégories
│   │       └── AreaResponse.cs         # Zones/cuisines
│   │
│   ├── Data/
│   │   └── ApplicationDbContext.cs     # EF Core DbContext
│   │
│   └── Properties/
│       └── launchSettings.json
│
├── menuMalin.Shared/                   # Code partagé
│   └── Models/
│       ├── Entities/
│       └── Dtos/
│
├── menuMalin.Tests/                    # Tests Backend (xUnit)
│   ├── Services/
│   │   ├── RecipeServiceTests.cs       # 8 tests
│   │   ├── FavoriteServiceTests.cs     # 5 tests
│   │   ├── ContactServiceTests.cs      # 5 tests
│   │   └── HttpApiServiceTests.cs      # 5 tests
│   │
│   ├── Integration/
│   │   └── ServiceIntegrationTests.cs  # 9 tests
│   │
│   └── EdgeCases/
│       └── EdgeCaseTests.cs            # 9 tests
│
├── menuMalin.Client.Tests/             # Tests Frontend (BUnit)
│   └── [Configuration ready]
│
└── docs/                               # Documentation
    ├── ARCHITECTURE.md                 # Vue d'ensemble système
    ├── API_DOCUMENTATION.md            # Endpoints et réponses
    ├── CONTRIBUTING.md                 # Guide contribution
    ├── TESTING_REPORT.md               # Couverture tests
    ├── PROGRESS.md                     # Progression sprints
    └── CHANGELOG.md                    # Historique features
```

---

## 🔐 Authentification

### Auth0 OIDC Flow

1. **Frontend** → Redirects to Auth0 login
2. **Auth0** → Issues JWT token
3. **Frontend** → Stores token + redirects to callback
4. **Backend API** → Validates JWT Bearer token
5. **Protected Routes** → Require [Authorize] attribute

### Configuration

**Frontend** (`menuMalin/wwwroot/appsettings.json`):
```json
{
  "auth0": {
    "authority": "https://your-domain.auth0.com",
    "clientId": "your-client-id",
    "redirectUri": "https://localhost:7777/authentication/login-callback"
  }
}
```

**Backend** (`menuMalin.Server/appsettings.json`):
```json
{
  "Auth0": {
    "Domain": "your-domain.auth0.com",
    "ClientId": "your-client-id"
  }
}
```

---

## 🐛 Troubleshooting

### "Hosting failed to start on port 7777"
```bash
# Port déjà utilisé? Vérifiez/changez dans launchSettings.json
# Ou tuez le processus:
netstat -ano | findstr :7777  # Windows
lsof -i :7777                  # Mac/Linux
```

### "Connection refused on port 3306"
```bash
# MySQL pas lancé? Démarrez le container Docker:
docker start mysql-menumalin

# Ou créez un nouveau container:
docker run --name mysql-menumalin ... (voir Installation)
```

### "Auth0 login redirect loop"
```bash
# Vérifiez dans Auth0 Dashboard:
1. Application Settings → Allowed Callback URLs
2. Applications → Allowed Logout URLs
3. Match avec localhost:7777 dans les deux configs
```

### "Tests failing after code changes"
```bash
# Reconstruisez et réexécutez:
dotnet clean
dotnet build
dotnet test
```

---

## 🎯 Phases du Projet

| Phase | Sprints | Statut | Description |
|-------|---------|--------|-------------|
| **Phase 1** | 1-5 | ✅ 100% | Backend setup (DB, Auth, Services, Controllers) |
| **Phase 2** | 6-10 | ✅ 100% | Frontend Blazor (Pages, Components, Services) |
| **Phase 3** | 11-15 | ✅ 100% | Tests (41 tests, 100% pass rate) |
| **Phase 4** | 16-19 | ⏳ En cours | Finalisation (Documentation, Polish, Release) |

**Completion:** 78.9% (15/19 sprints)

---

## 🤝 Contribuer

1. **Fork** le repository
2. **Créer une branch** (`git checkout -b feature/amazing-feature`)
3. **Commiter** les changements (`git commit -m 'feat: Add amazing feature'`)
4. **Push** vers la branch (`git push origin feature/amazing-feature`)
5. **Ouvrir une Pull Request**

Voir [CONTRIBUTING.md](docs/CONTRIBUTING.md) pour les **commit guidelines** et **code style**.

---

## 📋 Checklist Avant Production

- [ ] Base de données MySQL configurée et migrée
- [ ] Credentials Auth0 valides et configurés
- [ ] Tous les tests passent (`dotnet test`)
- [ ] Build en release mode (`dotnet publish`)
- [ ] Frontend WASM sur CDN (optionnel)
- [ ] HTTPS configuré en production
- [ ] Rate limiting configuré
- [ ] Logs et monitoring en place
- [ ] Documentation à jour
- [ ] Security headers configurés

---

## 📊 Statistiques du Projet

| Métrique | Valeur |
|----------|--------|
| **Sprints Complétés** | 15/19 (78.9%) |
| **Tests** | 41 xUnit (100% pass rate) |
| **Endpoints API** | 13 (publics + protégés) |
| **Pages Blazor** | 8 |
| **Composants** | 7 |
| **Services** | 8 |
| **Controllers** | 4 |
| **Tables DB** | 4 |
| **Dépendances NuGet** | 15+ |
| **Documentation** | 6 fichiers |

---

## 🔧 Stack Technique Détaillé

### Frontend
- **Framework:** Blazor WebAssembly
- **Language:** C# + Razor
- **Authentication:** Auth0 OIDC
- **Storage:** LocalStorage (Blazored.LocalStorage)
- **Styling:** Bootstrap 5 + CSS custom

### Backend
- **Framework:** ASP.NET Core 9
- **Database:** Entity Framework Core + MySQL
- **Authentication:** JWT Bearer (Auth0)
- **Patterns:** Clean Architecture, Repository Pattern, Service Pattern
- **HTTP Client:** HttpClient + Polly (retry policies)

### Testing
- **Framework:** xUnit v2.9.2
- **Mocking:** Moq v4.20.72
- **Assertions:** FluentAssertions v8.8.0
- **Component Testing:** BUnit (ready)

### External APIs
- **TheMealDB:** Recipe data (random, search, filter)
- **Auth0:** Authentication & user management

---

## 📞 Support & Questions

- **📖 Documentation:** Voir le dossier `/docs`
- **🐛 Issues:** [GitHub Issues](https://github.com/yourusername/menumalin/issues)
- **💬 Discussions:** [GitHub Discussions](https://github.com/yourusername/menumalin/discussions)
- **📧 Email:** Envoyer via formulaire /contact

---

## 📄 Licence

Ce projet est sous licence **[MIT](LICENSE)**

Vous êtes libre de:
- ✅ Utiliser commercialement
- ✅ Modifier
- ✅ Distribuer
- ✅ Utiliser en privé

Avec la condition de garder la mention de licence.

---

## 🎉 Remerciements

Merci à:
- **TheMealDB** pour l'API de recettes gratuite
- **Auth0** pour l'authentification SSO
- **Microsoft** pour .NET et Blazor
- **Tous les contributeurs** et testeurs

---

**Dernière mise à jour:** 24 février 2026
**Auteur:** menuMalin Team
**Version:** 1.0.0

---

Prêt à découvrir des recettes? [Démarrer maintenant](https://localhost:7777) 🚀
