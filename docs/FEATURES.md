# Fonctionnalités Détaillées

Description complète des fonctionnalités de MenuMalin avec cas d'usage et implémentation.

## 1. Recherche de Recettes (API TheMealDB)

### Description
Permet à l'utilisateur de rechercher et découvrir des recettes dans une base de données publique via l'API TheMealDB.

### Cas d'Utilisation
1. **Recherche par mot-clé**: Taper "pasta" pour trouver toutes les recettes contenant ce mot
2. **Filtrer par catégorie**: Sélectionner "Dessert" pour voir uniquement les desserts
3. **Filtrer par région**: Choisir "Italian" pour explorer la cuisine italienne
4. **Voir détails**: Cliquer sur une recette pour voir la liste complète des ingrédients et instructions

### Endpoints API

**Recherche**:
```
GET /api/recipes/search?query=pasta
```

**Par catégorie**:
```
GET /api/recipes/filter?type=category&value=Dessert
```

**Par région**:
```
GET /api/recipes/filter?type=area&value=Italian
```

**Détails recette**:
```
GET /api/recipes/{mealDBId}
```

### Implémentation Technique
- **Source**: TheMealDB API publique (https://themealdb.com/api.php)
- **Caching**: En-mémoire sur 5 minutes (Polly + caching policy)
- **Retry Logic**: 3 tentatives avec backoff exponentiel (2s, 4s)
- **Timeout**: 15 secondes par requête
- **Gestion d'erreurs**:
  - 401/403: Clé API invalide
  - 404: Recette non trouvée
  - 500+: Erreur serveur avec retry automatique
  - Timeout réseau: ErreurReseauException

### DTOs

```csharp
public class RecetteMealDTO
{
    public string? IdMeal { get; set; }
    public string? StrMeal { get; set; }
    public string? StrMealThumb { get; set; }
    public string? StrCategory { get; set; }
    public string? StrArea { get; set; }
    public string? StrInstructions { get; set; }
    public List<string> Ingredients { get; set; } = new();
    public List<string> Measures { get; set; } = new();
    public string? StrYoutube { get; set; }
}
```

## 2. Gestion des Favoris

### Description
Permet aux utilisateurs authentifiés de sauvegarder et gérer une liste personnalisée de recettes favorites.

### Cas d'Utilisation
1. **Ajouter aux favoris**: Cliquer ❤️ sur une recette de l'API pour la sauvegarder
2. **Voir mes favoris**: Accéder à `/mes-favoris` pour voir toutes les recettes sauvegardées
3. **Supprimer des favoris**: Cliquer ❤️ pour retirer une recette de la liste
4. **Persistance**: Les favoris restent même après déconnexion

### Endpoints API

**Lister les favoris**:
```
GET /api/favorites
Authorization: Cookie
```

**Ajouter**:
```
POST /api/favorites/{mealDBId}
Authorization: Cookie
```

**Supprimer**:
```
DELETE /api/favorites/{mealDBId}
Authorization: Cookie
```

**Vérifier si favori**:
```
GET /api/favorites/{mealDBId}/check
Authorization: Cookie (optionnel)
```

### Structure Base de Données

```sql
CREATE TABLE Favoris (
    Id VARCHAR(36) PRIMARY KEY,
    UtilisateurId VARCHAR(36) NOT NULL,
    MealDBId VARCHAR(36) NOT NULL,
    RecetteId VARCHAR(36),  -- Si recette locale
    DateAjout DATETIME NOT NULL,
    FOREIGN KEY (UtilisateurId) REFERENCES Utilisateurs(Id),
    UNIQUE KEY UQ_Favori (UtilisateurId, MealDBId)
);
```

### Implémentation Technique
- **Authentification requise**: Exception levée si userId null
- **Isolation par utilisateur**: Requête filtrée par UtilisateurId
- **Transactions**: AddAsync/DeleteAsync dans une transaction
- **Soft delete**: Non utilisé (suppression logique)
- **Caching**: Invalidé à chaque modification

## 3. Création de Recettes Personnalisées

### Description
Permet aux utilisateurs de créer et modifier leurs propres recettes avec image, ingrédients et instructions.

### Cas d'Utilisation
1. **Créer une recette**: Accéder à `/creer-recette` et remplir le formulaire
2. **Télécharger une image**: Uploader une image qui sera sauvegardée
3. **Ajouter ingrédients**: Ajouter chaque ingrédient avec sa quantité
4. **Modifier**: Éditer la recette après création
5. **Supprimer**: Supprimer une recette personnelle

### Endpoints API

**Créer**:
```
POST /api/user-recipes
Content-Type: multipart/form-data
Authorization: Cookie

{
  "titre": "Ma Pasta",
  "description": "Pâtes maison",
  "image": <file>,
  "ingredients": ["Farine", "Eau"],
  "mesures": ["500g", "250ml"],
  "instructions": "Mélanger et cuire"
}
```

**Lister mes recettes**:
```
GET /api/user-recipes
Authorization: Cookie
```

**Détail**:
```
GET /api/user-recipes/{id}
Authorization: Cookie
```

**Modifier**:
```
PATCH /api/user-recipes/{id}
Authorization: Cookie
```

**Supprimer**:
```
DELETE /api/user-recipes/{id}
Authorization: Cookie
```

### Structure Entité

```csharp
public class RecetteUtilisateur
{
    public string Id { get; set; }
    public string UtilisateurId { get; set; }
    public string Titre { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Instructions { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public bool EstPublique { get; set; } = false;
}
```

### Upload d'Image
- **Stockage**: Disque local `/wwwroot/uploads/recipes/`
- **Format**: JPG, PNG uniquement
- **Taille max**: 5 MB
- **Nom fichier**: `{userId}_{timestamp}_{originalName}`
- **Endpoint**: `ControleurTeleversement.UploadAsync()`

## 4. Partage Public/Privé de Recettes

### Description
Les utilisateurs peuvent partager leurs recettes personnalisées publiquement ou les garder privées.

### Cas d'Utilisation
1. **Créer recette privée**: Par défaut (EstPublique = false)
2. **Publier recette**: Modifier et cocher "Partager publiquement"
3. **Découvrir**: Consulter `/recettes-publiques` pour voir les recettes partagées
4. **Profil utilisateur**: Voir toutes les recettes publiques d'un utilisateur

### Endpoints API

**Recettes publiques**:
```
GET /api/recipes/public
```

**Par auteur**:
```
GET /api/recipes/public?author={userId}
```

**Vérification d'accès**:
```csharp
// Dans ControleurRecettesUtilisateur.GetRecipe()
if (!recipe.EstPublique && recipe.UtilisateurId != currentUserId)
{
    return Forbid(); // 403 Forbidden
}
```

### Règles d'Accès

| Cas | Propriétaire | Autre utilisateur |
|-----|-------------|------------------|
| Recette privée | ✅ Accès | ❌ Forbidden (403) |
| Recette publique | ✅ Accès | ✅ Lecture seule |
| Modifier | ✅ Oui | ❌ Non |
| Supprimer | ✅ Oui | ❌ Non |

## 5. Authentification Sécurisée

### Description
Système d'authentification robuste avec hashage de mots de passe et gestion de sessions sécurisées.

### Cas d'Utilisation
1. **Inscription**: Créer un compte avec email et password
2. **Connexion**: Se connecter pour accéder à ses données personnelles
3. **Déconnexion**: Terminer la session et nettoyer les données locales
4. **Session persistante**: Rester connecté sur plusieurs onglets

### Endpoints API

**Inscription**:
```
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "nom": "John Doe"
}
```

**Connexion**:
```
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}

Response:
{
  "id": "user-id-uuid",
  "nom": "John Doe",
  "email": "user@example.com"
}
```

**Vérifier session**:
```
GET /api/auth/current-user
Authorization: Cookie
```

**Déconnexion**:
```
POST /api/auth/logout
Authorization: Cookie
```

### Sécurité Implémentée

1. **Hashage Password**: BCrypt avec salt aléatoire
   ```csharp
   var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
   var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
   ```

2. **Cookies HttpOnly**: Inaccessible depuis JavaScript
   ```csharp
   options.Cookie.HttpOnly = true; // Protège contre XSS
   options.Cookie.SameSite = SameSiteMode.Lax; // Protège contre CSRF
   options.ExpireTimeSpan = TimeSpan.FromHours(24);
   options.SlidingExpiration = true; // Expire après 24h d'inactivité
   ```

3. **Sliding Expiration**: La session se prolonge à chaque activité
   - Utilisateur actif reste connecté indéfiniment
   - Utilisateur inactif > 24h est déconnecté
   - Protège les comptes abandonnés

4. **Validation Email**: Lors de l'inscription
   - Email unique en base de données
   - Format validé
   - SMTP optionnel pour envoi de confirmation

### Flow Authentification

```
1. User complète formulaire login
   ↓
2. Frontend POST /api/auth/login
   ↓
3. Backend valide credentials
   ↓
4. BCrypt.Verify(password, hashedPassword)
   ↓
5. SignInAsync() → Crée ticket d'authentification
   ↓
6. Cookie envoyé au client
   ↓
7. Frontend stocke userId dans localStorage
   ↓
8. Requêtes suivantes avec Cookie automatique
```

## 6. Gestion de Profil Utilisateur

### Description
Permet aux utilisateurs de gérer leurs informations personnelles.

### Cas d'Utilisation
1. **Voir profil**: Accéder à `/mon-profil`
2. **Modifier nom**: Changer le nom d'affichage
3. **Changer mot de passe**: Mettre à jour le password
4. **Consulter infos**: Email, date d'inscription

### Endpoints API

**Récupérer profil**:
```
GET /api/users/profile
Authorization: Cookie
```

**Modifier nom**:
```
PATCH /api/users/profile/name
{
  "nom": "Nouveau Nom"
}
Authorization: Cookie
```

**Changer mot de passe**:
```
PATCH /api/users/profile/password
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword456!"
}
Authorization: Cookie
```

### Structure

```csharp
public class Utilisateur
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Nom { get; set; }
    public string PasswordHash { get; set; }
    public DateTime DateInscription { get; set; }
    public DateTime? DateDerniereConnexion { get; set; }
}
```

## 7. Gestion d'Erreurs Globale

### Description
Système centralisé de gestion d'erreurs avec messages localisés et codes d'erreur standardisés.

### Types d'Erreurs

**ErreurApiException** (HTTP 4xx/5xx):
```csharp
if (!response.IsSuccessStatusCode)
{
    throw new ErreurApiException(
        (int)response.StatusCode,
        await response.Content.ReadAsStringAsync()
    );
}
```

Codes gérés:
- 401 Unauthorized → "Vous devez vous connecter"
- 403 Forbidden → "Vous n'avez pas accès à cette ressource"
- 404 Not Found → "Recette non trouvée"
- 500 Server Error → "Erreur serveur, réessayez plus tard"

**ErreurReseauException** (Network):
```csharp
catch (HttpRequestException ex)
{
    throw new ErreurReseauException(
        "Erreur de connexion réseau",
        ex
    );
}
```

### Middleware Global

Dans `Program.cs`:
```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerPathFeature>()?.Error;

        logger.LogError(exception, "Erreur non gérée");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            message = "Une erreur serveur s'est produite",
            traceId = context.TraceIdentifier
        });
    });
});
```

## 8. Notifications Utilisateur

### Description
Système de notifications non-intrusif pour l'utilisateur sur le succès/erreur des actions.

### Types de Notifications
- ✅ **Succès** (vert): "Recette ajoutée aux favoris"
- ❌ **Erreur** (rouge): "Impossible de charger vos favoris"
- ⚠️ **Avertissement** (orange): "Session va expirer"
- ℹ️ **Info** (bleu): "Action en cours..."

### Implémentation

Service frontend `ServiceNotification.cs`:
```csharp
public class ServiceNotification : IServiceNotification
{
    private readonly IToastService _toastService;

    public async Task AfficherSucces(string message, string titre = "Succès")
    {
        await _toastService.ShowSuccess(message, titre);
    }

    public async Task AfficherErreur(string message, string titre = "Erreur")
    {
        await _toastService.ShowError(message, titre);
    }
}
```

Utilisé dans les composants:
```csharp
try
{
    await _serviceFavoris.AddFavoriAsync(recipeId);
    await _serviceNotification.AfficherSucces("Recette ajoutée aux favoris");
}
catch (ErreurApiException ex) when (ex.StatusCode == 401)
{
    await _serviceNotification.AfficherErreur("Vous devez vous connecter");
}
```

## 9. Service Contact

### Description
Permet aux utilisateurs de contacter l'équipe avec des messages et s'inscrire à la newsletter.

### Cas d'Utilisation
1. **Contacter**: Remplir le formulaire de contact (optionnel: email)
2. **Newsletter**: Cocher pour recevoir les actualités
3. **Feedback**: Envoyer une suggestion ou signaler un bug

### Endpoint API

**Envoyer message**:
```
POST /api/contact
{
  "email": "user@example.com",
  "name": "John Doe",
  "subject": "Suggestion",
  "message": "Il faudrait ajouter...",
  "subscribeNewsletter": true
}
```

## Résumé Fonctionnalités

| Fonctionnalité | Authentification | Persistance | Source |
|---|---|---|---|
| Recherche API | Non | Non | TheMealDB |
| Favoris | Oui | MySQL | Base locale |
| Recettes perso | Oui | MySQL + Disque | Utilisateur |
| Partage public | Non* | MySQL | Utilisateur |
| Profil | Oui | MySQL | Utilisateur |
| Contact | Non | Optionnel | SMTP |

*Non requis pour lire les recettes publiques, oui pour créer/modifier

