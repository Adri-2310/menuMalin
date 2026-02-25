# 🍳 MenuMalin - Application de Recettes

![Status](https://img.shields.io/badge/Status-Development-blue)
![C#](https://img.shields.io/badge/C%23-9-blue)
![.NET](https://img.shields.io/badge/.NET-9-blue)
![License](https://img.shields.io/badge/License-MIT-green)

**Application Blazor WebAssembly** de découverte et gestion de recettes de cuisine avec authentification Auth0, intégration API TheMealDB, et gestion des favoris personnels.

---

## 📋 Table des matières

- [À propos](#à-propos)
- [Démarrage rapide](#démarrage-rapide)
- [Fonctionnalités](#fonctionnalités)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Installation](#installation)
- [Configuration](#configuration)
- [Tests](#tests)
- [Roadmap](#roadmap)
- [Contribution](#contribution)
- [License](#license)

---

## 🎯 À propos

**MenuMalin** est une application web moderne qui permet aux utilisateurs de:
- 🔍 Découvrir des recettes aléatoires gratuitement
- ❤️ Créer et gérer leurs favoris personnels (authentification requise)
- 🔎 Rechercher par nom, catégorie ou cuisine
- 📧 Envoyer des messages de contact (accessible anonyme et connecté)
- 🌙 Basculer entre thème clair et sombre (persistant)

### Données sources
- **Recettes**: [TheMealDB API](https://www.themealdb.com/) - Gratuit, 300+ recettes
- **Authentification**: [Auth0](https://auth0.com/) - Gestion sécurisée des utilisateurs
- **Base de données**: MySQL - Stockage des favoris et messages

---

## 🚀 Démarrage rapide

```bash
# 1. Cloner le repository
git clone https://github.com/yourusername/RecipeHub.git
cd RecipeHub

# 2. Setup Backend
cd RecipesApp.Server
dotnet restore
# Configurer appsettings.json (voir Configuration)

# 3. Setup Frontend
cd ../RecipesApp.Client
dotnet restore

# 4. Exécuter les migrations DB
cd ../RecipesApp.Server
dotnet ef database update

# 5. Démarrer l'application
# Terminal 1 - Backend
dotnet run --project RecipesApp.Server

# Terminal 2 - Frontend
dotnet run --project RecipesApp.Client

# 6. Accéder à l'app
# Ouvrir: https://localhost:5001
```

---

## ✨ Fonctionnalités

### 🏠 Page d'accueil
- **Anonyme**: 6 recettes aléatoires, bouton rafraîchir
- **Connecté**: Idem + cœurs pour ajouter aux favoris, salutation personnalisée

### 🔍 Recherche avancée (login requis)
- Recherche par nom (temps réel, debounce)
- Filtres: Catégorie, Cuisine
- Pagination 12 résultats/page
- Résultats cliquables pour détails

### ❤️ Mes Recettes (login requis)
- Affichage tous les favoris personnels
- Filtres et tri
- Bouton supprimer
- Pagination

### 📧 Page Contact
- **Anonyme**: Email admin statique + Sujet + Message
- **Connecté**: Nom/Email pré-remplis Auth0 + Newsletter consent
- Validation client + serveur
- Toast notifications

### 🌙 Thème Dark/Light
- Toggle en navbar
- Persistance LocalStorage
- Changement instantané sans rechargement

### 🔐 Authentification
- Login/Logout via Auth0
- Protection des pages avec [Authorize]
- Récupération automatique infos utilisateur

---

## 🏗️ Architecture

### 🎯 Approche Hybrid (localStorage + Database)

**Philosophie:**
- **localStorage**: Profil utilisateur complet (Name, Picture, Token, Préférences)
- **MySQL**: Données critiques pour les relations (UserId, Auth0Id, Email)
- **Avantage**: Performance optimale ⚡ + Persistance 💾 + Sécurité 🔒

**Flux:**
1. User login → Auth0 retourne ID token
2. Profil stocké dans **localStorage** (rapide, persistant navigateur)
3. UserId mappé en **BD** pour lier favoris/contacts
4. App utilise localStorage partout (pas de requête DB pour le profil)
5. Theme préférence persisté en localStorage aussi

### Layers

```
┌──────────────────────────────────────────────────────┐
│  Frontend: Blazor WebAssembly (C#)                   │
│  ├─ localStorage: Profil User, Tokens, Preferences  │
│  ├─ Services: HTTP, Recipe, Favorite, Theme         │
│  └─ Pages: Home, Search, MyRecipes, Contact         │
└─────────────────┬──────────────────────────────────┘
                  │ HTTP REST API
                  ▼
┌──────────────────────────────────────────────────────┐
│  Backend: ASP.NET Core (C#)                          │
│  ├─ Controllers: Recipes, Favorites, Contact        │
│  ├─ Services: Business logic                        │
│  ├─ Repositories: Data access (User, Favorite, etc) │
│  └─ EF Core + MySQL: Données critiques              │
└─────────────────┬──────────────────────────────────┘
                  │
          ┌───────┼───────┐
          ▼       ▼       ▼
       MySQL   Auth0   TheMealDB
        DB      API      API
```

### Structure projet

```
RecipesApp/
├── RecipesApp.Client/              # Blazor WebAssembly Frontend
│   ├── Pages/                      # Home, Search, MyRecipes, Contact
│   ├── Components/                 # RecipeCard, NavBar, ThemeToggle
│   ├── Services/                   # Recipe, Favorite, Theme, HttpApi
│   └── wwwroot/                    # CSS (Light/Dark themes)
│
├── RecipesApp.Server/              # ASP.NET Core Backend
│   ├── Controllers/                # API endpoints
│   ├── Services/                   # Business logic
│   ├── Repositories/               # Data access
│   ├── Models/                     # Entities & DTOs
│   └── Data/                       # DbContext, Migrations
│
├── RecipesApp.Shared/              # Models & DTOs partagés
│
├── RecipesApp.Tests/               # Tests unitaires (xUnit)
│
└── RecipesApp.Client.Tests/        # Tests composants (BUnit)
```

---

## 🛠️ Tech Stack

### Frontend
- **Blazor WebAssembly** - UI framework C#
- **Bootstrap 5** - Responsive design
- **Auth0.OidcClient** - Authentication
- **C# 9+** - Language

### Backend
- **ASP.NET Core 9** - Web framework
- **Entity Framework Core** - ORM
- **Pomelo MySQL** - Database driver
- **Polly** - Resilience & retry policies
- **Serilog** - Structured logging

### Database
- **MySQL 8.0+** - Relational database
- **EF Core Migrations** - Schema versioning

### External APIs
- **TheMealDB** - Recipe data (gratuit)
- **Auth0** - Authentication & authorization

### Testing
- **xUnit** - Unit tests
- **BUnit** - Blazor component tests
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library
- **Testcontainers** - MySQL for tests

---

## 💾 Installation détaillée

### Prérequis
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download)
- **MySQL 8.0+** - [Download](https://dev.mysql.com/downloads/mysql/)
- **Git** - [Download](https://git-scm.com/)
- **Compte Auth0** - [Sign up](https://auth0.com/signup)

### 1. Cloner le repository
```bash
git clone https://github.com/yourusername/RecipeHub.git
cd RecipeHub
```

### 2. Créer base de données MySQL
```bash
mysql -u root -p
CREATE DATABASE recipes_db;
CREATE USER 'recipes_user'@'localhost' IDENTIFIED BY 'password123';
GRANT ALL PRIVILEGES ON recipes_db.* TO 'recipes_user'@'localhost';
FLUSH PRIVILEGES;
EXIT;
```

### 3. Configurer Backend
```bash
cd RecipesApp.Server
```

Éditer `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=recipes_db;User Id=recipes_user;Password=password123;"
  },
  "Auth0": {
    "Domain": "your-domain.auth0.com",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Audience": "https://your-api"
  },
  "AllowedOrigins": "https://localhost:5001"
}
```

### 4. Appliquer les migrations
```bash
dotnet ef database update
```

### 5. Configurer Frontend
```bash
cd ../RecipesApp.Client
```

Éditer `appsettings.json`:
```json
{
  "Auth0": {
    "Domain": "your-domain.auth0.com",
    "ClientId": "your-spa-client-id",
    "RedirectUri": "https://localhost:5001/login/callback"
  },
  "ApiBaseAddress": "https://localhost:5001/api"
}
```

### 6. Restaurer dépendances
```bash
dotnet restore
```

### 7. Démarrer l'application

Terminal 1 (Backend):
```bash
cd RecipesApp.Server
dotnet run
```

Terminal 2 (Frontend):
```bash
cd RecipesApp.Client
dotnet run
```

Accéder à: **https://localhost:5001**

---

## ⚙️ Configuration

### Auth0 Setup

1. **Créer une application**:
   - Dashboard Auth0 → Applications
   - Create Application → Regular Web Application
   - Name: "RecipeHub"

2. **Configurer URLs**:
   - Allowed Callback URLs: `https://localhost:5001/callback`
   - Allowed Logout URLs: `https://localhost:5001`
   - Allowed Web Origins: `https://localhost:5001`

3. **Récupérer credentials**:
   - Domain: `Settings` tab
   - Client ID: `Settings` tab
   - Client Secret: `Settings` tab

4. **Créer API**:
   - APIs → Create API
   - Name: "RecipeHub API"
   - Identifier: `https://recipehub-api`

5. **Configurer application appsettings.json**

### MySQL Setup

```bash
# Connection string format
Server=localhost;Port=3306;Database=recipes_db;User Id=user;Password=pass;
```

### Variables d'environnement (Production)
```bash
export AUTH0_DOMAIN=your-domain.auth0.com
export AUTH0_CLIENT_ID=xxx
export AUTH0_CLIENT_SECRET=xxx
export MYSQL_CONNECTION_STRING=Server=...
```

---

## 🧪 Tests

### Exécuter tous les tests
```bash
dotnet test
```

### Tests unitaires (xUnit)
```bash
dotnet test RecipesApp.Tests
```

### Tests composants (BUnit)
```bash
dotnet test RecipesApp.Client.Tests
```

### Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Test Categories
- **Unit Tests**: Services, Repositories (~50 tests)
- **Component Tests**: Blazor components (~15 tests)
- **Integration Tests**: API + Database

---

## 📊 API Endpoints

### Recipes
```
GET    /api/recipes/random              → 6 recettes aléatoires
GET    /api/recipes/search?query=X      → Recherche par nom
GET    /api/recipes/categories          → Liste catégories
GET    /api/recipes/cuisines            → Liste cuisines
```

### Favorites
```
GET    /api/favorites                   → Tous favoris utilisateur
POST   /api/favorites                   → Ajouter favori
DELETE /api/favorites/{id}              → Supprimer favori
GET    /api/favorites/{id}/exists       → Vérifier si favori
```

### Contact
```
POST   /api/contact                     → Envoyer message
```

### Admin (Futur)
```
GET    /api/admin/messages              → Tous messages
PATCH  /api/admin/messages/{id}         → Marquer comme lus
```

---

## 🗂️ Structure Base de Données

### Tables
- **users**: Utilisateurs Auth0
- **favorites**: Recettes favorites (FK user_id)
- **contact_messages**: Messages de contact (FK user_id nullable)

### Migrations
```bash
# Créer nouvelle migration
dotnet ef migrations add MigrationName

# Appliquer migration
dotnet ef database update

# Rollback
dotnet ef database update PreviousMigration
```

---

## 🚦 Status & Roadmap

### ✅ MVP (Exam - 8 mars 2026)
- [x] Blazor WebAssembly setup
- [x] Auth0 authentication
- [x] TheMealDB API integration
- [x] Favorites CRUD
- [x] Contact form
- [x] Dark/Light theme
- [x] xUnit + BUnit tests

### 🔄 Phase 2 (Post-exam)
- [ ] Admin Dashboard
- [ ] Message management
- [ ] Export to CSV
- [ ] Email notifications

### 🎯 Phase 3 (Long-term)
- [ ] User recipes (create/edit)
- [ ] Recipe sharing
- [ ] Comments & ratings
- [ ] Meal planning
- [ ] Nutrition API

---

## 🤝 Contribution

1. Fork le repository
2. Créer une feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Ouvrir un Pull Request

---

## 📝 License

Ce projet est sous license **MIT** - voir le fichier [LICENSE](LICENSE) pour détails.

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/yourusername/RecipeHub/issues)
- **Documentation**: Voir [PROJET_RECETTES_COMPLET.md](PROJET_RECETTES_COMPLET.md)
- **Architecture**: Voir [ARCHITECTURE.md](ARCHITECTURE.md) (à créer)

---

## 👨‍💻 Auteur

Créé comme project d'examen - C# 9, Blazor WebAssembly, ASP.NET Core

**Date de création**: 08-02-2026
**Deadline**: 8 mars 2026

---

**Made with ❤️ using Blazor & C#**
