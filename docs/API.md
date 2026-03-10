# Documentation API

Référence complète des endpoints disponibles dans l'API MenuMalin.

## Configuration Base

**Base URL**: `https://localhost:7057/api` (développement)

**Headers Requis**:
```
Content-Type: application/json
```

**Headers d'Authentification**:
```
Cookie: .AspNetCore.Cookies=<session_token>
```

Les cookies sont envoyés automatiquement par le navigateur et les clients HTTP configurés avec `credentials: "include"`.

## Format des Réponses

### Succès (2xx)
```json
{
  "data": { ... },
  "success": true,
  "timestamp": "2026-03-10T10:30:00Z"
}
```

### Erreur (4xx, 5xx)
```json
{
  "error": "Erreur descriptive",
  "statusCode": 400,
  "traceId": "0HN7JBE8RN3Q0:00000001",
  "timestamp": "2026-03-10T10:30:00Z"
}
```

## Authentification

### POST /auth/register

Créer un nouveau compte utilisateur.

**Request**:
```bash
curl -X POST https://localhost:7057/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@example.com",
    "password": "SecurePassword123!",
    "nom": "John Doe"
  }' \
  -k
```

**Body**:
| Champ | Type | Requis | Validation |
|-------|------|--------|-----------|
| email | string | ✅ | Format email valide, unique |
| password | string | ✅ | Min 8 chars, 1 majuscule, 1 chiffre, 1 spécial |
| nom | string | ✅ | 2-50 caractères |

**Response** (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "newuser@example.com",
  "nom": "John Doe",
  "dateInscription": "2026-03-10T10:30:00Z"
}
```

**Codes d'erreur**:
- `400` - Email déjà utilisé
- `400` - Password ne respecte pas les critères
- `400` - Validation échouée

---

### POST /auth/login

Authentifier un utilisateur.

**Request**:
```bash
curl -X POST https://localhost:7057/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePassword123!"
  }' \
  -k -c cookies.txt
```

**Body**:
| Champ | Type | Requis |
|-------|------|--------|
| email | string | ✅ |
| password | string | ✅ |

**Response** (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "nom": "John Doe"
}
```

**Cookies définis**:
```
Set-Cookie: .AspNetCore.Cookies=<encrypted_token>; HttpOnly; SameSite=Lax; Max-Age=86400
```

**Codes d'erreur**:
- `401` - Email ou password incorrect
- `400` - Validation échouée

---

### POST /auth/logout

Terminer la session.

**Request**:
```bash
curl -X POST https://localhost:7057/api/auth/logout \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (204 No Content):
```
(vide)
```

---

### GET /auth/current-user

Récupérer l'utilisateur actuellement connecté.

**Request**:
```bash
curl -X GET https://localhost:7057/api/auth/current-user \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "nom": "John Doe",
  "dateInscription": "2026-03-10T10:30:00Z"
}
```

**Codes d'erreur**:
- `401` - Non authentifié

---

## Recettes (TheMealDB)

### GET /recipes/search

Rechercher des recettes par mot-clé.

**Request**:
```bash
curl -X GET "https://localhost:7057/api/recipes/search?query=pasta" -k
```

**Query Parameters**:
| Param | Type | Requis | Exemple |
|-------|------|--------|---------|
| query | string | ✅ | "pasta", "pizza" |

**Response** (200 OK):
```json
[
  {
    "idMeal": "715494",
    "strMeal": "Pasta Carbonara",
    "strMealThumb": "https://www.themealdb.com/images/...",
    "strCategory": "Pasta",
    "strArea": "Italian",
    "ingredients": ["Pasta", "Bacon", "Egg", "Cheese"],
    "measures": ["500g", "200g", "3", "100g"]
  }
]
```

**Codes d'erreur**:
- `400` - Query vide
- `500` - Erreur API TheMealDB

---

### GET /recipes/filter

Filtrer les recettes par catégorie ou région.

**Request**:
```bash
# Par catégorie
curl -X GET "https://localhost:7057/api/recipes/filter?type=category&value=Dessert" -k

