# PLAN DE REFACTORISATION - menuMalin
## Architecture Simplifiée: Authentification Classique + GUI Légère

---

## 🎯 OBJECTIF FINAL

**Frontend**: Interface utilisateur légère (affichage + logiques UI minimales)
**Backend**: Gère TOUT (authentification, logique métier, appels API externes)

---

## 📊 CHOIX: OPTION 2 (Authentification Classique)

**Pourquoi cette option?**
- ✅ Plus simple à maintenir
- ✅ Moins de dépendances externes
- ✅ Meilleure contrôle
- ✅ Pas de SRI problèmes
- ✅ UX meilleure (pas de redirects)
- ✅ Économie (pas de coûts Auth0)

---

## 🚀 PLAN D'EXÉCUTION PAR PHASES

### **PHASE 1: NETTOYAGE & SUPPRESSION (Étape 1-8)**

**Objectif**: Supprimer tout ce qui est inutilisé ou conflictuel avec Auth0/OIDC

#### Étape 1: Identifier les fichiers inutilisés
- Trouver `Pages/Authentication.razor` (utilisé pour OIDC client-side)
- Vérifier s'il y a d'autres fichiers qui importent `Microsoft.AspNetCore.Components.WebAssembly.Authentication`
- Noter tous les endroits qui font référence à Auth0 ou OIDC

#### Étape 2: Supprimer les pages inutilisées
- Supprimer complètement le fichier `Pages/Authentication.razor`
- Supprimer tout dossier `Pages/Authentication/` s'il existe

#### Étape 3: Supprimer les dépendances NuGet Frontend
- Ouvrir le fichier `.csproj` du projet frontend (menuMalin)
- Trouver et supprimer la référence à `Microsoft.AspNetCore.Components.WebAssembly.Authentication`
- Sauvegarder

#### Étape 4: Supprimer les dépendances NuGet Backend
- Ouvrir le fichier `.csproj` du projet backend (menuMalin.Server)
- Trouver et supprimer les références à:
  - `Auth0.AspNetCore.Authentication`
  - Toute autre dépendance Auth0
- Sauvegarder

#### Étape 5: Nettoyer Program.cs du Backend
- Ouvrir `menuMalin.Server/Program.cs`
- Trouver la section `AddOpenIdConnect("Auth0", ...)`
- Supprimer TOUTE cette section (peut être plusieurs lignes/blocs)
- Garder UNIQUEMENT: `AddCookie()` configuration
- Sauvegarder

#### Étape 6: Supprimer fichier de configuration Auth0
- Supprimer ou vider la section `"Auth0"` du fichier `appsettings.json` (backend)
- Garder juste les sections `"ApiConfig"` si elles existent

#### Étape 7: Supprimer le SRI workaround
- Ouvrir `menuMalin/wwwroot/index.html`
- Trouver le bloc `<script>` qui contient "Global fetch interceptor" ou "SRI"
- Supprimer TOUT ce bloc de script
- Sauvegarder

#### Étape 8: Vérifier compilation
- Ouvrir terminal
- Faire `dotnet clean`
- Faire `dotnet build` pour vérifier qu'il n'y a pas d'erreurs
- Si erreurs, chercher d'autres références à Auth0/OIDC et supprimer

---

### **PHASE 2: CRÉER INFRASTRUCTURE D'AUTHENTIFICATION (Étape 9-14)**

**Objectif**: Créer les endpoints backend pour gérer login/logout simples

#### Étape 9: Ajouter champs au modèle User
- Ouvrir `menuMalin.Server/Models/Entities/User.cs`
- Vérifier que le User a au minimum: `Id`, `Email`, `Name`
- Si pas de champs pour password: NE PAS AJOUTER TOUT DE SUITE (optionnel, voir Étape 13)
- Sauvegarder

