# MenuMalin - Documentation Technique

## Vue d'ensemble

MenuMalin est une application web de gestion de recettes développée avec **Blazor WebAssembly** et **.NET 9.0**. L'application permet aux utilisateurs de rechercher des recettes via l'API TheMealDB, de les sauvegarder en favoris, de les modifier localement et de générer des listes de courses.

## 🎯 Statut du Projet

**Phase actuelle**: Phase 1 - Infrastructure (Partiellement complétée)
**Date de livraison**: 8 mars 2026
**Dernière mise à jour**: 9 février 2026

### ✅ Fonctionnalités Implémentées

- Infrastructure Blazor WebAssembly configurée
- Navigation responsive avec menu mobile
- Authentification OIDC (Auth0) configurée
- Page d'accueil avec double vue (authentifié/visiteur)
- Design UI/UX moderne et responsive
- Configuration PWA (Progressive Web App)
- CSS scoped par composant

### 🚧 En Cours de Développement

- Intégration API TheMealDB
- Système de favoris avec LocalStorage
- Liste de courses
- Composants de recherche et affichage de recettes

### 📋 À Faire (Prioritaire)

- Corriger les problèmes critiques (voir [ISSUES.md](./ISSUES.md))
- Implémenter les services core (API, LocalStorage, State)
- Créer les composants de recettes
- Ajouter les tests unitaires

## 📚 Documentation

- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Architecture détaillée du projet
- **[ISSUES.md](./ISSUES.md)** - Problèmes techniques identifiés et solutions
- **[IMPLEMENTATION_PLAN.md](./IMPLEMENTATION_PLAN.md)** - Plan d'implémentation par phases
- **[CODE_REVIEW.md](./CODE_REVIEW.md)** - Revue de code et recommandations

## 🏗️ Architecture Technique

### Stack Technologique

- **Framework**: Blazor WebAssembly .NET 9.0
- **UI**: Bootstrap 5.3 + CSS Custom
- **Authentification**: OIDC (Auth0)
- **API externe**: TheMealDB API
- **Storage**: LocalStorage (navigateur)
- **PWA**: Service Workers

### Structure du Projet

```
menuMalin/
├── Components/          # Composants réutilisables (à implémenter)
│   ├── Common/         # Composants communs
│   ├── Recipe/         # Composants de recettes
│   ├── Search/         # Composants de recherche
│   ├── Shopping/       # Liste de courses
│   └── Favorites/      # Gestion favoris
├── Layout/             # Layout principal
│   └── MainLayout.razor
├── Pages/              # Pages de l'application
│   └── Index.razor
├── Services/           # Services (à implémenter)
│   ├── Api/           # Services API
│   ├── Business/      # Logique métier
│   ├── State/         # Gestion d'état
│   └── Storage/       # LocalStorage
├── Models/            # Modèles de données (à compléter)
└── wwwroot/           # Ressources statiques
    ├── css/           # Styles
    ├── images/        # Images
    └── js/            # JavaScript
```

## 🚀 Démarrage Rapide

### Prérequis

- .NET 9.0 SDK
- Visual Studio 2022 ou VS Code
- Compte Auth0 (pour l'authentification)

### Installation

```bash
# Cloner le repository
git clone <repo-url>

# Restaurer les packages
dotnet restore

# Lancer l'application
dotnet run --project menuMalin
```

### Configuration

1. Créer un compte Auth0
2. Configurer l'application Auth0
3. Mettre à jour `appsettings.json` avec vos identifiants
4. Redémarrer l'application

## 📊 Métriques du Code

- **Fichiers C#**: 3 (+ fichiers générés)
- **Composants Razor**: 4
- **Lignes de code**: ~150
- **Couverture de tests**: 0% (tests à implémenter)
- **Dependencies**: 3 packages NuGet

## 🔗 Liens Utiles

- [TheMealDB API Documentation](https://www.themealdb.com/api.php)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [Auth0 Documentation](https://auth0.com/docs)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.3/)

## 👥 Contributeur

- **Adrien Mertens** - Développeur principal

## 📄 Licence

Projet académique - Tous droits réservés
