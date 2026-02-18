# Guide Complet de l'API TheMealDB pour Blazor

## Table des Matières
1. [Introduction à TheMealDB](#introduction)
2. [Endpoints Disponibles](#endpoints)
3. [Comment Appeler Chaque Endpoint](#appels)
4. [Structures de Données](#structures)
5. [Limites et Rate Limiting](#limites)
6. [Gestion des Erreurs](#erreurs)
7. [Stratégies de Caching](#caching)
8. [Tests avec Postman/Thunder Client](#tests)

---

## Introduction à TheMealDB {#introduction}

### Qu'est-ce que TheMealDB?

TheMealDB est une **base de données gratuite et ouverte contenant des informations sur les recettes culinaires du monde entier**. L'API fournit un accès à une collection massive de repas, d'ingrédients, de catégories et d'informations nutritionnelles.

### Caractéristiques Principales

| Caractéristique | Description |
|---|---|
| **Type de Service** | API REST gratuite, aucune authentification requise |
| **Format de Données** | JSON |
| **Protocole** | HTTPS |
| **Domaine** | www.themealdb.com |
| **Base de Données** | Plus de 1000 recettes complètes |
| **Couverture Géographique** | Cuisine mondiale (italienne, indienne, chinoise, etc.) |
| **Mise à Jour** | Régulièrement mise à jour avec de nouvelles recettes |

### Cas d'Usage Typiques

- Applications de recettes de cuisine
- Planification des repas
- Exploration culinaire internationale
- Apprentissage culinaire interactif
- Recommandations de plats
- Applications de regime ou nutritionnelles

---

## Endpoints Disponibles {#endpoints}

### Vue d'Ensemble des Endpoints

TheMealDB propose plusieurs catégories d'endpoints permettant de rechercher, filtrer et récupérer des informations sur les repas.

| Catégorie | Endpoints | Description |
|---|---|---|
| **Recherche** | Search by Name<br>Search by First Letter<br>Search by Ingredient | Trouver des repas via différents critères |
| **Filtrage** | Filter by Category<br>Filter by Area<br>Filter by Ingredient | Filtrer par classification |
| **Détails** | Lookup Full Meal Details<br>Random Meal | Récupérer les informations complètes |
| **Listes** | List Categories<br>List Areas<br>List Ingredients | Récupérer les listes de classification |
| **Recherche Avancée** | Search by ID | Récupérer un repas spécifique par identifiant |

### Structure d'URL de Base

```
https://www.themealdb.com/api/json/v1/1/[ENDPOINT]
```

**Exemple:**
```
https://www.themealdb.com/api/json/v1/1/search.php?s=Arrabiata
```

---

## Comment Appeler Chaque Endpoint {#appels}

### 1. Recherche par Nom (Search by Name)

**Endpoint:** `search.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `s` | Nom du repas | String | Oui | Nom du plat à rechercher (minimum 1 caractère) |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/search.php?s=Arrabiata
```

**Description:** Recherche une recette par son nom exact ou partiel. La recherche retourne tous les repas correspondant au nom.

**Exemple de Cas d'Usage:** Un utilisateur tape "Pâtes" pour trouver toutes les recettes contenant ce mot.

---

### 2. Recherche par Première Lettre (Search by First Letter)

**Endpoint:** `search.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `f` | Première lettre | String | Oui | Une lettre unique (A-Z) pour filtrer les repas commençant par cette lettre |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/search.php?f=a
```

**Description:** Retourne tous les repas commençant par une lettre spécifique. Utile pour créer une navigation alphabétique.

**Exemple de Cas d'Usage:** Afficher un menu A-Z où chaque lettre contient une liste de repas.

---

### 3. Recherche par Ingrédient (Search by Ingredient)

**Endpoint:** `filter.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `i` | Ingrédient | String | Oui | Nom de l'ingrédient à rechercher |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/filter.php?i=Chicken
```

**Description:** Retourne tous les repas contenant un ingrédient spécifique. Retour limité avec informations de base (ID, nom, image).

**Exemple de Cas d'Usage:** Afficher tous les plats à base de poulet.

---

### 4. Filtrer par Catégorie (Filter by Category)

**Endpoint:** `filter.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `c` | Catégorie | String | Oui | Nom de la catégorie (ex: Seafood, Breakfast, Dessert) |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/filter.php?c=Seafood
```

**Description:** Filtre tous les repas appartenant à une catégorie spécifique.

**Exemple de Cas d'Usage:** Afficher tous les fruits de mer disponibles.

---

### 5. Filtrer par Zone Géographique (Filter by Area)

**Endpoint:** `filter.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `a` | Zone/Région | String | Oui | Nom du pays ou de la région culinaire |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/filter.php?a=Italian
```

**Description:** Retourne tous les repas appartenant à une cuisine géographique spécifique.

**Exemple de Cas d'Usage:** Afficher toutes les recettes italiennes.

---

### 6. Consulter Détails Complets du Repas (Lookup Full Meal Details)

**Endpoint:** `lookup.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `i` | ID du Repas | Integer | Oui | Identifiant unique du repas |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/lookup.php?i=52772
```

**Description:** Retourne **TOUS** les détails d'un repas spécifique incluant instructions, ingrédients, mesures, vidéo YouTube, source.

**Exemple de Cas d'Usage:** Afficher la page de détail complète d'une recette après avoir cliqué sur un repas de la liste.

---

### 7. Repas Aléatoire (Random Meal)

**Endpoint:** `random.php`

**Paramètres:** Aucun

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/random.php
```

**Description:** Retourne un repas aléatoire avec tous ses détails complets. Chaque appel retourne un repas différent.

**Exemple de Cas d'Usage:** Bouton "Surprise moi!" qui suggère une recette aléatoire.

---

### 8. Lister Toutes les Catégories (List Categories)

**Endpoint:** `list.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `c` | Liste | String | Oui | Doit être exactement "list" |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/list.php?c=list
```

**Description:** Retourne la liste complète de toutes les catégories de repas disponibles.

**Exemple de Cas d'Usage:** Remplir une liste déroulante avec toutes les catégories disponibles.

---

### 9. Lister Toutes les Zones (List Areas)

**Endpoint:** `list.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `a` | Liste | String | Oui | Doit être exactement "list" |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/list.php?a=list
```

**Description:** Retourne la liste complète de toutes les zones géographiques/cuisines disponibles.

**Exemple de Cas d'Usage:** Afficher toutes les cuisines du monde disponibles dans l'application.

---

### 10. Lister Tous les Ingrédients (List Ingredients)

**Endpoint:** `list.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `i` | Liste | String | Oui | Doit être exactement "list" |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/list.php?i=list
```

**Description:** Retourne la liste complète de tous les ingrédients disponibles dans la base de données.

**Exemple de Cas d'Usage:** Créer un système d'autocomplétion ou un filtre d'ingrédients.

---

### 11. Rechercher par ID (Lookup by ID)

**Endpoint:** `lookup.php`

**Paramètres:**
| Paramètre | Nom | Type | Obligatoire | Description |
|---|---|---|---|---|
| `i` | ID | Integer | Oui | Identifiant numérique unique du repas |

**Appel:**
```
GET https://www.themealdb.com/api/json/v1/1/lookup.php?i=52772
```

**Description:** Identique à "Consulter Détails Complets du Repas". Retourne tous les détails d'une recette via son ID.

**Exemple de Cas d'Usage:** Récupérer les détails complets après avoir stocké uniquement l'ID dans le panier ou favoris.

---

## Structures de Données {#structures}

### Structure de Réponse: Détails Complets du Repas

Ceci est la structure retournée par les endpoints `lookup.php` et `random.php`:

```
Réponse Principale
├── meals (Array)
│   └── [0] (Object - Le Repas)
│       ├── idMeal (String) - Identifiant unique
│       ├── strMeal (String) - Nom du repas
│       ├── strDrinkAlternate (String/Null) - Boisson alternative
│       ├── strCategory (String) - Catégorie (Seafood, Breakfast, etc)
│       ├── strArea (String) - Zone culinaire (Italian, Indian, etc)
│       ├── strInstructions (String) - Instructions de préparation complètes
│       ├── strMealThumb (String) - URL de l'image du plat
│       ├── strTags (String) - Tags séparés par virgules
│       ├── strYoutube (String) - URL vidéo YouTube
│       ├── strSource (String) - Source/URL d'origine
│       ├── strImageSource (String/Null) - Source de l'image
│       ├── strCreativeCommonsConfirmed (String/Null) - Confirmé CC
│       ├── dateModified (String) - Date dernière modification
│       └── Ingrédients et Mesures (Dynamiques)
│           ├── strIngredient1 à strIngredient20 (String)
│           └── strMeasure1 à strMeasure20 (String)
```

### Structure de Réponse: Résultats de Recherche Simple

Structure retournée par `filter.php` (résumé):

```
Réponse
├── meals (Array)
│   └── [0] (Object)
│       ├── strMeal (String) - Nom du repas
│       ├── strMealThumb (String) - URL image
│       └── idMeal (String) - Identifiant
```

### Structure de Réponse: Liste des Catégories

Structure retournée par `list.php?c=list`:

```
Réponse
├── meals (Array)
│   └── [0] (Object)
│       ├── strCategory (String) - Nom de la catégorie
│       ├── strCategoryThumb (String) - URL image catégorie
│       └── strCategoryDescription (String) - Description détaillée
```

### Structure de Réponse: Liste des Zones

Structure retournée par `list.php?a=list`:

```
Réponse
├── meals (Array)
│   └── [0] (Object)
│       └── strArea (String) - Nom de la zone/cuisine
```

### Structure de Réponse: Liste des Ingrédients

Structure retournée par `list.php?i=list`:

```
Réponse
├── meals (Array)
│   └── [0] (Object)
│       ├── idIngredient (String) - Identifiant ingrédient
│       ├── strIngredient (String) - Nom ingrédient
│       ├── strDescription (String) - Description
│       └── strType (String) - Type (Seasoning, Spice, etc)
```

### Tableau de Mappage des Ingrédients et Mesures

| Ingrédient | Mesure | Ingrédient | Mesure |
|---|---|---|---|
| strIngredient1 | strMeasure1 | strIngredient11 | strMeasure11 |
| strIngredient2 | strMeasure2 | strIngredient12 | strMeasure12 |
| strIngredient3 | strMeasure3 | strIngredient13 | strMeasure13 |
| strIngredient4 | strMeasure4 | strIngredient14 | strMeasure14 |
| strIngredient5 | strMeasure5 | strIngredient15 | strMeasure15 |
| strIngredient6 | strMeasure6 | strIngredient16 | strMeasure16 |
| strIngredient7 | strMeasure7 | strIngredient17 | strMeasure17 |
| strIngredient8 | strMeasure8 | strIngredient18 | strMeasure18 |
| strIngredient9 | strMeasure9 | strIngredient19 | strMeasure19 |
| strIngredient10 | strMeasure10 | strIngredient20 | strMeasure20 |

### Valeurs Possibles pour strType (Ingredients)

- Seasoning
- Spice
- Produce
- Dairy
- Protein
- Seafood
- Grain
- Cooking Oil
- etc.

### Valeurs Possibles pour strCategory

- Seafood
- Breakfast
- Lunch
- Dessert
- Pasta
- Vegan
- Vegetarian
- Side
- Miscellaneous
- Starter

### Valeurs Possibles pour strArea (Zone Culinaire)

- American
- British
- Canadian
- Chinese
- French
- Indian
- Italian
- Japanese
- Korean
- Mexican
- Polish
- Portuguese
- Russian
- Spanish
- Thai
- Turkish
- Vietnamese
- etc.

---

## Limites et Rate Limiting {#limites}

### Limites de Requêtes

| Aspect | Limite | Notes |
|---|---|---|
| **Requêtes par Minute** | Pas de limite officielle publiée | L'API est gratuite et stable |
| **Requêtes par Jour** | Pas de limite officielle publiée | Conçue pour l'usage généreux |
| **Taille de Réponse** | Variable | Dépend de l'endpoint et des filtres |
| **Timeout** | 30 secondes (typique) | Adapter selon le réseau |
| **Format** | JSON uniquement | Pas d'autres formats disponibles |

### Recommandations de Rate Limiting pour Blazor

1. **Implémentation Locale de Throttling:**
   - Limiter les requêtes utilisateur à 1 par seconde pour les recherches
   - Ajouter un délai de 500ms entre les requêtes successives
   - Implémenter une file d'attente pour les requêtes en arrière-plan

2. **Identification des Appels Coûteux:**
   - Requête de liste complète d'ingrédients (gros volume)
   - Recherche par première lettre (peut retourner 100+ résultats)
   - Multiples recherches simultanées

3. **Bonnes Pratiques:**
   - Utiliser le caching aggressif (voir section Caching)
   - Charger les listes une seule fois au démarrage
   - Implémenter la pagination côté client
   - Débouncer les champs de recherche en temps réel

### Considérations de Performance

| Scenario | Impact | Solution |
|---|---|---|
| Recherche en temps réel | Appels réseau fréquents | Débouncer (300-500ms) |
| Chargement de 1000 ingrédients | Gros volume de données | Cache local et pagination |
| Filtres multiples combinés | Plusieurs requêtes | Combiner côté client |
| Image des repas | Latence réseau d'images | Utiliser un CDN, lazy loading |

---

## Gestion des Erreurs {#erreurs}

### Types d'Erreurs Possibles

#### 1. Pas de Résultats Trouvés

**Réponse de l'API:**
```
Statut HTTP: 200 OK
Corps: null
```

**Signification:** La recherche n'a retourné aucun résultat mais l'appel est valide.

**Exemple:** Rechercher un repas avec un nom qui n'existe pas.

**Gestion Recommandée:** Afficher un message "Aucun résultat trouvé" avec des suggestions.

---

#### 2. Paramètre Manquant ou Invalide

**Cas de l'API:**
- Recherche par lettre avec caractères invalides (non alphabétique)
- Paramètre mal formaté dans l'URL
- Paramètre manquant obligatoire

**Gestion Recommandée:**
- Valider tous les paramètres côté client avant d'appeler l'API
- Utiliser des listes prédéfinies pour les énumérations (catégories, zones)
- Nettoyer et encoder les chaînes de recherche

---

#### 3. Erreur Réseau

**Causes Possibles:**
- Pas de connexion Internet
- Timeout de la connexion
- Serveur TheMealDB temporairement indisponible
- Bloc CORS (rare pour cette API)

**Codes HTTP Possibles:**
- 408: Timeout
- 500: Erreur serveur
- 503: Service indisponible
- 0: Erreur réseau (dans le navigateur)

**Gestion Recommandée:**
- Afficher un message d'erreur clair à l'utilisateur
- Implémenter un système de retry avec backoff exponentiel
- Proposer à l'utilisateur de réessayer manuellement
- Utiliser les données en cache si disponibles

---

#### 4. Erreur CORS (Cross-Origin Resource Sharing)

**Symptôme:** Erreur "Access to XMLHttpRequest blocked by CORS policy"

**Cause:** L'API est appelée depuis un domaine différent.

**Gestion Recommandée pour Blazor:**
- TheMealDB supporte CORS, donc pas de problème en général
- Si rencontre CORS: utiliser un proxy CORS ou backend personnel
- Vérifier les en-têtes CORS de la réponse

---

#### 5. Réponse Corrompue ou Incomplète

**Causes:**
- Données JSON mal formées
- Connexion interrompue
- Encodage de caractères incorrect

**Gestion Recommandée:**
- Implémenter un parser JSON robuste
- Ajouter des vérifications de champs obligatoires
- Implémenter un timeout global sur toutes les requêtes
- Logger les erreurs pour debugging

---

### Stratégie Globale de Gestion d'Erreurs pour Blazor

| Étape | Action |
|---|---|
| 1. Validation Entrée | Valider les paramètres avant d'appeler l'API |
| 2. Try-Catch Réseau | Capturer les exceptions de requête HTTP |
| 3. Vérifier Statut HTTP | Vérifier statusCode (200, 4xx, 5xx) |
| 4. Parser Réponse | Essayer de parser JSON, gérer les erreurs |
| 5. Vérifier Données | Vérifier que les champs obligatoires existent |
| 6. Fallback | Utiliser le cache ou afficher un message |
| 7. Notifier Utilisateur | Afficher une erreur conviviale à l'utilisateur |
| 8. Logger | Enregistrer l'erreur complète pour debugging |

### Codes d'Erreur à Anticiper

| Situation | Gestion |
|---|---|
| **Réponse null** | Recherche valide sans résultats - afficher "Aucun résultat" |
| **JSON invalide** | Erreur parsage - afficher "Erreur de données" |
| **Timeout** | Pas de réponse - offrir retry manuel |
| **Serveur 5xx** | Erreur serveur - offrir retry automatique |
| **Pas de connexion** | Mode offline - utiliser cache ou afficher offline |

---

## Caching Recommandé {#caching}

### Stratégie de Caching Multi-Niveaux pour Blazor

### Niveau 1: Cache Très Court Terme (Session Memory)

**Durée:** 1-5 minutes
**Stockage:** Memory cache dans le composant Blazor
**Données à Cacher:**
- Dernière recherche effectuée (5 min)
- Résultats de filtrage précédents (3 min)
- Détails du repas actuellement affiché (5 min)

**Bénéfice:** Éviter les requêtes identiques rapides lors de navigations rapides.

---

### Niveau 2: Cache Court Terme (SessionStorage du Navigateur)

**Durée:** Session complète (tant que le navigateur est ouvert)
**Stockage:** sessionStorage JavaScript
**Données à Cacher:**
- Liste de toutes les catégories (rechargée à chaque session)
- Liste de toutes les zones/cuisines (rechargée à chaque session)
- Historique de recherches récentes (10 dernières)
- Repas favoris de la session actuelle

**Bénéfice:** Partage les données entre navigations, une seule requête par session.

---

### Niveau 3: Cache Long Terme (LocalStorage du Navigateur)

**Durée:** Plusieurs jours à plusieurs semaines
**Stockage:** localStorage JavaScript avec versioning
**Données à Cacher:**
- Liste des catégories (1-2 semaines)
- Liste des zones géographiques (1-2 semaines)
- Liste des ingrédients (version number pour invalidation)
- Repas favoris de l'utilisateur (infini)
- Historique des recherches (30 dernières)

**Bénéfice:** Réduit drastiquement les requêtes API au fil du temps.

---

### Niveau 4: Cache IndexedDB (Large Scale)

**Durée:** Persistant
**Stockage:** Base de données IndexedDB
**Données à Cacher:**
- Tous les repas consultés (tous les détails)
- Index par catégorie, zone, ingrédient
- Statistiques d'utilisation

**Bénéfice:** Application offline capable, navigation instantanée.

**Applicable:** Si l'application consomme vraiment beaucoup l'API.

---

### Stratégie de Invalidation du Cache

| Type de Données | Invalidation |
|---|---|
| **Catégories/Zones** | Jamais (ou avec numéro de version) |
| **Ingrédients** | Hebdomadaire |
| **Listes de Recherche** | Après 3 jours ou manuellement |
| **Détails Repas** | Après 7 jours ou manuellement |
| **Favoris** | Jamais (sauf suppression manuelle) |
| **Historique** | Après 30 jours |

### Implémentation du Versioning

**Concept:** Ajouter un numéro de version aux données en cache.

| Donnée | Version Actuelle |
|---|---|
| Categories | 1 |
| Areas | 1 |
| Ingredients | 1.5 |

Lorsque la version augmente, l'ancien cache est invalide et une nouvelle requête est faite.

---

### Points Critiques pour le Caching Blazor

1. **Savoir Quoi Cacher:**
   - Données statiques ou rarement changeantes = priorité haute
   - Données fréquemment réutilisées = priorité haute
   - Données volumineuses = priorité haute

2. **Savoir Quand Cacher:**
   - Après le premier chargement avec succès
   - Avec un timestamp pour invalidation future
   - Avec une clé unique et déterministe

3. **Savoir Où Cacher:**
   - Préférer localStorage pour données persistantes
   - Préférer sessionStorage pour session courante
   - Préférer Memory pour données très temporaires

4. **Stratégie de Fallback:**
   - Si cache expiré ET pas de connexion réseau = utiliser cache expiré
   - Si pas de cache ET pas de connexion = afficher offline
   - Si réponse API en erreur = utiliser cache ancien même expiré

---

### Tableau de Stratégie de Caching Recommandée

| Endpoint | Cache | Durée | Type |
|---|---|---|---|
| `list.php?c=list` | OUI | 14 jours | localStorage |
| `list.php?a=list` | OUI | 14 jours | localStorage |
| `list.php?i=list` | OUI | 7 jours | localStorage |
| `search.php?s=` | OUI | 1 jour | localStorage |
| `search.php?f=` | OUI | 3 jours | localStorage |
| `filter.php?i=` | OUI | 3 jours | localStorage |
| `filter.php?c=` | OUI | 3 jours | localStorage |
| `filter.php?a=` | OUI | 3 jours | localStorage |
| `lookup.php?i=` | OUI | 30 jours | localStorage |
| `random.php` | NON | N/A | N/A |

---

## Tests avec Postman/Thunder Client {#tests}

### Prérequis

**Logiciels Nécessaires:**
- Postman (https://www.postman.com/downloads/) OU
- Thunder Client (Extension VS Code gratuite) OU
- Insomnia (https://insomnia.rest/)

**Niveau Technique:** Aucun requis, tous les endpoints sont publics.

### Étape 1: Configurer l'Environnement Postman

#### Configuration Basique

1. **Créer une Nouvelle Collection:**
   - Cliquer sur "New" → "Collection"
   - Nommer: "TheMealDB API Tests"
   - Ajouter une description: "Tests complets de l'API TheMealDB"

2. **Créer des Variables d'Environnement:**
   - Cliquer sur "Manage Environments" → "Create"
   - Nommer l'environnement: "TheMealDB"
   - Ajouter les variables:
     - `base_url`: https://www.themealdb.com/api/json/v1/1
     - `meal_id`: 52772
     - `search_query`: Arrabiata

3. **Utiliser les Variables dans les Requêtes:**
   - À la place de l'URL complète, utiliser: `{{base_url}}/search.php?s=Arrabiata`

---

### Étape 2: Tester Chaque Endpoint

#### Test 1: Recherche par Nom (Search by Name)

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/search.php?s=Arrabiata |
| **Header Requis** | Aucun |
| **Body** | Vide (GET) |
| **Résultat Attendu** | Statut 200, array de repas avec Arrabiata |

**Étapes:**
1. Créer nouvelle requête POST dans Postman
2. Coller l'URL: `https://www.themealdb.com/api/json/v1/1/search.php?s=Arrabiata`
3. Cliquer Send
4. Vérifier la réponse contient des repas

**Éléments à Vérifier:**
- Code HTTP 200 OK
- Réponse contient un array "meals"
- Chaque élément a "strMeal", "strMealThumb", "idMeal"
- Pas de valeurs null pour les champs obligatoires

---

#### Test 2: Recherche par Première Lettre

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/search.php?f=a |
| **Résultat Attendu** | Tous les repas commençant par "a" |

**Variantes à Tester:**
- Lettre majuscule: `?f=A`
- Lettre minuscule: `?f=a`
- Caractère invalide: `?f=1` (doit retourner null)

---

#### Test 3: Recherche par Ingrédient

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/filter.php?i=Chicken |
| **Résultat Attendu** | Tous les plats contenant du poulet |

**Tester Aussi:**
- Ingrédient existant: `?i=Tomato`
- Ingrédient inexistant: `?i=XYZ123` (doit retourner null)

---

#### Test 4: Filtrer par Catégorie

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/filter.php?c=Seafood |
| **Résultat Attendu** | Tous les fruits de mer |

**Valeurs à Tester:**
- `Seafood`
- `Breakfast`
- `Dessert`
- `Pasta`
- `Vegan`

---

#### Test 5: Filtrer par Zone Géographique

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/filter.php?a=Italian |
| **Résultat Attendu** | Tous les repas italiens |

**Valeurs à Tester:**
- `Italian`
- `Indian`
- `Chinese`
- `French`
- `Mexican`

---

#### Test 6: Détails Complets du Repas

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/lookup.php?i=52772 |
| **Résultat Attendu** | Détails complets incluant instructions, ingrédients |

**À Vérifier:**
- Présence de strInstructions (texte long)
- 20 paires d'ingrédients/mesures
- strYoutube est une URL valide
- strMealThumb est une URL image valide

---

#### Test 7: Repas Aléatoire

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/random.php |
| **Résultat Attendu** | Un repas aléatoire différent à chaque appel |

**À Vérifier:**
- Appeler 3 fois et vérifier que les IDs sont différents
- Chaque réponse a la structure complète du repas

---

#### Test 8: Lister Toutes les Catégories

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/list.php?c=list |
| **Résultat Attendu** | Array avec 14-15 catégories |

**À Vérifier:**
- strCategory contient: Seafood, Breakfast, Dessert, Pasta, Vegan, etc.
- Chaque catégorie a une image (strCategoryThumb)
- Chaque catégorie a une description

---

#### Test 9: Lister Toutes les Zones

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/list.php?a=list |
| **Résultat Attendu** | Array avec 25+ zones géographiques |

**À Vérifier:**
- Nombre de zones (environ 25-30)
- Chaque zone a un nom valide (Italian, Indian, etc)

---

#### Test 10: Lister Tous les Ingrédients

| Propriété | Valeur |
|---|---|
| **Méthode** | GET |
| **URL** | {{base_url}}/list.php?i=list |
| **Résultat Attendu** | Array avec 600+ ingrédients |

**Note:** Cette requête retourne un gros volume de données.

**À Vérifier:**
- Volume de données important (plusieurs secondes possibles)
- Chaque ingrédient a idIngredient, strIngredient, strDescription

---

### Étape 3: Tester les Scénarios d'Erreur

#### Scénario 1: Recherche sans Résultats

**Requête:**
```
GET https://www.themealdb.com/api/json/v1/1/search.php?s=ZZZZZZZZNOTEXIST
```

**Résultat Attendu:**
- Statut: 200 OK
- Corps: null (ou pas de propriété meals)

**Apprentissage:** L'API retourne 200 même sans résultats, pas 404.

---

#### Scénario 2: Paramètre Invalide

**Requête:**
```
GET https://www.themealdb.com/api/json/v1/1/search.php?f=123
```

**Résultat Attendu:**
- Statut: 200 OK
- Corps: null

**Apprentissage:** L'API valide peu, c'est au client de valider.

---

#### Scénario 3: URL Malformée

**Requête:**
```
GET https://www.themealdb.com/api/json/v1/1/search.php
```

(Sans paramètre "s")

**Résultat Attendu:**
- Statut: 200 OK
- Corps: null

---

#### Scénario 4: Endpoint Inexistant

**Requête:**
```
GET https://www.themealdb.com/api/json/v1/1/invalid.php
```

**Résultat Attendu:**
- Statut: 404 Not Found OU erreur HTTP

---

### Étape 4: Vérifier les En-têtes de Réponse

Pour chaque requête, inspecter les en-têtes de réponse:

| En-tête | Signification | Valeur Attendue |
|---|---|---|
| `Content-Type` | Format de données | `application/json` |
| `Content-Length` | Taille de réponse | Varie |
| `Server` | Serveur web | Généralement Apache |
| `Access-Control-Allow-Origin` | Support CORS | `*` (accepte tous) |
| `Cache-Control` | Directives de cache | Varie |
| `Date` | Timestamp serveur | Heure actuelle |

**À Vérifier:** Pas d'erreurs 4xx ou 5xx, Content-Type est JSON, CORS headers présents.

---

### Étape 5: Tester les Performances

#### Temps de Réponse Normal

| Endpoint | Temps Attendu |
|---|---|
| `search.php?s=` | 300-800ms |
| `filter.php?c=` | 300-800ms |
| `lookup.php?i=` | 300-800ms |
| `random.php` | 300-800ms |
| `list.php?c=list` | 300-800ms |
| `list.php?a=list` | 300-800ms |
| `list.php?i=list` | 1-3 secondes (données volumineuses) |

#### Comment Mesurer dans Postman

1. Regarder le bottom de la fenêtre de réponse
2. Trouver: "Time: XXXms"
3. Enregistrer pour tous les endpoints
4. Identifier les plus lents

#### Optimisations si Lent

- Réduire la fréquence des appels (ajouter cache)
- Utiliser des requêtes parallèles au lieu de séquentielles
- Charger les gros volumes (ingrédients) une seule fois

---

### Étape 6: Exporter les Résultats de Test

#### Exporter pour Documentaion

1. Pour chaque requête testée:
   - Cliquer sur "Code" (icône en haut à droite)
   - Choisir le langage (C#, JavaScript, etc)
   - Copier le code généré pour la documentation

2. Créer un rapport:
   - Exporter la collection en JSON
   - Utiliser cet export comme référence

#### Utiliser les Résultats dans Blazor

- Les codes générés par Postman donnent un bon point de départ
- Adapter pour .NET HttpClient
- Tester chaque integration dans Blazor

---

### Points Clés à Retenir Pendant les Tests

| Aspect | Point Important |
|---|---|
| **Aucune Auth** | Pas besoin de token ou API key |
| **GET Uniquement** | Tous les endpoints sont GET, pas de POST/PUT |
| **Réponses Vides** | null signifie aucun résultat, pas une erreur |
| **Très Stable** | API rarement down, ~99.9% uptime |
| **CORS Compatible** | Peut être appelée directement depuis le navigateur |
| **JSON Uniquement** | XML, CSV etc pas supportés |

---

## Résumé des Bonnes Pratiques pour Blazor

### Architecture Recommandée

```
1. Service TheMealDB (C# HTTP Client)
   ├── Gestion des requêtes HTTP
   ├── Parsing JSON en modèles C#
   └── Gestion d'erreurs

2. Cache Manager (LocalStorage + Memory)
   ├── Cache en mémoire court terme
   ├── Cache localStorage long terme
   └── Invalidation intelligente

3. Composants Blazor
   ├── Affichage des données
   ├── Gestion de l'UI
   └── Gestion des événements utilisateur
```

### Points Critiques

1. **Ne pas appeler directement** l'API depuis les composants, utiliser un service
2. **Implémenter le caching** dès le départ pour réduire les appels
3. **Gérer les erreurs réseau** proprement
4. **Valider les paramètres** avant d'appeler l'API
5. **Logger les erreurs** pour debugging
6. **Tester dans Postman** avant de développer en Blazor
7. **Implémenter le debouncing** pour les champs de recherche en temps réel
8. **Utiliser des modèles C# typés** pour le JSON

---

**Fin du Guide**

*Version: 1.0*
*Dernière Mise à Jour: Février 2026*
*API TheMealDB: Stable et Gratuite*
