# MenuMalin - Documentation Technique

**Dernière mise à jour :** 9 février 2026

## Vue d'ensemble

MenuMalin est une application web de gestion de recettes développée avec **Blazor WebAssembly** et **.NET 9.0**. L'application permet aux utilisateurs de rechercher des recettes via l'API TheMealDB, de les sauvegarder en favoris, de les modifier localement et de générer des listes de courses.

## 🎯 Statut du Projet

**Phase actuelle :** Phase 1 - Infrastructure (✅ Complétée)
**Date de livraison :** 8 mars 2026
**Build Status :** ✅ Compilation réussie (0 erreurs)

### ✅ Fonctionnalités Implémentées

- ✅ Infrastructure Blazor WebAssembly configurée
- ✅ Navigation responsive avec menu mobile
- ✅ Authentification OIDC (Auth0) configurée
- ✅ Page d'accueil avec double vue (authentifié/visiteur)
- ✅ Design UI/UX moderne et responsive
- ✅ Configuration PWA (Progressive Web App)
- ✅ CSS scoped par composant
- ✅ Service RecipeService pour API TheMealDB
- ✅ Structure professionnelle avec DTOs séparés
- ✅ Page Authentication pour callbacks Auth0

### 🚧 En Cours de Développement

- ⏳ Service LocalStorage pour persistance
- ⏳ Service FavoriteService
- ⏳ Service ShoppingListService
- ⏳ Composants de recettes (RecipeCard, RecipeList, etc.)
- ⏳ Pages de favoris et liste de courses

### 📋 À Faire (Prioritaire)

- [ ] Ajouter gestion d'erreurs complète dans services
- [ ] Créer composants UI (LoadingSpinner, ErrorAlert, etc.)
- [ ] Implémenter les pages Favorites et ShoppingList
- [ ] Ajouter tests unitaires
- [ ] Optimiser images (logo.png)

## 📚 Documentation

- **[PROJECT_STATUS.md](./PROJECT_STATUS.md)** - État actuel du projet (détaillé)
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Architecture détaillée du projet
- **[ISSUES.md](./ISSUES.md)** - Problèmes techniques (TOUS RÉSOLUS ✅)
- **[IMPLEMENTATION_PLAN.md](./IMPLEMENTATION_PLAN.md)** - Plan d'implémentation par phases
- **[CODE_REVIEW.md](./CODE_REVIEW.md)** - Revue de code et recommandations

## 🏗️ Architecture Technique

### Stack Technologique

- **Framework** : Blazor WebAssembly .NET 9.0
- **UI** : Bootstrap 5.3 + CSS Custom
- **Authentification** : OIDC (Auth0)
- **API externe** : TheMealDB API
- **Storage** : LocalStorage (navigateur)
- **PWA** : Service Workers

### Structure du Projet

```
menuMalin/
├── Components/          # Composants réutilisables
│   ├── Authentication/  # → RedirectToLogin.razor ✅
│   ├── Common/         # Composants communs (à développer)
│   ├── Recipe/         # Composants de recettes (à développer)
│   ├── Search/         # Composants de recherche (à développer)
│   ├── Shopping/       # Liste de courses (à développer)
│   └── Favorites/      # Gestion favoris (à développer)
├── Configuration/      # Classes de configuration ✅
├── Constants/          # Constantes de l'application ✅
├── DTOs/              # Data Transfer Objects ✅
│   └── RecipeResponse.cs
├── Extensions/         # Méthodes d'extension ✅
├── Layouts/           # Layout principal ✅
│   └── MainLayout.razor
├── Models/            # Modèles de données ✅
│   └── Recipe.cs
├── Pages/             # Pages de l'application ✅
│   ├── Index.razor
│   └── Authentication.razor
├── Services/          # Services métier ✅
│   ├── IRecipeService.cs
│   └── RecipeService.cs
├── Shared/            # Composants partagés ✅
├── Utilities/         # Classes utilitaires ✅
└── wwwroot/           # Ressources statiques ✅
    ├── css/           # Styles
    ├── images/        # Images
    ├── js/            # JavaScript
    └── ...
```

## 🚀 Démarrage Rapide

### Prérequis

- .NET 9.0 SDK (installé ✅)
- Visual Studio 2022 ou VS Code
- Compte Auth0 (pour l'authentification)

### Installation

```bash
# Cloner le repository
git clone <repo-url>
cd menuMalin

# Restaurer les packages
dotnet restore

# Build le projet
dotnet build

# Lancer l'application
dotnet run
```

### Configuration

1. Créer un compte Auth0
2. Configurer l'application Auth0
3. Mettre à jour `wwwroot/appsettings.json` avec vos identifiants :
```json
{
  "Auth0": {
    "Authority": "https://votre-domaine.auth0.com",
    "ClientId": "votre-client-id"
  }
}
```
4. Redémarrer l'application

## 📊 Métriques du Code

- **Fichiers C#** : 5
- **Composants Razor** : 5
- **Lignes de code** : ~200
- **Couverture de tests** : 0% (tests à implémenter)
- **Erreurs de compilation** : 0 ✅
- **Avertissements** : 0 ✅
- **Dependencies** : 4 packages NuGet

## 🔗 Liens Utiles

- [TheMealDB API Documentation](https://www.themealdb.com/api.php)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [Auth0 Documentation](https://auth0.com/docs)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.3/)

## 📝 Changelog

### Version 1.0.0-dev (9 février 2026)

**Améliorations :**
- ✅ Structure nettoyée et professionnelle
- ✅ DTOs séparés des Models
- ✅ Configuration Auth0 standardisée
- ✅ Page Authentication.razor ajoutée
- ✅ Packages NuGet alignés (v9.0.11)
- ✅ 0 erreurs de compilation

**Corrections :**
- ✅ Fichiers temporaires supprimés (nul, STRUCTURE.txt)
- ✅ Dossier Layout/ renommé en Layouts/
- ✅ Configuration appsettings.Development.json corrigée
- ✅ RecipeResponse extrait de Recipe.cs
- ✅ Namespace menuMalin.DTOs ajouté

**Nouveautés :**
- ✅ Dossiers Configuration/, Constants/, Extensions/, Shared/
- ✅ robots.txt pour SEO
- ✅ wwwroot/css/components/ créé

## 👥 Contributeur

**Adrien Mertens** - Développeur principal

## 📄 Licence

Projet académique - Tous droits réservés

---

**Note :** Ce projet est en développement actif. Consultez [PROJECT_STATUS.md](./PROJECT_STATUS.md) pour l'état détaillé.
