# 📚 API Documentation - MenuMalin

**Architecture**: Backend for Frontend (BFF) - Tous les appels frontend passent par le backend

---

## 🔐 Authentication Endpoints

### POST `/api/auth/login`
Authentifier avec email + password, retourner un cookie HttpOnly.

**Request**:
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response** (200 OK):
```json
{
  "userId": "user-id-123",
  "email": "user@example.com",
  "name": "John Doe",
  "isAuthenticated": true
}
```

**Errors**:
- 400: Email/password manquants ou format email invalide
- 500: Erreur serveur

**Notes**:
- Crée un cookie HttpOnly, SameSite=Strict automatiquement
- Email peut être créé automatiquement (DEV mode)
- Cookie expiré après 24h

---

### POST `/api/auth/logout`
Déconnecter l'utilisateur courant.

**Request**: (vide)

**Response** (200 OK):
```json
{
  "message": "Déconnecté avec succès"
}
```

**Notes**:
- Supprime le cookie d'authentification
- Frontend doit recharger la page
- Requiert authentification

---

### GET `/api/auth/me`
Récupérer l'utilisateur courant.

**Response** (200 OK):
```json
{
  "userId": "user-id-123",
  "email": "user@example.com",
  "name": "John Doe",
  "isAuthenticated": true
}
```

**Response** (401 Unauthorized):
```json
{
  "isAuthenticated": false
}
```

**Notes**:
- Accessible sans authentification (retourne false si non authentifié)
- Appelé au démarrage pour vérifier l'authentification

---

## 🍽️ Recipe Endpoints (Proxy pour TheMealDB)

### GET `/api/recipes/random`
Récupérer 6 recettes aléatoires.

**Response** (200 OK):
```json
[
  {
    "idMeal": "52772",
    "strMeal": "Teriyaki Chicken Casserole",
    "strMealThumb": "https://www.themealdb.com/images/media/meals/wvpsux1468256321.jpg",
    "strCategory": "Chicken",
    "strArea": "Japanese"
  },
  ...
]
```

**Notes**:
- Appelle TheMealDB API
- Cache les résultats en base de données
- Aucune authentification requise

---

### GET `/api/recipes/search?query={q}`
Rechercher des recettes par nom.

**Parameters**:
- `query` (required): Terme de recherche

**Example**:
```
GET /api/recipes/search?query=pizza
```

**Response** (200 OK): Liste de recettes correspondant au terme

**Errors**:
- 400: `query` vide

---

### GET `/api/recipes/{mealId}`
Récupérer les détails complets d'une recette.

**Parameters**:
- `mealId` (required): ID TheMealDB

**Response** (200 OK):
```json
{
  "idMeal": "52772",
  "strMeal": "Teriyaki Chicken Casserole",
  "strMealThumb": "https://www.themealdb.com/images/media/meals/wvpsux1468256321.jpg",
  "strCategory": "Chicken",
  "strArea": "Japanese",
  "strInstructions": "...",
  "strIngredient1": "Chicken breast",
  "strMeasure1": "500g",
  ...
}
```

**Errors**:
- 404: Recette non trouvée

---

### GET `/api/recipes/categories/list`
Récupérer la liste de toutes les catégories.

**Response** (200 OK):
```json
[
  "Seafood",
  "Breakfast",
  "Vegan",
  "Dessert",
  ...
]
```

---

### GET `/api/recipes/areas/list`
Récupérer la liste de toutes les zones/cuisines.

**Response** (200 OK):
```json
[
  "American",
  "British",
  "French",
  "Japanese",
  "Italian",
  ...
]
```

---

### GET `/api/recipes/filter/category?category={c}`
Filtrer les recettes par catégorie.

**Parameters**:
- `category` (required): Nom de la catégorie

**Example**:
```
GET /api/recipes/filter/category?category=Seafood
```

**Response** (200 OK): Liste des recettes de la catégorie

**Errors**:
- 400: Catégorie vide

---

### GET `/api/recipes/filter/area?area={a}`
Filtrer les recettes par zone/cuisine.

**Parameters**:
- `area` (required): Nom de la zone

**Example**:
```
GET /api/recipes/filter/area?area=French
```

**Response** (200 OK): Liste des recettes de la zone

**Errors**:
- 400: Zone vide

---

## ❤️ Favorites Endpoints

### GET `/api/favorites`
Récupérer tous les favoris de l'utilisateur courant.

**Response** (200 OK):
```json
[
  {
    "favoritesId": "fav-1",
    "recipeId": "52772",
    "userId": "user-id-123",
    "recipeTitle": "Teriyaki Chicken Casserole",
    ...
  },
  ...
]
```

**Errors**:
- 401: Non authentifié

**Notes**:
- Requiert authentification
- Retourne les recettes cachées + leurs détails actuels

---

### POST `/api/favorites`
Ajouter une recette aux favoris.

**Request**:
```json
{
  "recipeId": "52772"
}
```

**Response** (200 OK):
```json
{
  "favoritesId": "fav-1",
  "recipeId": "52772",
  "userId": "user-id-123",
  ...
}
```

**Errors**:
- 400: `recipeId` manquant
- 401: Non authentifié
- 500: Erreur serveur (recette introuvable, etc.)

---

### DELETE `/api/favorites/{recipeId}`
Supprimer une recette des favoris.

**Parameters**:
- `recipeId` (required): ID de la recette à supprimer