#### Étape 10: Mettre à jour le contrôleur AuthController
- Ouvrir `menuMalin.Server/Controllers/AuthController.cs`
- Supprimer TOUS les anciens endpoints OAuth/Auth0 (Login redirect, Callback, etc.)
- Vérifier que le endpoint `/api/auth/me` existe et récupère les infos de `User.Identity`
- Adapter le endpoint pour retourner: `{ userId, email, name, isAuthenticated }`
- Ajouter un endpoint POST `/api/auth/login` (on l'implémentera à l'étape 11)
- Ajouter un endpoint POST `/api/auth/logout` (on l'implémentera à l'étape 11)
- Sauvegarder (les fonctions seront vides pour l'instant)

#### Étape 11: Implémenter logique d'authentification Backend
- Dans le AuthController:
  - **Endpoint POST /api/auth/login**:
    - Accepte email + password en JSON
    - Valide les credentials (pour maintenant, accepter juste tout avec un email simple ou faire une validation basique)
    - Si valide: Créer un `ClaimsPrincipal` avec claims (sub/userId, email, name)
    - Appeler `HttpContext.SignInAsync()` pour créer le cookie
    - Retourner: `{ isAuthenticated: true, userId, email, name }`
    - Si invalide: Retourner 401

  - **Endpoint POST /api/auth/logout**:
    - Appeler `HttpContext.SignOutAsync()` pour supprimer le cookie
    - Retourner: `{ message: "Logged out" }`

- Sauvegarder

#### Étape 12: Vérifier UserService
- Ouvrir `menuMalin.Server/Services/UserService.cs`
- Vérifier qu'il a une méthode `GetOrCreateUserAsync(userId, email, name)`
- Cette méthode doit créer l'user en base si n'existe pas
- Sauvegarder

#### Étape 13: (OPTIONNEL) Ajouter hachage de password
- SI tu veux stocker des passwords en base:
  - Ajouter champs `PasswordHash` et `PasswordSalt` au User
  - Ajouter une migration EF Core
  - Implémenter hachage de password dans AuthController (login)
- SINON: Utiliser une validation simple pour dev (pas recommandé en prod)

#### Étape 14: Ajouter middleware de validation
- Vérifier que `app.UseAuthentication()` et `app.UseAuthorization()` sont présents dans Program.cs
- Ils doivent être APRÈS `app.UseStaticFiles()` et AVANT `app.MapControllers()`
- Sauvegarder

---

### **PHASE 3: METTRE À JOUR CONTRÔLEURS PROTÉGÉS (Étape 15-17)**

**Objectif**: Adapter tous les contrôleurs qui utilisent `[Authorize]`

#### Étape 15: Identifier les contrôleurs protégés
- Trouver tous les fichiers dans `menuMalin.Server/Controllers/` qui ont `[Authorize]`
- Lister: FavoritesController, UserRecipesController, UploadController, etc.
- Noter les endpoints qui demandent authentication

#### Étape 16: Remplacer `[Authorize]` par vérification manuelle
- Pour chaque contrôleur protégé:
  - Supprimer l'attribut `[Authorize]` du contrôleur ET des endpoints
  - Au début de CHAQUE endpoint protégé, ajouter une vérification:
    - Vérifier: `if (!User.Identity?.IsAuthenticated) { return Unauthorized(); }`
    - Puis récupérer l'userId depuis `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
  - Cela permet plus de contrôle et meilleure gestion d'erreurs

#### Étape 17: Tester les contrôleurs
- Utiliser Postman ou curl
- Tester `/api/auth/login` avec email fictif
- Tester `/api/auth/me` AVANT et APRÈS login
- Vérifier les cookies sont bien envoyés/reçus

---

### **PHASE 4: REFACTORISER FRONTEND - AUTHENTIFICATION (Étape 18-24)**

**Objectif**: Remplacer le flux OAuth par un simple login form

#### Étape 18: Créer un composant LoginForm
- Créer un nouveau fichier Razor component: `Components/LoginForm.razor`
- Ce composant doit:
  - Avoir deux inputs: email et password
  - Un bouton "Se connecter"
  - Un message d'erreur si login échoue
  - NE PAS faire de redirects, juste appeler un service

#### Étape 19: Mettre à jour AuthService
- Ouvrir `Services/AuthService.cs`
- Supprimer TOUT le code qui parle d'Auth0, OIDC, redirects
- Ajouter une nouvelle méthode `LoginAsync(email, password)`:
  - Fait un POST HTTP vers `/api/auth/login` avec email + password
  - Récupère la réponse
  - Retourne les infos de l'user
  - En cas d'erreur, lancer une exception
- Ajouter une nouvelle méthode `LogoutAsync()`:
  - Fait un POST HTTP vers `/api/auth/logout`
  - Supprime l'user du state local
- Garder la méthode `IsAuthenticatedAsync()` qui appelle `/api/auth/me`
- Sauvegarder

#### Étape 20: Mettre à jour Program.cs du Frontend
- Ouvrir `menuMalin/Program.cs`
- Supprimer TOUTE configuration d'authentification OIDC
- Garder juste la configuration `HttpClient` de base
- Sauvegarder

#### Étape 21: Mettre à jour la page Index.razor ou App.razor
- Trouver où faire l'appel initial à `/api/auth/me` pour vérifier si déjà connecté
- C'est généralement dans `App.razor` ou dans un layout
- Vérifier que c'est appelé au chargement de la page

#### Étape 22: Créer une page de Login
- Créer ou modifier `Pages/Login.razor` (ou `Pages/Index.razor` si c'est la page d'accueil)
- Utiliser le composant `LoginForm.razor` créé à l'étape 18
- Ajouter navigation vers autres pages une fois connecté

#### Étape 23: Supprimer appels d'authentification partout
- Chercher dans tous les fichiers `.razor` du frontend:
  - Les mots-clés: "Auth0", "OIDC", "RemoteAuthenticator"
  - Supprimer ou commenter ces sections
  - Remplacer par appels à `AuthService.LoginAsync()`

#### Étape 24: Tester le frontend
- Arrêter l'application si elle tourne
- Faire `dotnet build`
- Vérifier qu'il n'y a pas d'erreurs
- Lancer avec `dotnet run -c Release` ou `-c Debug`

---

### **PHASE 5: CRÉER PROXY POUR RECIPES (Étape 25-28)**

**Objectif**: Que le frontend appelle le backend au lieu de TheMealDB directement

#### Étape 25: Ajouter endpoints pour Recipes dans le Backend
- Ouvrir `menuMalin.Server/Controllers/RecipesController.cs` ou en créer un
- Ajouter endpoints:
  - **GET `/api/recipes/random`**: Appelle TheMealDB pour recette aléatoire, retourne au frontend
  - **GET `/api/recipes/search?q={query}`**: Appelle TheMealDB pour chercher, retourne au frontend
  - **GET `/api/recipes/categories`**: Appelle TheMealDB pour catégories, retourne au frontend
  - Ces endpoints NE DEMANDENT PAS d'authentification (publics)
- Ces endpoints doivent utiliser le service `ITheMealDBService` pour appeler TheMealDB
- Le backend retourne les mêmes DTOs/formats qu'avant

#### Étape 26: Mettre à jour RecipeService du Frontend
- Ouvrir `Services/RecipeService.cs` (frontend)
- Trouver toutes les requêtes HTTP qui appellent directement `https://www.themealdb.com/api/...`
- Remplacer par des appels à `/api/recipes/...` (le backend proxy)
- Par exemple:
  - Au lieu de `GET https://www.themealdb.com/api/json/v1/1/random.php`
  - Appeler `GET /api/recipes/random`

#### Étape 27: Vérifier HttpApiService
- Ouvrir `Services/HttpApiService.cs` (frontend)
- Vérifier que `BrowserRequestCredentials.Include` est utilisé dans la configuration HttpClient
- Cela garantit que les cookies sont envoyés avec CHAQUE requête au backend

#### Étape 28: Tester le proxy
- Lancer l'app
- Chercher une recette
- Vérifier dans Devtools que la requête va vers `/api/recipes/search` et pas vers themealdb.com

---

### **PHASE 6: TESTER & VALIDER (Étape 29-35)**

**Objectif**: Vérifier que tout fonctionne

#### Étape 29: Test login basique
- Ouvrir `https://localhost:7057`
- La page doit montrer un formulaire de login
- Entrer un email et un password fictif
- Cliquer "Se connecter"
- Vérifier que:
  - Pas d'erreur 404 ou CORS
  - Cookie `.AspNetCore.Cookies` est créé (Devtools → Application → Cookies)
  - La page change pour montrer l'utilisateur connecté

#### Étape 30: Test logout
- Cliquer sur "Se déconnecter"
- Vérifier que:
  - Le cookie est supprimé
  - La page revient au login

#### Étape 31: Test persistence du cookie
- Login
- Recharger la page (F5)
- Vérifier que toujours connecté
- Vérifier que `/api/auth/me` retourne `isAuthenticated: true`

#### Étape 32: Test des pages protégées
- Login
- Naviguer vers Favoris, Mes Recettes, etc.
- Vérifier que tout fonctionne
- Logout et tenter d'accéder aux pages protégées
- Vérifier qu'on est redirigé au login

#### Étape 33: Test du proxy Recipes
- Login n'est PAS nécessaire pour ça
- Aller à la page de recherche
- Chercher une recette par nom
- Vérifier que:
  - Les requêtes vont vers `/api/recipes/search`
  - Les résultats s'affichent

#### Étape 34: Test sans SRI errors
- Ouvrir Devtools → Console
- Vérifier qu'il n'y a PLUS d'erreurs:
  - "SRI integrity check failed"
  - "Failed to find a valid digest in the 'integrity' attribute"
  - Si ces erreurs apparaissent, c'est qu'on n'a pas bien supprimé le workaround

#### Étape 35: Test complet en Release
- Arrêter l'app
- Faire `dotnet clean`
- Lancer `dotnet run -c Release` depuis le dossier Backend
- Refaire les tests des étapes 29-34 en Release mode

---

### **PHASE 7: CLEANUP & SÉCURITÉ (Étape 36-40)**

#### Étape 36: Nettoyer les fichiers de config
- Vérifier `appsettings.json` (backend) n'a plus de sections Auth0
- Vérifier `wwwroot/appsettings.json` (frontend) n'a plus de sections Auth0
- Garder juste: `ApiConfig` avec `BackendUrl`

#### Étape 37: Nettoyer les commentaires inutiles
- Dans `Program.cs` (backend): Supprimer les commentaires qui parlent d'Auth0, OIDC
- Garder les commentaires utiles pour la logique d'authentification cookie

#### Étape 38: Ajouter validation d'entrées
- Dans le endpoint `/api/auth/login`:
  - Vérifier que l'email est valide (format email)
  - Vérifier que le password n'est pas vide
  - Retourner erreurs explicites si invalide

#### Étape 39: (RECOMMANDÉ) Implémenter rate limiting
- Ajouter un rate limiting sur `/api/auth/login` pour éviter brute force
- Exemple: Max 5 tentatives par IP par minute
- Plusieurs options: NuGet package ou middleware custom

#### Étape 40: Documenter les endpoints
- Créer un fichier `API_ENDPOINTS.md` ou mettre à jour la doc existante
- Lister tous les endpoints:
  - `/api/auth/login` (POST) - email, password
  - `/api/auth/logout` (POST)
  - `/api/auth/me` (GET)
  - `/api/recipes/random` (GET)
  - `/api/recipes/search` (GET)
  - `/api/favorites/*` (GET, POST, DELETE) - protégés
  - `/api/user-recipes/*` (GET, POST, DELETE) - protégés

---

## 📋 TABLEAU RÉCAPITULATIF DES ÉTAPES

| Phase | Étapes | Durée estimée | Risque |
|-------|--------|---------------|----|
| Phase 1: Nettoyage | 1-8 | 30 min | ⚠️ Bas |
| Phase 2: Infrastructure Auth | 9-14 | 1-2 heures | ⚠️ Moyen |
| Phase 3: Contrôleurs | 15-17 | 30 min | ✅ Bas |
| Phase 4: Frontend Auth | 18-24 | 1-2 heures | ⚠️ Moyen |
| Phase 5: Recipe Proxy | 25-28 | 1 heure | ✅ Bas |
| Phase 6: Tests | 29-35 | 1-2 heures | ✅ Bas |
| Phase 7: Cleanup | 36-40 | 30 min | ✅ Bas |
| **TOTAL** | **40 étapes** | **6-8 heures** | |

---

## ⚠️ POINTS CRITIQUES À SURVEILLER

1. **Cookies HttpOnly + Credentials**:
   - Frontend doit envoyer `BrowserRequestCredentials.Include` PARTOUT
   - Backend doit accepter les cookies avec SameSite=Strict

2. **Pas de localStorage pour auth**:
   - Auth doit être dans le cookie HttpOnly
   - Jamais stocker le password dans localStorage

3. **Vérifier User.Identity.IsAuthenticated**:
   - CHAQUE endpoint protégé doit vérifier ça
   - Pas de confiance sur le client

4. **Migration de la base de données**:
   - Si on ajoute PasswordHash/PasswordSalt, faire une migration EF Core
   - Tester en local avant prod

5. **Tests en Release mode**:
   - Les erreurs SRI apparaissent UNIQUEMENT en Release
   - Toujours tester en Release avant de déployer

---

## 🎓 RÉSULTAT FINAL ATTENDU

✅ Authentification simple et robuste (pas de Auth0)
✅ Frontend léger (juste UI, pas de logique d'auth complexe)
✅ Backend gère TOUT (auth, logique, API externes)
✅ Pas d'erreurs SRI
✅ Pas de CORS complications
✅ Cookies sécurisés (HttpOnly, SameSite=Strict)
✅ Architecture claire et maintenable

---

**Besoin d'aide pour une étape?** Pose ta question et je te détaille!
