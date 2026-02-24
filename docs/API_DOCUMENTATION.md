# 📡 API Documentation - menuMalin

**Version:** 1.0
**Base URL:** `http://localhost:5266/api`
**Last Updated:** 24 février 2026

---

## 🔑 Authentication

Tous les endpoints protégés nécessitent un **JWT Bearer Token** dans l'header:

```http
Authorization: Bearer <token>
```

**Endpoints publics** (pas d'auth requise):
- `GET /recipes/random`
- `GET /recipes/search`
- `POST /contact`
- `GET /auth/health`

---

## 📋 Endpoints

### 🍽️ Recipes Controller

#### GET /api/recipes/random
Récupère 6 recettes aléatoires

**Paramètres:**
- Aucun

**Réponse:**
```json
[
  {
    "idMeal": "52977",
    "strMeal": "Corba",
    "strCategory": "Miscellaneous",
    "strArea": "Turkish",
    "strMealThumb": "https://www.themealdb.com/images/..."
  }
]
```

**Codes:**
- `200 OK` - Succès
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/recipes/search?query={query}
Recherche des recettes par nom

**Paramètres:**
- `query` (string) - Terme de recherche (ex: "brownie")

**Réponse:**
```json
[
  {
    "idMeal": "52897",
    "strMeal": "Chocolate Brownies",
    "strCategory": "Dessert",
    "strArea": "American",
    "strMealThumb": "https://..."
  }
]
```

**Codes:**
- `200 OK` - Succès
- `400 Bad Request` - Query vide ou invalide
- `404 Not Found` - Aucune recette trouvée
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/recipes/{mealId}
Récupère les détails d'une recette

**Paramètres:**
- `mealId` (string) - ID de la recette

**Réponse:**
```json
{
  "idMeal": "52977",
  "strMeal": "Corba",
  "strCategory": "Miscellaneous",
  "strArea": "Turkish",
  "strMealThumb": "https://...",
  "strInstructions": "...",
  "strYoutube": "https://youtube.com/watch?v=..."
}
```

**Codes:**
- `200 OK` - Succès
- `404 Not Found` - Recette non trouvée
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/recipes/categories/list
Récupère toutes les catégories

**Paramètres:**
- Aucun

**Réponse:**
```json
[
  "Seafood",
  "Breakfast",
  "Vegetarian",
  "Pasta",
  "Dessert"
]
```

**Codes:**
- `200 OK` - Succès
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/recipes/areas/list
Récupère toutes les zones/cuisines

**Paramètres:**
- Aucun

**Réponse:**
```json
[
  "Italian",
  "French",
  "American",
  "Japanese",
  "Turkish"
]
```

**Codes:**
- `200 OK` - Succès
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/recipes/filter/category?category={category}
Filtre les recettes par catégorie

**Paramètres:**
- `category` (string) - Nom de la catégorie

**Réponse:**
```json
[
  {
    "idMeal": "52977",
    "strMeal": "...",
    "strCategory": "Dessert"
  }
]
```

**Codes:**
- `200 OK` - Succès
- `400 Bad Request` - Catégorie invalide
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/recipes/filter/area?area={area}
Filtre les recettes par zone/cuisine

**Paramètres:**
- `area` (string) - Nom de la zone

**Réponse:**
```json
[
  {
    "idMeal": "52977",
    "strMeal": "...",
    "strArea": "Italian"
  }
]
```

**Codes:**
- `200 OK` - Succès
- `400 Bad Request` - Zone invalide
- `500 Internal Server Error` - Erreur serveur

---

### ❤️ Favorites Controller

#### GET /api/favorites
Récupère tous les favoris de l'utilisateur **[AUTHORIZE]**

**Paramètres:**
- Aucun

**Réponse:**
```json
[
  {
    "favoriteId": "uuid",
    "userId": "auth0|123",
    "recipeId": "52977",
    "recipe": {
      "idMeal": "52977",
      "strMeal": "..."
    }
  }
]
```

**Codes:**
- `200 OK` - Succès
- `401 Unauthorized` - Non authentifié
- `500 Internal Server Error` - Erreur serveur

---

#### POST /api/favorites
Ajoute une recette aux favoris **[AUTHORIZE]**

**Body:**
```json
{
  "recipeId": "52977"
}
```

**Réponse:**
```json
{
  "favoriteId": "uuid",
  "userId": "auth0|123",
  "recipeId": "52977"
}
```

**Codes:**
- `201 Created` - Favori créé
- `400 Bad Request` - Données invalides
- `401 Unauthorized` - Non authentifié
- `409 Conflict` - Déjà en favori
- `500 Internal Server Error` - Erreur serveur

---

#### DELETE /api/favorites/{recipeId}
Supprime une recette des favoris **[AUTHORIZE]**

**Paramètres:**
- `recipeId` (string) - ID de la recette

**Réponse:**
```
204 No Content
```

**Codes:**
- `204 No Content` - Succès
- `401 Unauthorized` - Non authentifié
- `404 Not Found` - Favori non trouvé
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/favorites/{recipeId}/exists
Vérifie si une recette est en favori **[AUTHORIZE]**

**Paramètres:**
- `recipeId` (string) - ID de la recette

**Réponse:**
```json
true  // ou false
```

**Codes:**
- `200 OK` - Succès
- `401 Unauthorized` - Non authentifié
- `500 Internal Server Error` - Erreur serveur

---

### 📧 Contact Controller

#### POST /api/contact
Envoie un message de contact

**Body:**
```json
{
  "email": "user@example.com",
  "subject": "Question",
  "message": "I need help with..."
}
```

**Réponse:**
```json
{
  "success": true,
  "message": "Message sent successfully"
}
```

**Codes:**
- `200 OK` - Message envoyé
- `400 Bad Request` - Données invalides
- `422 Unprocessable Entity` - Validation échouée
- `500 Internal Server Error` - Erreur serveur

---

### 🔐 Auth Controller

#### GET /api/auth/me
Récupère les infos de l'utilisateur connecté **[AUTHORIZE]**

**Paramètres:**
- Aucun

**Réponse:**
```json
{
  "userId": "auth0|123",
  "email": "user@example.com",
  "name": "John Doe"
}
```

**Codes:**
- `200 OK` - Succès
- `401 Unauthorized` - Non authentifié
- `500 Internal Server Error` - Erreur serveur

---

#### GET /api/auth/health
Health check de l'API

**Paramètres:**
- Aucun

**Réponse:**
```json
{
  "status": "healthy",
  "timestamp": "2026-02-24T10:30:00Z"
}
```

**Codes:**
- `200 OK` - API fonctionnelle
- `503 Service Unavailable` - API indisponible

---

## 🔄 Rate Limiting

Actuellement **pas de rate limiting** configuré (À ajouter en production).

---

## 📊 Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Requête réussie |
| 201 | Created | Ressource créée |
| 204 | No Content | Succès sans contenu |
| 400 | Bad Request | Requête invalide |
| 401 | Unauthorized | Authentification requise |
| 404 | Not Found | Ressource non trouvée |
| 409 | Conflict | Ressource en conflit |
| 422 | Unprocessable Entity | Validation échouée |
| 500 | Internal Server Error | Erreur serveur |
| 503 | Service Unavailable | Service indisponible |

---

## 🐛 Error Response Format

```json
{
  "error": "Validation failed",
  "message": "Email is required",
  "statusCode": 400
}
```

---

## 📈 Example Usage

### Recherche et Ajout aux Favoris

```javascript
// 1. Rechercher des recettes
const response = await fetch('http://localhost:5266/api/recipes/search?query=brownie');
const recipes = await response.json();

// 2. Ajouter un favori (authentifié)
const favoriteResponse = await fetch('http://localhost:5266/api/favorites', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    recipeId: recipes[0].idMeal
  })
});

// 3. Récupérer les favoris
const myFavorites = await fetch('http://localhost:5266/api/favorites', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});
```

---

*Last Updated: 2026-02-24*
