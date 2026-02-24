# 🤝 Contributing Guide - menuMalin

Merci de contribuer à menuMalin ! Ce guide vous aide à configurer votre environnement et à contribuer efficacement.

---

## 📋 Table des Matières

1. [Setup Environnement](#-setup-environnement)
2. [Structure du Projet](#-structure-du-projet)
3. [Exécuter l'Application](#-exécuter-lapplication)
4. [Exécuter les Tests](#-exécuter-les-tests)
5. [Commit Guidelines](#-commit-guidelines)
6. [Code Style](#-code-style)

---

## 🛠️ Setup Environnement

### Prérequis
- **.NET 9 SDK** (télécharger depuis [dotnet.microsoft.com](https://dotnet.microsoft.com))
- **MySQL 8.0+** (local ou Docker)
- **Git**
- **Visual Studio Code** ou **Visual Studio 2022**
- **Auth0 Account** (gratuit sur [auth0.com](https://auth0.com))

### 1. Cloner le Repository
```bash
git clone https://github.com/yourusername/menumalin.git
cd menumalin
```

### 2. Configurer la Base de Données

#### Option A: MySQL Local
```bash
# Créer la base de données
mysql -u root -p
CREATE DATABASE menumalin_db;
CREATE USER 'menumalin_user'@'localhost' IDENTIFIED BY 'password';
GRANT ALL PRIVILEGES ON menumalin_db.* TO 'menumalin_user'@'localhost';
FLUSH PRIVILEGES;
```

#### Option B: Docker (Recommandé)
```bash
docker run --name mysql-menumalin \
  -e MYSQL_ROOT_PASSWORD=root \
  -e MYSQL_DATABASE=menumalin_db \
  -e MYSQL_USER=menumalin_user \
  -e MYSQL_PASSWORD=password \
  -p 3306:3306 \
  -d mysql:8.0
```

### 3. Configurer Auth0
1. Créer un compte sur [auth0.com](https://auth0.com)
2. Créer une Application "Single Page Application"
3. Configurer les **Allowed Callback URLs**:
   ```
   https://localhost:7777/authentication/login-callback
   http://localhost:7777/authentication/login-callback
   ```
4. Configurer les **Allowed Logout URLs**:
   ```
   https://localhost:7777
   http://localhost:7777
   ```
5. Copier le **Domain** et **Client ID**

### 4. Configurer appsettings.json

**menuMalin.Server/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=menumalin_db;User=menumalin_user;Password=password;"
  },
  "Auth0": {
    "Domain": "your-domain.auth0.com",
    "ClientId": "your-client-id"
  }
}
```

**menuMalin/wwwroot/appsettings.json:**
```json
{
  "auth0": {
    "authority": "https://your-domain.auth0.com",
    "clientId": "your-client-id",
    "redirectUri": "https://localhost:7777/authentication/login-callback"
  }
}
```

### 5. Restaurer les Dépendances
```bash
dotnet restore
```

### 6. Appliquer les Migrations
```bash
cd menuMalin.Server
dotnet ef database update
cd ..
```

---

## 📁 Structure du Projet

```
menumalin/
├── menuMalin/                      # Frontend (Blazor WebAssembly)
│   ├── Pages/                      # Pages Razor
│   ├── Components/                 # Composants Blazor
│   ├── Services/                   # Services frontend
│   ├── Layouts/                    # Layouts
│   └── wwwroot/                    # Assets statiques
│
├── menuMalin.Server/               # Backend (ASP.NET Core)
│   ├── Controllers/                # Contrôleurs API
│   ├── Services/                   # Services backend
│   ├── Repositories/               # Repositories
│   ├── Models/
│   │   ├── Entities/               # Database entities
│   │   └── DTOs/                   # Data transfer objects
│   ├── Data/                       # DbContext
│   └── Properties/                 # Configurations
│
├── menuMalin.Shared/               # Code partagé
│   └── Models/                     # Models partagés
│
├── menuMalin.Tests/                # Backend tests
│   ├── Services/                   # Service tests
│   ├── Integration/                # Integration tests
│   └── EdgeCases/                  # Edge case tests
│
├── menuMalin.Client.Tests/         # Frontend tests (BUnit)
│   ├── Components/                 # Component tests
│   └── Pages/                      # Page tests
│
└── docs/                           # Documentation
    ├── ARCHITECTURE.md
    ├── API_DOCUMENTATION.md
    ├── TESTING_REPORT.md
    └── CONTRIBUTING.md
```

---

## 🚀 Exécuter l'Application

### Backend
```bash
cd menuMalin.Server
dotnet run
# Accéder à https://localhost:5266/api/auth/health
```

### Frontend
```bash
cd menuMalin
dotnet run
# Accéder à https://localhost:7777
```

### Les deux ensemble (dans 2 terminaux différents)
Terminal 1:
```bash
cd menuMalin.Server && dotnet run
```

Terminal 2:
```bash
cd menuMalin && dotnet run
```

---

## 🧪 Exécuter les Tests

### Tous les tests
```bash
dotnet test
```

### Tests spécifiques
```bash
# Tests de services uniquement
dotnet test menuMalin.Tests --filter "Category=Services"

# Tests d'intégration uniquement
dotnet test menuMalin.Tests --filter "Category=Integration"

# Tests d'edge cases uniquement
dotnet test menuMalin.Tests --filter "Category=EdgeCase"
```

### Avec couverture
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=json
```

### Watch mode (re-exécute à chaque changement)
```bash
dotnet watch test
```

---

## 📝 Commit Guidelines

### Format du Message
```
<type>: <subject>

<body>

<footer>
```

### Types
- **feat**: Nouvelle feature
- **fix**: Bug fix
- **docs**: Documentation
- **style**: Formatage (pas de changement logique)
- **refactor**: Refactoring du code
- **perf**: Amélioration de performance
- **test**: Ajout ou modification de tests
- **chore**: Maintenance

### Exemples
```bash
# Bonne pratique
git commit -m "feat: Add recipe search by category"
git commit -m "fix: Handle null reference in FavoriteService"
git commit -m "test: Add edge case tests for ContactService"

# Mauvaise pratique
git commit -m "Fixed stuff"
git commit -m "Updated files"
```

### Commit Workflow
```bash
# 1. Vérifier le statut
git status

# 2. Ajouter les fichiers
git add <files>

# 3. Vérifier les changements
git diff --staged

# 4. Créer le commit
git commit -m "feat: Your descriptive message"

# 5. Vérifier le log
git log --oneline -5
```

---

## 🎨 Code Style

### C# / Backend
- **Naming**: PascalCase pour classes/méthodes, camelCase pour variables
- **Indentation**: 4 espaces
- **Null checks**: Utiliser `?.` ou `?? new()`
- **Comments**: XML comments pour les méthodes publiques
- **Async**: `async/await` pour opérations asynchrones

**Exemple:**
```csharp
/// <summary>
/// Ajoute une recette aux favoris
/// </summary>
/// <param name="recipe">La recette à ajouter</param>
/// <returns>Task complété</returns>
public async Task AddFavoriteAsync(Recipe recipe)
{
    if (recipe == null)
        throw new ArgumentNullException(nameof(recipe));

    var favorites = await GetFavoritesAsync();
    // Logic...
}
```

### Razor Components
- **File names**: PascalCase (RecipeCard.razor)
- **Components**: Un composant par fichier
- **Events**: @onclick, @onchange avec handlers
- **Styling**: Scoped CSS ou CSS global

**Exemple:**
```razor
@implements IAsyncDisposable
@page "/search"
@attribute [Authorize]

<div class="search-container">
    <input type="text" @bind="searchQuery" />
    <button @onclick="HandleSearch">Search</button>
</div>

@code {
    private string? searchQuery;

    private async Task HandleSearch()
    {
        // Logic...
    }
}
```

### Tests
- **Naming**: `MethodName_Condition_ExpectedResult`
- **Pattern**: Arrange-Act-Assert
- **Mocking**: Moq pour les dépendances
- **Assertions**: FluentAssertions pour lisibilité

**Exemple:**
```csharp
[Fact]
public async Task AddFavoriteAsync_WithValidRecipe_ShouldSucceed()
{
    // Arrange
    var recipe = CreateTestRecipe("1");

    // Act
    var result = await favoriteService.AddFavoriteAsync(recipe);

    // Assert
    result.Should().NotBeNull();
}
```

---

## 🔍 Pre-commit Checklist

Avant de commiter:
- [ ] Code compile sans erreurs (`dotnet build`)
- [ ] Tous les tests passent (`dotnet test`)
- [ ] Pas de console warnings
- [ ] Code suit le style guide
- [ ] Commit message est descriptif
- [ ] Changes sont logiquement groupés

---

## 🚦 Pull Request Process

1. Fork le repository
2. Créer une branch (`git checkout -b feature/amazing-feature`)
3. Faire vos changements
4. Commiter vos changements (`git commit -m 'feat: Add amazing feature'`)
5. Push vers la branch (`git push origin feature/amazing-feature`)
6. Ouvrir une Pull Request

### PR Description Template
```markdown
## Description
Brève description de ce que cette PR fait

## Type de changement
- [ ] Bug fix
- [ ] Nouvelle feature
- [ ] Breaking change
- [ ] Documentation

## Testing
- [ ] Unitaires
- [ ] Intégration
- [ ] Manuel

## Checklist
- [ ] Mon code suit le style guide
- [ ] J'ai commenté mon code
- [ ] J'ai mis à jour la documentation
- [ ] Mes changements ne génèrent pas de warnings
- [ ] J'ai ajouté des tests
```

---

## 📞 Support & Questions

- **Documentation**: Vérifiez les fichiers dans `/docs`
- **Issues**: Ouvrez une issue sur GitHub
- **Discussions**: Utilisez les Discussions GitHub

---

## 📄 Licence

Ce projet est sous licence [MIT](../LICENSE)

---

**Merci de contribuer à menuMalin! 🎉**

*Last Updated: 2026-02-24*
