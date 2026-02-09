# 📝 Product Requirements Document (PRD) - MenuMalin

**Projet :** Application Blazor WebAssembly de gestion de recettes
**Étudiant :** Adrien Mertens
**Date de remise :** 8 mars 2026
**Statut :** En cours de développement

---

## 1. Vision du Produit
**MenuMalin** est une application web interactive (PWA) conçue pour aider les utilisateurs à organiser leurs repas. L'application permet de découvrir des recettes via une API internationale, de les sauvegarder localement pour les personnaliser, et de générer une liste de courses intelligente basée sur les ingrédients manquants.

## 2. Objectifs Techniques (Critères d'Examen)
* **Frontend :** Blazor WebAssembly (.NET 8/9) pour une expérience fluide.
* **Authentification :** Implémentation du protocole **OIDC (OpenID Connect)** via Auth0.
* **API :** Intégration de l'API REST [TheMealDB](https://www.themealdb.com/api.php).
* **Persistance :** CRUD local complet (LocalStorage ou SQLite WASM) pour la gestion des favoris et des modifications.
* **Qualité :** Tests unitaires avec **xUnit** et tests de composants UI avec **bUnit**.
* **Déploiement :** Dockerisation et hébergement sur serveur **Coolify**.

---

## 3. Spécifications Fonctionnelles (User Stories)

| ID | Titre | Description |
| :--- | :--- | :--- |
| **US1** | Recherche API | L'utilisateur peut rechercher des recettes par nom ou ingrédient principal. |
| **US2** | Auth OIDC | L'utilisateur doit se connecter via Auth0 pour accéder à son espace "Favoris". |
| **US3** | Gestion Favoris | Un utilisateur connecté peut ajouter/supprimer une recette de ses favoris. |
| **US4** | Édition Locale | L'utilisateur peut modifier le titre, les notes et les instructions d'une recette sauvegardée sans modifier l'API originale. |
| **US5** | Liste de Courses | L'utilisateur peut cocher les ingrédients qu'il n'a pas. L'application génère une liste récapitulative. |
| **US6** | Tutoriel Vidéo | Affichage dynamique d'un lecteur YouTube pour chaque recette (si disponible). |

---

## 4. Architecture Logicielle

### Flux d'authentification OIDC


1. L'utilisateur demande une ressource protégée (les favoris).
2. L'application redirige vers Auth0.
3. Après succès, Auth0 renvoie un `ID Token` et un `Access Token`.
4. L'application utilise l'identifiant unique (`sub`) pour filtrer les favoris locaux de l'utilisateur.

### Structure des données (C#)
```csharp
public class Recipe
{
    public string IdMeal { get; set; } // ID API
    public string StrMeal { get; set; } // Nom modifiable
    public string StrInstructions { get; set; } // Instructions modifiables
    public string StrYoutube { get; set; } // Lien vidéo
    public List<Ingredient> Ingredients { get; set; } = new();
    public string UserNote { get; set; } // Note personnelle
    public string OwnerId { get; set; } // Lien avec le 'sub' OIDC
}

public class Ingredient
{
    public string Name { get; set; }
    public string Measure { get; set; }
    public bool IsMissing { get; set; } // État pour la liste de courses
}