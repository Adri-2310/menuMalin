# Historique des Modifications

Toutes les modifications et améliorations apportées à MenuMalin.

## [En Cours de Développement]

### À faire
- [ ] Rate limiting sur les endpoints sensibles
- [ ] Pagination des résultats API
- [ ] Recherche avancée avec filtres multiples
- [ ] Export des recettes en PDF
- [ ] Dark mode
- [ ] Notifications email pour les recettes similaires

---

## [2.0.0] - 2026-03-10

### ✨ Nouvelles Fonctionnalités

- **Documentation Complète**: Restructuration en `/docs` avec 6 fichiers détaillés
  - INSTALLATION.md - Guide d'installation et configuration
  - ARCHITECTURE.md - Design patterns et principes POO
  - FEATURES.md - Fonctionnalités détaillées et cas d'usage
  - API.md - Référence complète des endpoints
  - TROUBLESHOOTING.md - Dépannage et solutions
  - CHANGELOG.md - Historique des modifications

- **Console Propre**: Logging minimaliste et professionnel
  - Information level uniquement pour navigation et hosting
  - Warning level pour System.Net.Http
  - Pas de debug console.writeline en production

- **Database Robuste**:
  - Contrôle de connexion MySQL avec timeout (5s)
  - Migrations automatiques si MySQL accessible
  - Message d'avertissement clair si non accessible

### 🐛 Corrections de Bugs

- **Synchronisation Auth**: localStorage désynchronisé du cookie serveur
  - MesFavoris.razor appelle CheckAuthentication() à chaque chargement
  - ClearAuthStateAsync() nettoie les données si 401

- **JSON Deserialization**: Erreurs de désérialisation non capturées
  - Ajout de catch JsonException dans ServiceApiHttp
  - Enveloppage dans ErreurApiException

- **Route Mismatch**: Navigation vers `/recipe/` mais route `/recette/{Id}`
  - CarteRecette.razor ligne 24: correction de l'URL

- **Cookie SameSite**: Favoris non chargés en cross-port
  - Changé de Strict à Lax pour permettre les cookies entre ports différents

### 🔧 Améliorations

- **Logging Configuration**:
  - appsettings.Development.json ajusté pour afficher navigation
  - Suppression des Console.WriteLine en production
  - Logging cohérent par catégorie

- **Service HTTP**:
  - PropertyNamingPolicy retiré (PropertyNameCaseInsensitive=true suffit)
  - Gestion d'erreurs centralisée avec exceptions spécifiques

- **Kestrel**: Configuration clarifié, évite duplication

### 📦 Dépendances
- Microsoft.AspNetCore 9.0
- Entity Framework Core 9.0
- Blazored.Toast 4.2.1
- Polly 8.x (retry logic)

---

## [1.5.0] - 2026-03-03

### ✨ Nouvelles Fonctionnalités

- **Page d'Accueil Améliorée**:
  - Logo application remplace l'icône egg-fried
  - Taille 120x120px avec drop-shadow
  - Suppression des boutons redondants (Mes Favoris, Se déconnecter)

### 🐛 Corrections de Bugs

- **26 Bugs Corrigés** dans un audit systématique:

#### Frontend (12 corrections)
1. URLs invalides - 6 fichiers: changé `href="/"` → `href="/connexion"`
2. Accueil.razor - URL favoris: `href="favorites"` → `href="/mes-favoris"`
3. FormulaireContact.razor - Email codé: "admin@recipehub.com" → empty
4. ServiceAuthentification.LogoutAsync - Supprimé reload(), exécute ClearAuthStateAsync()
5. MesFavoris.razor - Rafraîchissement SPA: ajouté OnParametersSetAsync()
6. Services - Exceptions avalées: supprimé try/catch, remonte l'erreur
7. App.razor - Texte anglais traduit en français
8. RedirectionConnexion.razor - Route corrigée
9. FormulaireConnexion.razor - Validation redondante supprimée
10. GrilleRecettesDto.razor - Composant mort supprimé
11. ServiceTeleversement.cs - Credentials ajoutés: SetBrowserRequestCredentials(Include)
12. Accueil.razor - Suppression des deux boutons (Mes Favoris, Déconnexion)

