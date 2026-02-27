je # 📊 PROGRESSION DE REFACTORISATION - menuMalin

**Date de début**: 2026-02-27
**Objectif**: Migration vers Login Simple + Architecture Légère Frontend

---

## 📈 RÉSUMÉ GÉNÉRAL

**Étapes complétées**: 8/40 (20%)
**Phase actuelle**: ✅ PHASE 1 TERMINÉE! → Commençons Phase 2
**Durée estimée restante**: ~3.5 heures

---

## ✅ PHASE 1: NETTOYAGE & SUPPRESSION (Étapes 1-8)

**État**: ✅ TERMINÉE!
**Durée réelle**: ~45 min
**Risque**: ✅ Aucun problème

- [x] **Étape 1**: Identifier les fichiers inutilisés (Authentication.razor, imports Auth0)
- [x] **Étape 2**: Supprimer `Pages/Authentication.razor`
- [x] **Étape 3**: Supprimer dépendance NuGet frontend `Microsoft.AspNetCore.Components.WebAssembly.Authentication`
- [x] **Étape 4**: Supprimer dépendance NuGet backend `Auth0.AspNetCore.Authentication`
- [x] **Étape 5**: Nettoyer `Program.cs` backend (supprimer `AddOpenIdConnect`)
- [x] **Étape 6**: Supprimer section `"Auth0"` de `appsettings.json`
- [x] **Étape 7**: Supprimer SRI workaround de `wwwroot/index.html`
- [x] **Étape 8**: Vérifier compilation (`dotnet clean` + `dotnet build`)

**Notes**:
```
✅ COMPLÉTÉES:
- Étape 1: Identifié les fichiers inutilisés (Authentication.razor, AuthenticationDelegatingHandler.cs)
- Étape 2: Supprimé Pages/Authentication.razor et Services/AuthenticationDelegatingHandler.cs
- Étape 3: Supprimé NuGet package WebAssembly.Authentication de menuMalin.csproj (ligne 20)

- Étape 4: Supprimé NuGet Auth0.AspNetCore.Authentication + Auth0.ManagementApi + OpenIdConnect du backend
- Étape 5: Supprimé bloc AddOpenIdConnect complet de Program.cs (120+ lignes)
  - Changé DefaultChallengeScheme de "Auth0" à "Cookies"
  - Supprimé import "using Microsoft.AspNetCore.Authentication.OpenIdConnect;"

- Étape 6: Supprimé sections "Auth0" de TOUS les appsettings:
  - menuMalin.Server/appsettings.json
  - menuMalin.Server/appsettings.Development.json (+ logs OpenIdConnect)
  - menuMalin.Server/appsettings.Production.json
  - menuMalin/wwwroot/appsettings.json (gardé ApiConfig)
- Étape 7: Supprimé SRI workaround de index.html + script AuthenticationService.js
- Étape 8: Compilation réussie!
  - Fixé imports dans _Imports.razor (supprimé Components.Authorization)
  - Compilation: 0 Erreurs, 42 Avertissements ✅

🎉 PHASE 1 COMPLÉTÉE! Tous les Auth0/OIDC/SRI workaround supprimés!
```

---

## 🔐 PHASE 2: INFRASTRUCTURE D'AUTHENTIFICATION (Étapes 9-14)

**État**: ⏳ Prêt à commencer
**Durée estimée**: 1-2 heures
**Risque**: ⚠️ Moyen

- [ ] **Étape 9**: Vérifier modèle `User.cs` (Id, Email, Name existants)
- [ ] **Étape 10**: Mettre à jour `AuthController.cs` (ajouter endpoint POST `/api/auth/login` vide)
- [ ] **Étape 11**: Implémenter logique d'authentification (login/logout dans AuthController)
- [ ] **Étape 12**: Vérifier `UserService.GetOrCreateUserAsync()` existe
- [ ] **Étape 13**: (OPTIONNEL) Ajouter hachage password (PasswordHash, PasswordSalt)
- [ ] **Étape 14**: Vérifier middleware auth dans `Program.cs` (`UseAuthentication`, `UseAuthorization`)

**Détails Étape 11**:
- [ ] Endpoint POST `/api/auth/login` (email, password)
- [ ] Endpoint POST `/api/auth/logout`
- [ ] Endpoint GET `/api/auth/me` fonctionnel

**Notes**:
```
- AuthController existe déjà
- À implémenter: Validation credentials simple pour dev
- Cookies HttpOnly + SameSite=Strict déjà configurés
```