# Par région
curl -X GET "https://localhost:7057/api/recipes/filter?type=area&value=Italian" -k
```

**Query Parameters**:
| Param | Type | Valeurs |
|-------|------|---------|
| type | string | "category" ou "area" |
| value | string | Dessert, Pasta, Italian, Chinese, etc. |

**Response** (200 OK):
```json
[
  {
    "idMeal": "715494",
    "strMeal": "Tiramisu",
    "strMealThumb": "https://www.themealdb.com/images/...",
    "strCategory": "Dessert",
    "strArea": "Italian"
  }
]
```

---

### GET /recipes/{mealDBId}

Obtenir les détails complets d'une recette.

**Request**:
```bash
curl -X GET "https://localhost:7057/api/recipes/715494" -k
```

**Path Parameters**:
| Param | Type | Description |
|-------|------|-------------|
| mealDBId | string | ID de la recette sur TheMealDB |

**Response** (200 OK):
```json
{
  "idMeal": "715494",
  "strMeal": "Pasta Carbonara",
  "strMealThumb": "https://...",
  "strCategory": "Pasta",
  "strArea": "Italian",
  "strInstructions": "Faire bouillir l'eau...",
  "ingredients": ["Pasta", "Bacon", "Egg", "Cheese", ...],
  "measures": ["500g", "200g", "3", "100g", ...],
  "strYoutube": "https://youtube.com/watch?v=..."
}
```

---

## Favoris

### GET /favorites

Récupérer tous les favoris de l'utilisateur.

**Request**:
```bash
curl -X GET "https://localhost:7057/api/favorites" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (200 OK):
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "mealDBId": "715494",
    "titre": "Pasta Carbonara",
    "image": "https://...",
    "dateAjout": "2026-03-10T10:30:00Z"
  }
]
```

**Codes d'erreur**:
- `401` - Non authentifié

---

### POST /favorites/{mealDBId}

Ajouter une recette aux favoris.

**Request**:
```bash
curl -X POST "https://localhost:7057/api/favorites/715494" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "mealDBId": "715494",
  "dateAjout": "2026-03-10T10:30:00Z"
}
```

**Codes d'erreur**:
- `401` - Non authentifié
- `409` - Déjà dans les favoris
- `404` - Recette non trouvée

---

### DELETE /favorites/{mealDBId}

Retirer une recette des favoris.

**Request**:
```bash
curl -X DELETE "https://localhost:7057/api/favorites/715494" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (204 No Content):
```
(vide)
```

**Codes d'erreur**:
- `401` - Non authentifié
- `404` - Favori non trouvé

---

### GET /favorites/{mealDBId}/check

Vérifier si une recette est en favori.

**Request**:
```bash
curl -X GET "https://localhost:7057/api/favorites/715494/check" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (200 OK):
```json
{
  "isFavorite": true
}
```

---

## Recettes Personnalisées

### GET /user-recipes

Récupérer les recettes personnelles de l'utilisateur.

**Request**:
```bash
curl -X GET "https://localhost:7057/api/user-recipes" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Query Parameters**:
| Param | Type | Optionnel | Valeur |
|-------|------|-----------|--------|
| includePrivate | boolean | Oui | true/false |

**Response** (200 OK):
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "titre": "Ma Pasta",
    "description": "Recette personnelle",
    "imageUrl": "https://localhost:7057/uploads/recipes/...",
    "instructions": "Mélanger et cuire",
    "estPublique": false,
    "dateCreation": "2026-03-10T10:30:00Z",
    "ingredients": ["Farine", "Eau"],
    "measures": ["500g", "250ml"]
  }
]
```

**Codes d'erreur**:
- `401` - Non authentifié

---

### POST /user-recipes

Créer une nouvelle recette personnalisée.

**Request**:
```bash
curl -X POST "https://localhost:7057/api/user-recipes" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -F "titre=Ma Pasta" \
  -F "instructions=Mélanger et cuire" \
  -F "image=@/path/to/image.jpg" \
  -k
```

**Form Data**:
| Champ | Type | Requis |
|-------|------|--------|
| titre | string | ✅ |
| instructions | string | ✅ |
| description | string | Non |
| image | file (JPG/PNG) | Non |
| ingredients | string[] | Non |
| measures | string[] | Non |
| estPublique | boolean | Non (défaut: false) |

**Response** (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "titre": "Ma Pasta",
  "imageUrl": "https://localhost:7057/uploads/recipes/...",
  "dateCreation": "2026-03-10T10:30:00Z"
}
```

**Codes d'erreur**:
- `401` - Non authentifié
- `400` - Validation échouée
- `413` - Image > 5MB

---

### GET /user-recipes/{id}

Obtenir une recette personnelle spécifique.

**Request**:
```bash
curl -X GET "https://localhost:7057/api/user-recipes/550e8400-e29b-41d4-a716-446655440002" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "titre": "Ma Pasta",
  "description": "Recette personnelle",
  "imageUrl": "https://...",
  "instructions": "Mélanger et cuire",
  "estPublique": false,
  "dateCreation": "2026-03-10T10:30:00Z",
  "dateModification": "2026-03-10T11:30:00Z"
}
```

**Codes d'erreur**:
- `401` - Non authentifié (si privée)
- `403` - Pas l'auteur (si privée)
- `404` - Recette non trouvée

---