#### Backend (14 corrections)
1. Routes - Conventions corrigées (api/[controller] → noms spécifiques)
2. ControleurTeleversement - Supprimé vérification manuelle d'auth
3. ControleurRecettesUtilisateur - Vérification privée/propriétaire ajoutée
4. ServiceRecetteUtilisateur.UpdateAsync - ImageUrl maintenant mise à jour
5. RecetteMealDTO - Ingrédients étendus de 5 à 20
6. RecetteDTO - MealDBId nullable pour recettes user
7. ControleurFavori - GetAuth0IdFromClaims() → GetCurrentUserId()
8. ControleurAuthentification - Supprimé ExpiresUtc (bloquait SlidingExpiration)
9+ Plus de corrections d'erreurs et validations

### 🏗️ Refactoring

- Audit complet du codebase (25+ fichiers modifiés)
- Nettoyage des logs de débogage
- Validation cohérente dans les services
- Gestion d'erreurs standardisée

---

## [1.4.0] - 2026-03-01

### 🐛 Corrections de Bugs

- **Route Mismatch**: `/recipe/` vs `/recette/{Id}`
  - CarteRecette.razor: changé href vers la bonne route
  - Commit: e3c8909

- **Cookie Authentication Favoris**:
  - SameSite=Lax pour permettre cross-port cookies
  - Frontend 7777 + Backend 7057 peuvent maintenant partager les cookies
  - Commit: 2692055

### 📝 Commits
- e3c8909 - fix: Corriger l'URL vers la page de détails
- 2692055 - fix: Permettre l'envoi des cookies d'authentification entre les ports différents

---

## [1.3.0] - 2026-02-28

### 🐛 Corrections de Bugs

- **Recipe Details Mapping**:
  - Deserialization issue dans FilterByCategoryAsync
  - RecetteMealDTO deserialisé au lieu de Recette
  - IdMeal utilise MealDBId au lieu de RecipeId
  - Commit: e8a6a65

### 📝 Services Modifiés
- ServiceRecetteFrontend.cs
- ReponseRecette.cs

---

## [1.2.0] - 2026-02-27

### ✨ Nouvelles Fonctionnalités

- **NToastNotify Integration**:
  - Système de notifications toast (succès, erreur, warning, info)
  - IServiceNotification interface avec 6 méthodes
  - ServiceNotification implémentation
  - BlazoredToasts dans DispositionPrincipale.razor

- **Exception Hierarchy**:
  - ErreurApiException pour les erreurs HTTP
  - ErreurReseauException pour les erreurs réseau
  - ServiceApiHttp propage les exceptions

- **Modal Confirmation**:
  - ModaleConfirmation.razor remplace JSRuntime.confirm()
  - Callbacks OnConfirme/OnAnnule

### 📝 Fichiers Créés
- Services/Exceptions/ErreurApiException.cs
- Services/Exceptions/ErreurReseauException.cs
- Services/ServiceNotification.cs
- Components/ModaleConfirmation.razor

---

## [1.1.0] - 2026-02-25

### ✨ Nouvelles Fonctionnalités