---

## 🛡️ PHASE 3: MISE À JOUR CONTRÔLEURS PROTÉGÉS (Étapes 15-17)

**État**: ⏳ Non commencé
**Durée estimée**: 30 min
**Risque**: ✅ Bas

- [ ] **Étape 15**: Identifier contrôleurs avec `[Authorize]` (Favorites, UserRecipes, Upload, etc.)
- [ ] **Étape 16**: Remplacer `[Authorize]` par vérification manuelle `if (!User.Identity?.IsAuthenticated)`
- [ ] **Étape 17**: Tester endpoints avec Postman (login, accès aux endpoints protégés)

**Contrôleurs à vérifier**:
- [ ] FavoritesController
- [ ] UserRecipesController
- [ ] UploadController
- [ ] Autres?

**Notes**:
```
- Chaque endpoint protégé doit vérifier User.Identity?.IsAuthenticated
```

---

## 🎨 PHASE 4: REFACTORISER FRONTEND - AUTHENTIFICATION (Étapes 18-24)

**État**: ⏳ Non commencé
**Durée estimée**: 1-2 heures
**Risque**: ⚠️ Moyen

- [ ] **Étape 18**: Créer composant `Components/LoginForm.razor` (email + password inputs)
- [ ] **Étape 19**: Mettre à jour `Services/AuthService.cs` (LoginAsync, LogoutAsync)
- [ ] **Étape 20**: Nettoyer `Program.cs` frontend (supprimer config OIDC)
- [ ] **Étape 21**: Vérifier appel `/api/auth/me` au démarrage (App.razor ou Layout)
- [ ] **Étape 22**: Créer/modifier page de Login (`Pages/Login.razor` ou `Index.razor`)
- [ ] **Étape 23**: Supprimer appels Auth0/OIDC partout dans frontend
- [ ] **Étape 24**: Vérifier compilation frontend (`dotnet build`)

**Composants/Services à mettre à jour**:
- [ ] Components/LoginForm.razor (NEW)
- [ ] Services/AuthService.cs
- [ ] Program.cs (frontend)
- [ ] Pages/Index.razor ou Pages/Login.razor
- [ ] App.razor ou MainLayout.razor

**Notes**:
```
- LoginForm: email + password + bouton + gestion erreurs
- AuthService: supprimer tout code OAuth, ajouter LoginAsync(email, password)
```

---

## 🍽️ PHASE 5: CRÉER PROXY POUR RECIPES (Étapes 25-28)

**État**: ⏳ Non commencé
**Durée estimée**: 1 heure
**Risque**: ✅ Bas

- [ ] **Étape 25**: Ajouter endpoints backend pour Recipes (`/api/recipes/random`, `/api/recipes/search`, etc.)
- [ ] **Étape 26**: Mettre à jour `RecipeService.cs` frontend (appeler `/api/recipes/...` au lieu de themealdb.com)
- [ ] **Étape 27**: Vérifier `HttpApiService.cs` utilise `BrowserRequestCredentials.Include`
- [ ] **Étape 28**: Tester proxy recipes (vérifier requêtes vont vers backend, pas themealdb)

**Endpoints à créer**:
- [ ] GET `/api/recipes/random`
- [ ] GET `/api/recipes/search?q={query}`
- [ ] GET `/api/recipes/categories`

**Notes**:
```
- TheMealDBService backend existe déjà ✅
- Frontend appelle directement themealdb.com → remplacer par backend proxy
```

---

## ✅ PHASE 6: TESTS & VALIDATION (Étapes 29-35)

**État**: ⏳ Non commencé
**Durée estimée**: 1-2 heures
**Risque**: ✅ Bas

- [ ] **Étape 29**: Test login basique (formulaire → cookie créé)
- [ ] **Étape 30**: Test logout (cookie supprimé)
- [ ] **Étape 31**: Test persistence cookie (reload page → toujours connecté)
- [ ] **Étape 32**: Test pages protégées (après login → accès, après logout → redirect login)
- [ ] **Étape 33**: Test proxy recipes (recherche fonctionne → appel `/api/recipes/search`)
- [ ] **Étape 34**: Vérifier PLUS d'erreurs SRI en console (DevTools → Console)
- [ ] **Étape 35**: Test complet en Release (`dotnet run -c Release`)