**Response** (200 OK):
```json
{
  "message": "Favori supprimé avec succès"
}
```

**Errors**:
- 401: Non authentifié
- 404: Favori non trouvé

---

### GET `/api/favorites/{recipeId}/exists`
Vérifier si une recette est dans les favoris.

**Parameters**:
- `recipeId` (required): ID de la recette

**Response** (200 OK):
```json
{
  "isFavorite": true
}
```

**Errors**:
- 401: Non authentifié

---

## 📝 User Recipes Endpoints

### POST `/api/userrecipes`
Créer une nouvelle recette personnalisée.

**Request**:
```json
{
  "title": "Ma Recette",
  "instructions": "1. Faire ceci\n2. Faire cela",
  "imageUrl": "/uploads/img.jpg",
  "ingredients": ["Ingrédient 1", "Ingrédient 2"]
}
```

**Response** (201 Created):
```json
{
  "userRecipeId": "recipe-123",
  "userId": "user-id-123",
  "title": "Ma Recette",
  ...
}
```

**Errors**:
- 400: Données invalides
- 401: Non authentifié

---

### GET `/api/userrecipes/my`
Récupérer toutes mes recettes personnalisées.

**Response** (200 OK): Liste des recettes de l'utilisateur

**Errors**:
- 401: Non authentifié

---

### GET `/api/userrecipes/public`
Récupérer toutes les recettes publiques de la communauté.

**Response** (200 OK): Liste des recettes publiques

**Notes**:
- Aucune authentification requise

---

### GET `/api/userrecipes/{id}`
Récupérer les détails d'une recette.

**Parameters**:
- `id` (required): ID de la recette

**Response** (200 OK): Détails complets de la recette

**Errors**:
- 404: Recette non trouvée

---

### DELETE `/api/userrecipes/{id}`
Supprimer une recette (seul le propriétaire peut).

**Parameters**:
- `id` (required): ID de la recette

**Response** (200 OK):
```json
{
  "message": "Recette supprimée avec succès"
}
```

**Errors**:
- 401: Non authentifié
- 403: Non propriétaire
- 404: Recette non trouvée

---

### PATCH `/api/userrecipes/{id}`
Mettre à jour une recette (seul le propriétaire peut).

**Parameters**:
- `id` (required): ID de la recette

**Request**:
```json
{
  "title": "Titre modifié",
  "instructions": "Instructions modifiées",
  ...
}
```

**Response** (200 OK): Recette modifiée

**Errors**:
- 401: Non authentifié
- 403: Non propriétaire
- 404: Recette non trouvée

---

### PATCH `/api/userrecipes/{id}/visibility`
Basculer la visibilité (public/privé) d'une recette.

**Parameters**:
- `id` (required): ID de la recette

**Request**:
```json
{
  "isPublic": true
}
```

**Response** (200 OK):
```json
{
  "message": "Visibilité mise à jour. La recette est maintenant publique"
}
```

**Errors**:
- 401: Non authentifié
- 403: Non propriétaire
- 404: Recette non trouvée

---

## 📤 Upload Endpoints

### POST `/api/upload/recipe-image`
Uploader une image de recette.

**Request**: Form data avec fichier image

**Response** (200 OK):
```json
{
  "imageUrl": "/uploads/uuid.jpg",
  "message": "Image uploadée avec succès"
}
```

**Errors**:
- 400: Fichier manquant, trop gros, ou format non supporté
- 401: Non authentifié
- 500: Erreur serveur

**Notes**:
- Max 5MB
- Formats: JPEG, PNG, GIF, WebP
- Validation magic bytes
- Requiert authentification

---

## 🔐 Authentication Flow

```
1. Frontend → POST /api/auth/login (email, password)
2. Backend → Crée utilisateur si nécessaire
3. Backend → SignInAsync() → Cookie HttpOnly
4. Frontend ← Reçoit cookie automatiquement
5. Frontend → GET /api/auth/me (avec cookie)
6. Backend ← Verify cookie claims
7. Frontend ← { userId, email, name, isAuthenticated: true }
```

---

## 📊 HTTP Status Codes

| Code | Signification |
|------|---------------|
| 200  | OK - Requête réussie |
| 201  | Created - Ressource créée |
| 400  | Bad Request - Paramètres invalides |
| 401  | Unauthorized - Non authentifié |
| 403  | Forbidden - Non autorisé (permission insuffisante) |
| 404  | Not Found - Ressource non trouvée |
| 500  | Internal Server Error - Erreur serveur |

---

## 🛡️ Security

✅ **Authentification Cookie HttpOnly**
- Cookies HttpOnly: Impossible d'accéder via JavaScript
- SameSite=Strict: Protection CSRF
- Expiré après 24h

✅ **Validation des entrées**
- Email: Format validation (@)
- Password: Non-vide
- Upload: Magic bytes + MIME type

✅ **CORS**
- Frontend-Backend: Même origin (BFF architecture)
- Pas de requêtes cross-origin

---

## 📝 Notes d'implémentation

- **Authentification DEV**: Accept any email/password (no bcrypt check)
- **Authentification PROD**: À implémenter avec bcrypt.VerifyHashedPassword()
- **Rate limiting**: À implémenter sur `/api/auth/login`
- **Caching**: Les recettes sont cachées après fetch TheMealDB

---

**Dernière mise à jour**: 2026-02-27
**Statut**: Documenté pour Phase 7