- **Authentication Système**:
  - Cookie-based auth au lieu de JWT
  - BCrypt password hashing
  - HttpOnly + SameSite=Lax cookies
  - Sliding expiration (24h d'inactivité)

- **Database Setup**:
  - Entity Framework Core avec MySQL
  - Auto-migrations au démarrage
  - ApplicationDbContext avec 5 DbSets

- **Dépôt Pattern**:
  - IDepotUtilisateur, IDepotRecette, IDepotFavori
  - IDepotRecetteUtilisateur, IDepotMessage
  - Implémentations concrètes avec EF Core

- **Services Métier**:
  - IServiceUtilisateur, IServiceRecette, IServiceFavoris
  - IServiceRecetteUtilisateur, IServiceEmail
  - ServiceMealDB pour TheMealDB API

### 📝 Architecture
```
Controllers/
├── ControleurAuthentification.cs
├── ControleurRecettes.cs
├── ControleurFavoris.cs
├── ControleurRecettesUtilisateur.cs
├── ControleurUtilisateurs.cs
├── ControleurContact.cs
└── ControleurTeleversement.cs

Services/
├── ServiceUtilisateur.cs
├── ServiceRecette.cs
├── ServiceMealDB.cs
├── ServiceFavoris.cs
├── ServiceRecetteUtilisateur.cs
├── ServiceEmail.cs
└── ServiceNotification.cs (future)

Depots/
├── DepotUtilisateur.cs
├── DepotRecette.cs
├── DepotFavori.cs
├── DepotRecetteUtilisateur.cs
└── DepotMessage.cs
```

---

## [1.0.0] - 2026-02-20

### ✨ Features Initiales

- **Recherche API**:
  - Intégration TheMealDB API
  - Recherche par mot-clé
  - Filtrage par catégorie et région
  - Détails complets des recettes
  - Retry automatique avec Polly

- **Gestion Favoris**:
  - Ajouter/supprimer des favoris
  - Vue "Mes Favoris"
  - Persistence en base de données
  - Vérification rapide (is favorite)

- **Recettes Personnalisées**:
  - CRUD complet
  - Upload d'images
  - Gestion des ingrédients
  - Partage public/privé

- **Authentification**:
  - Inscription avec validation email
  - Connexion sécurisée
  - Profil utilisateur
  - Gestion mot de passe

- **Interface Blazor**:
  - Bootstrap 5 responsive
  - Composants réutilisables
  - Navigation fluide (SPA)
  - Error boundaries

- **Gestion Erreurs**:
  - Middleware global
  - Messages localisés
  - Logging centralisé

### 📦 Stack Technique
- **Backend**: ASP.NET Core 9.0
- **Frontend**: Blazor WebAssembly
- **Database**: MySQL 8.0
- **ORM**: Entity Framework Core 9.0
- **Cache/Retry**: Polly 8.x
- **Auth**: Cookie-based + BCrypt

### 🎯 Principes POO Implémentés
✅ Encapsulation - Services avec interfaces
✅ Abstraction - DTOs et contracts
✅ Héritage - Hiérarchie d'exceptions
✅ Polymorphisme - Implémentations multiples
✅ Composition - Services injectés

### 🔒 Sécurité
- BCrypt password hashing
- HttpOnly cookies
- CORS configuré
- SQL paramétré (Entity Framework)
- Validation input
- Error messages génériques

---

## Conventions de Commits

Format utilisé: `<type>: <message>`

Types:
- `feat:` - Nouvelle fonctionnalité
- `fix:` - Correction de bug
- `docs:` - Modifications documentation
- `refactor:` - Refactorisation (pas de changement feature)
- `perf:` - Amélioration de performance
- `test:` - Ajout/modification tests
- `chore:` - Tâches de maintenance

Exemples:
- `feat: Ajouter le système de favoris`
- `fix: Corriger desynchronisation auth localStorage/cookie`
- `docs: Mettre à jour README`
- `refactor: Extraire la logique d'authentification`

---

## Version Numbering

Format: `MAJEUR.MINEUR.PATCH`

- **MAJEUR** (1→2): Changement cassant d'API ou architecture majeure
- **MINEUR** (0→4): Nouvelle fonctionnalité rétro-compatible
- **PATCH** (0→1): Correction de bug

Exemple: 1.3.2 = Version 1, 3e release mineure, 2e patch

---

## Liens Importants

- **Repository**: https://github.com/user/menuMalin
- **Documentation**: `/docs/` folder
- **Issues**: GitHub Issues
- **Contact**: adrien.mertens@example.com