### PATCH /user-recipes/{id}

Modifier une recette personnelle.

**Request**:
```bash
curl -X PATCH "https://localhost:7057/api/user-recipes/550e8400-e29b-41d4-a716-446655440002" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -H "Content-Type: application/json" \
  -d '{
    "titre": "Ma Pasta Améliorée",
    "instructions": "Mélanger, cuire et servir",
    "estPublique": true
  }' \
  -k
```

**Response** (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "titre": "Ma Pasta Améliorée",
  "dateModification": "2026-03-10T12:30:00Z"
}
```

**Codes d'erreur**:
- `401` - Non authentifié
- `403` - Pas l'auteur
- `404` - Recette non trouvée

---

### DELETE /user-recipes/{id}

Supprimer une recette personnelle.

**Request**:
```bash
curl -X DELETE "https://localhost:7057/api/user-recipes/550e8400-e29b-41d4-a716-446655440002" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (204 No Content):
```
(vide)
```

**Codes d'erreur**:
- `401` - Non authentifié
- `403` - Pas l'auteur
- `404` - Recette non trouvée

---

## Profil Utilisateur

### GET /users/profile

Récupérer le profil de l'utilisateur connecté.

**Request**:
```bash
curl -X GET "https://localhost:7057/api/users/profile" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -k
```

**Response** (200 OK):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "nom": "John Doe",
  "dateInscription": "2026-03-10T10:30:00Z",
  "dateDerniereConnexion": "2026-03-10T15:30:00Z"
}
```

**Codes d'erreur**:
- `401` - Non authentifié

---

### PATCH /users/profile/name

Modifier le nom d'affichage.

**Request**:
```bash
curl -X PATCH "https://localhost:7057/api/users/profile/name" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -H "Content-Type: application/json" \
  -d '{"nom": "Jane Doe"}' \
  -k
```

**Response** (200 OK):
```json
{
  "nom": "Jane Doe"
}
```

**Codes d'erreur**:
- `401` - Non authentifié
- `400` - Nom invalide

---

### PATCH /users/profile/password

Changer le mot de passe.

**Request**:
```bash
curl -X PATCH "https://localhost:7057/api/users/profile/password" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "OldPassword123!",
    "newPassword": "NewPassword456!"
  }' \
  -k
```

**Response** (200 OK):
```json
{
  "success": true
}
```

**Codes d'erreur**:
- `401` - Non authentifié ou ancien password incorrect
- `400` - Nouveau password ne respecte pas les critères

---

## Upload d'Image

### POST /upload

Télécharger une image pour une recette.

**Request**:
```bash
curl -X POST "https://localhost:7057/api/upload" \
  -H "Cookie: .AspNetCore.Cookies=<token>" \
  -F "file=@/path/to/image.jpg" \
  -k
```

**Form Data**:
| Champ | Type | Requis | Validation |
|-------|------|--------|-----------|
| file | file | ✅ | JPG/PNG, max 5MB |

**Response** (200 OK):
```json
{
  "fileName": "550e8400-e29b-41d4-a716-446655440000_1710000000_image.jpg",
  "filePath": "/uploads/recipes/550e8400-e29b-41d4-a716-446655440000_1710000000_image.jpg",
  "url": "https://localhost:7057/uploads/recipes/550e8400-e29b-41d4-a716-446655440000_1710000000_image.jpg"
}
```

**Codes d'erreur**:
- `401` - Non authentifié
- `400` - Format invalide
- `413` - Fichier trop gros

---

## Contact

### POST /contact

Envoyer un message de contact.

**Request**:
```bash
curl -X POST "https://localhost:7057/api/contact" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "sender@example.com",
    "name": "John Doe",
    "subject": "Suggestion",
    "message": "Il faudrait ajouter une fonctionnalité...",
    "subscribeNewsletter": true
  }' \
  -k
```

**Body**:
| Champ | Type | Requis |
|-------|------|--------|
| email | string | Non |
| name | string | Non |
| subject | string | ✅ |
| message | string | ✅ |
| subscribeNewsletter | boolean | Non |

**Response** (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440010",
  "message": "Message reçu avec succès"
}
```

---

## Codes HTTP Standards

| Code | Signification |
|------|---------------|
| 200 | OK - Requête réussie |
| 201 | Created - Ressource créée |
| 204 | No Content - Suppression réussie |
| 400 | Bad Request - Données invalides |
| 401 | Unauthorized - Non authentifié |
| 403 | Forbidden - Pas de permission |
| 404 | Not Found - Ressource introuvable |
| 409 | Conflict - Ressource existante |
| 413 | Payload Too Large - Fichier trop gros |
| 500 | Internal Server Error - Erreur serveur |