**Checklist Test**:
- [ ] Pas d'erreur SRI "integrity check failed"
- [ ] Pas d'erreur CORS
- [ ] Cookie `.AspNetCore.Cookies` présent
- [ ] Favoris sauvegardent/chargent correctement
- [ ] Recherche de recettes fonctionne

**Notes**:
```
- Tester OBLIGATOIREMENT en Release pour vérifier pas de SRI issues
- Ouvrir DevTools (F12) pour vérifier logs/cookies
```

---

## 🧹 PHASE 7: CLEANUP & SÉCURITÉ (Étapes 36-40)

**État**: ⏳ Non commencé
**Durée estimée**: 30 min
**Risque**: ✅ Bas

- [ ] **Étape 36**: Nettoyer fichiers config (supprimer sections Auth0)
- [ ] **Étape 37**: Nettoyer commentaires inutiles (supprimer mentions Auth0/OIDC)
- [ ] **Étape 38**: Ajouter validation d'entrées (email format, password non-vide)
- [ ] **Étape 39**: (RECOMMANDÉ) Implémenter rate limiting sur `/api/auth/login`
- [ ] **Étape 40**: Documenter endpoints API (`API_ENDPOINTS.md`)

**Notes**:
```
- Fichiers config: appsettings.json (frontend + backend)
- Rate limiting: Max 5 tentatives par IP par minute (optionnel mais recommandé)
```

---

## 📝 NOTES DE PROGRESSION

### Blocages identifiés:
```
(À remplir au fur et à mesure)
```

### Dépendances trouvées:
```
(À remplir au fur et à mesure)
```

### Fichiers importants:
```
Backend:
- menuMalin.Server/Program.cs
- menuMalin.Server/Controllers/AuthController.cs
- menuMalin.Server/Services/UserService.cs

Frontend:
- menuMalin/Program.cs
- menuMalin/Services/AuthService.cs
- menuMalin/wwwroot/appsettings.json
- menuMalin/wwwroot/index.html

Tests:
- Postman ou curl pour tester endpoints
- DevTools (F12) pour vérifier cookies/logs
```

---

## 🎯 RÉSUMÉ DES FICHIERS À MODIFIER

### À SUPPRIMER:
- [ ] `menuMalin/Pages/Authentication.razor`
- [ ] Section `"Auth0"` de `appsettings.json`
- [ ] SRI workaround de `wwwroot/index.html`

### À CRÉER:
- [ ] `menuMalin/Components/LoginForm.razor`
- [ ] `API_ENDPOINTS.md` (documentation)

### À MODIFIER:
- [ ] `menuMalin.Server/Program.cs` (nettoyer OpenIdConnect)
- [ ] `menuMalin.Server/Controllers/AuthController.cs` (ajouter login/logout)
- [ ] `menuMalin.Server/Services/UserService.cs` (vérifier)
- [ ] `menuMalin/Program.cs` (nettoyer OIDC)
- [ ] `menuMalin/Services/AuthService.cs` (remplacer OAuth par login simple)
- [ ] `menuMalin/Services/RecipeService.cs` (appeler backend proxy)
- [ ] `menuMalin/wwwroot/index.html` (supprimer SRI workaround)
- [ ] Contrôleurs protégés (remplacer `[Authorize]`)

---

## 🚀 COMMANDES GIT À UTILISER

```bash
# Au début de chaque phase
git checkout -b feat/refactor-auth-phase-X

# Quand phase terminée et testée
git add .
git commit -m "feat: Phase X - [description]"
git push origin feat/refactor-auth-phase-X

# Puis créer PR pour review
```

---

## 📅 TIMELINE ESTIMÉE

| Phase | Étapes | Temps | Date estimée |
|-------|--------|-------|--------------|
| Phase 1 | 1-8 | 30 min | 2026-02-27 (soir) |
| Phase 2 | 9-14 | 1-2h | 2026-02-28 (matin) |
| Phase 3 | 15-17 | 30 min | 2026-02-28 (midi) |
| Phase 4 | 18-24 | 1-2h | 2026-02-28 (après-midi) |
| Phase 5 | 25-28 | 1h | 2026-03-01 (matin) |
| Phase 6 | 29-35 | 1-2h | 2026-03-01 (midi/après-midi) |
| Phase 7 | 36-40 | 30 min | 2026-03-01 (soir) |
| **TOTAL** | **40** | **6-8h** | **~3 jours** |

---

**Mis à jour**: 2026-02-27 13:15
**Statut global**: 🔴 Non commencé
