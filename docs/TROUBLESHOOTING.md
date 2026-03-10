# Dépannage

Solutions aux problèmes courants rencontrés lors du développement et du déploiement de MenuMalin.

## Problèmes de Démarrage

### dotnet run est infini / Génération...

**Symptôme**: Le serveur semble gelé après `dotnet run`.

**Causes possibles**:
1. MySQL n'est pas accessible
2. Migrations bloquent indéfiniment
3. Port déjà utilisé

**Solutions**:

**Option 1 - Vérifier MySQL**:
```bash
# Windows
services.msc  # Vérifier que MySQL est démarré

# Linux/Mac
sudo systemctl status mysql
sudo systemctl start mysql

# Tester la connexion
mysql -u root -p
```

**Option 2 - Exécuter les migrations manuellement**:
```bash
cd menuMalin.Server
dotnet ef database update --no-build

# Ou si la DB n'existe pas:
dotnet ef database update --connection "Server=localhost;Port=3306;Database=menuMalin;User Id=root;Password=root;"
```

**Option 3 - Vérifier le port**:
```bash
# Windows
netstat -ano | findstr 7057

# Linux/Mac
lsof -i :7057

# Si utilisé, tuer le processus (Windows)
taskkill /PID <pid> /F

# Ou changer le port dans launchSettings.json
```

---

### Connection string not found

**Symptôme**:
```
InvalidOperationException: Connection string 'DefaultConnection' not found.
```

**Cause**: `appsettings.Development.json` manque la clé `ConnectionStrings.DefaultConnection`.

**Solution**:
Vérifier que `menuMalin.Server/appsettings.Development.json` contient:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=menuMalin;User Id=root;Password=root;"
  }
}
```

---

### MySQL pas accessible au démarrage

**Symptôme**:
```
⚠️  MySQL pas accessible - migrations ignorées
   Exécutez: dotnet ef database update
```

**Cause**: MySQL n'est pas disponible au moment du démarrage.

**Solution**:
```bash
# S'assurer que MySQL est lancé
sudo systemctl start mysql  # Linux
# ou services.msc (Windows)

# Puis exécuter les migrations
cd menuMalin.Server
dotnet ef database update
```

---

## Problèmes Frontend

### "Impossible de charger vos favoris" (401)

**Symptôme**: Page `/mes-favoris` affiche l'erreur 401 Unauthorized.

**Causes possibles**:
1. Cookie d'authentification expiré
2. Cookie non envoyé au serveur
3. Backend ne reçoit pas le userId

**Solutions**:

**Option 1 - Vérifier la configuration CORS**:
Dans `Program.cs`, vérifier que `AllowCredentials()` est activé:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7777", "https://localhost:7057")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // ✅ IMPORTANT
    });
});
```

**Option 2 - Vérifier Cookie.SameSite**:
```csharp
options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax; // Pour cross-port
```

**Option 3 - Vérifier localStorage desynchronisé**:
Dans `MesFavoris.razor`, la page appelle `CheckAuthentication()` pour synchroniser avec le serveur:
```csharp
protected override async Task OnParametersSetAsync()
{
    // Vérifier si authentifié avec le serveur
    var user = await _serviceAuthentification.GetCurrentUserAsync();
    if (user == null)
    {
        // Synchroniser: nettoyer le localStorage
        await _serviceAuthentification.ClearAuthStateAsync();
        NavigationManager.NavigateTo("/connexion");
    }
}
```

---

### Les favoris ne se chargent pas

**Symptôme**: Page blanche ou liste vide même après avoir ajouté des favoris.

**Causes possibles**:
1. Requête JSON mal désérialisée
2. DTO ne correspond pas à la réponse API
3. Service HTTP n'envoie pas les cookies

**Solutions**:

**Option 1 - Vérifier la désérialisation**:
Dans `ServiceApiHttp.cs`, s'assurer que les exceptions JSON sont capturées:
```csharp
catch (System.Text.Json.JsonException ex)
{
    throw new ErreurApiException(
        response.StatusCode,
        $"JSON deserialization error: {ex.Message}"
    );
}
```

**Option 2 - Vérifier le DTO**:
`RecetteDTO` doit avoir les mêmes propriétés que la réponse JSON (case-insensitive).

**Option 3 - Vérifier la requête HTTP**:
```csharp
// Vérifier que credentials sont inclus
using var httpRequestMessage = new HttpRequestMessage(
    HttpMethod.Get,
    new Uri(RequestUri)
);
httpRequestMessage.SetBrowserRequestCredentials(
    BrowserRequestCredentials.Include // ✅ Envoie les cookies
);
```

---

### "Sorry, there's nothing at this address" (404)

**Symptôme**: Page d'erreur 404 en français.

**Cause**: Route inexistante.

**Solutions**:
1. Vérifier l'URL dans `Components/RouteLister.razor`
2. Vérifier que `@page` directive est correcte dans la composante

Exemple:
```csharp
@page "/mes-favoris"  // ✅ Doit correspondre à l'URL navigateur
```

---

### Navigation brisée entre pages

**Symptôme**: Cliquer sur un lien mène à une URL mais le contenu ne change pas.

**Cause**: SPA routing - le frontend doit charger les données quand les paramètres changent.

**Solution**: Utiliser `OnParametersSetAsync()` plutôt que `OnInitializedAsync()`:
```csharp
public override async Task SetParametersAsync(ParameterView parameters)
{
    // Appelé quand les @parameters changent
    await base.SetParametersAsync(parameters);
}

protected override async Task OnParametersSetAsync()
{
    // Recharger les données ici
    await LoadFavorites();
}
```

---

### Image n'affiche pas

**Symptôme**: Image cassée (404) ou ne charge pas.

**Causes possibles**:
1. Image stockée au mauvais endroit
2. Chemin incorrect dans la DB
3. Permissions d'accès

**Solutions**:

**Option 1 - Vérifier le dossier**:
```bash
# Linux
ls -la menuMalin.Server/wwwroot/uploads/recipes/

# Windows
dir "C:\...\menuMalin.Server\wwwroot\uploads\recipes\"
```

**Option 2 - Vérifier le chemin stocké**:
La DB doit contenir:
```
/uploads/recipes/550e8400-e29b-41d4-a716-446655440000_1710000000_image.jpg
```

Et utiliser dans le HTML:
```html
<img src="@recipe.ImageUrl" />
```

**Option 3 - Ajouter le middleware StaticFiles**:
Dans `Program.cs`:
```csharp
app.UseStaticFiles(); // Doit être avant UseRouting()
```

---

## Problèmes Backend

### 401 Unauthorized sur API

**Symptôme**: Requête à `/api/favorites` retourne 401.

**Causes possibles**:
1. Cookie absent ou expiré
2. Endpoint ne reçoit pas l'authentification
3. Middleware d'authentification mal configuré

**Solutions**:

**Option 1 - Vérifier l'endpoint a [Authorize]**:
```csharp
[Authorize]
[HttpGet]
public async Task<IActionResult> GetFavorites()
{
    // ...
}
```

**Option 2 - Vérifier l'ordre des middlewares**:
```csharp
app.UseAuthentication();  // Avant UseAuthorization()
app.UseAuthorization();
app.MapControllers();
```

**Option 3 - Vérifier le cookie dans les en-têtes**:
```bash
curl -v https://localhost:7057/api/favorites --insecure
# Chercher "Cookie:" dans la réponse
```

---

### 403 Forbidden sur recette privée

**Symptôme**: Accès à `/api/user-recipes/{id}` d'une recette privée d'un autre utilisateur retourne 403.

**C'est normal** - les recettes privées ne sont accessibles qu'à leur propriétaire.

**Vérifier l'implémentation**:
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetRecipe(string id)
{
    var recipe = await _depot.GetByIdAsync(id);

    // Vérifier: privée OU propriétaire
    if (!recipe.EstPublique && recipe.UtilisateurId != GetCurrentUserId())
    {
        return Forbid(); // 403
    }

    return Ok(recipe);
}
```

---

### "Uncaught error about No Listener"

**Symptôme**: Console affiche une erreur non-gérée.

**Cause**: Une exception est levée mais non capturée quelque part.

**Solution**: Ajouter un `ErrorBoundary` dans `App.razor`:
```csharp
<CascadingValue Value=this>
    <ErrorBoundary>
        <Router AppAssembly="typeof(Program).Assembly"
                OnNavigateAsync="OnNavigateAsync">
            <!-- routes -->
        </Router>
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </ErrorBoundary>
</CascadingValue>
```

---

### Timeout sur requête API

**Symptôme**:
```
TaskCanceledException: A task was canceled.
```

**Cause**: Requête prend > 15 secondes (ou timeout défini).

**Solutions**:

**Option 1 - Augmenter le timeout**:
```csharp
builder.Services.AddHttpClient<IServiceMealDB, ServiceMealDB>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30); // Augmenter de 15s à 30s
    });
```

**Option 2 - Vérifier la requête lente**:
```bash
# Tester directement
curl -v https://api.themealdb.com/api/json/v1/1/search.php?s=pasta --max-time 20

# Si lent, le problème vient de TheMealDB
```

**Option 3 - Utiliser Polly Retry**:
```csharp
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt =>
            TimeSpan.FromSeconds(Math.Pow(2, attempt))
    );

builder.Services.AddHttpClient<IServiceMealDB, ServiceMealDB>()
    .AddPolicyHandler(retryPolicy);
```

---

## Problèmes Base de Données

### Foreign key constraint failed

**Symptôme**:
```
MySqlException: Cannot add or update a child row: a foreign key constraint fails
```

**Cause**: Essayer d'ajouter un enregistrement enfant sans parent.

**Exemple**:
```csharp
// ❌ Mauvais: créer un favori sans utilisateur
var favori = new Favori { MealDBId = "123", UtilisateurId = null };
```

**Solution**:
```csharp
// ✅ Bon: vérifier que l'utilisateur existe
var userId = GetCurrentUserId();
if (string.IsNullOrEmpty(userId))
    throw new InvalidOperationException("User not found");

var favori = new Favori { MealDBId = "123", UtilisateurId = userId };
```

---

### Duplicate entry for unique key

**Symptôme**:
```
MySqlException: Duplicate entry
```

**Cause**: Violation d'une contrainte UNIQUE.

**Exemple**: Ajouter un favori deux fois
```csharp
// DB a: UNIQUE KEY UQ_Favori (UtilisateurId, MealDBId)
// ❌ Cela lève une exception
```

**Solution**:
```csharp
// Vérifier avant d'ajouter
var existing = await _depot.GetAsync(userId, mealDBId);
if (existing != null)
    return BadRequest("Already in favorites");

// Ou capturer l'exception
try
{
    await _depot.AddAsync(favori);
}
catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("Duplicate"))
{
    return StatusCode(409, "Already in favorites");
}
```

---

### Migrations non appliquées

**Symptôme**: Les tables n'existent pas en base.

**Solution**:
```bash
# Vérifier les migrations en attente
dotnet ef migrations list

# Appliquer les migrations
cd menuMalin.Server
dotnet ef database update

# Ou spécifier une migration
dotnet ef database update CreateTablesInitial
```

---

## Problèmes de Performance

### API lente

**Symptôme**: Requêtes prennent > 1 seconde.

**Causes possibles**:
1. Requête N+1 (boucles de requêtes)
2. Pas d'index sur la colonne filtrée
3. TheMealDB API est lente

**Solutions**:

**Option 1 - Utiliser `.Include()` pour éviter N+1**:
```csharp
// ❌ Mauvais: boucle de requêtes
var favoris = await _context.Favoris.ToListAsync();
foreach (var f in favoris)
{
    var user = await _context.Utilisateurs.FindAsync(f.UtilisateurId); // N requêtes
}

// ✅ Bon: une seule requête avec JOIN
var favoris = await _context.Favoris
    .Include(f => f.Utilisateur)
    .ToListAsync();
```

**Option 2 - Ajouter des indexes**:
```csharp
// Dans la migration
modelBuilder.Entity<Favori>()
    .HasIndex(f => new { f.UtilisateurId, f.MealDBId })
    .IsUnique();
```

**Option 3 - Cacher les requêtes TheMealDB**:
```csharp
// Polly avec cache
var cachePolicy = Policy.Cache<HttpResponseMessage>(
    _cacheProvider,
    TimeSpan.FromMinutes(5)
);
```

---

### Mémoire haute

**Symptôme**: Application consomme beaucoup de mémoire.

**Causes possibles**:
1. Cache non limité
2. Fuites mémoire
3. Beaucoup de données chargées en mémoire

**Solutions**:

**Option 1 - Limiter le cache**:
```csharp
var options = new MemoryCacheOptions
{
    SizeLimit = 1024 * 1024 * 100 // 100 MB max
};
```

**Option 2 - Paginer les résultats**:
```csharp
public async Task<PagedResult<RecetteDTO>> GetFavoritesAsync(int page = 1, int pageSize = 20)
{
    var skip = (page - 1) * pageSize;
    var favoris = await _context.Favoris
        .Skip(skip)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<RecetteDTO>
    {
        Items = favoris,
        TotalCount = await _context.Favoris.CountAsync(),
        Page = page,
        PageSize = pageSize
    };
}
```

---

## Problèmes de Sécurité

### SQL Injection possible

**Symptôme**: Utiliser la concaténation de strings dans les requêtes.

**Problème**:
```csharp
// ❌ DANGEREUX
var query = $"SELECT * FROM Recettes WHERE Titre = '{titre}'";
```

**Solution**: Utiliser Entity Framework (protégé par défaut):
```csharp
// ✅ Sûr
var recettes = await _context.Recettes
    .Where(r => r.Titre == titre)
    .ToListAsync();
```

---

### XSS (Cross-Site Scripting)

**Symptôme**: Code HTML exécuté dans le navigateur.

**Problème**:
```csharp
// ❌ DANGEREUX
@Html.Raw(recette.Instructions)
```

**Solution**: Laisser Blazor échapper le contenu:
```csharp
// ✅ Sûr
@recette.Instructions
// Ou utiliser @Html.Raw seulement pour du contenu de confiance
```

---

### Weak Password

**Symptôme**: Les mots de passe faibles sont acceptés.

**Solution**: Valider les critères:
```csharp
if (password.Length < 8 ||
    !password.Any(char.IsUpper) ||
    !password.Any(char.IsDigit) ||
    !password.Any(c => !char.IsLetterOrDigit(c)))
{
    return BadRequest("Password must be at least 8 chars with uppercase, digit, and symbol");
}
```

---

## Logs pour Déboguer

### Activer les logs détaillés

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "System.Net.Http": "Debug",
      "menuMalin.Server": "Debug"
    }
  }
}
```

### Logger une requête HTTP

```csharp
_logger.LogInformation("API Call: {Method} {Url}", request.Method, request.RequestUri);
_logger.LogDebug("Response Status: {Status}", response.StatusCode);
```

### Afficher les requêtes SQL

Dans `ApplicationDbContext`:
```csharp
optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
```

---

## Support et Ressources

- **Docs**: Voir `/docs/` folder
- **Issues**: Créer une issue sur GitHub
- **Email**: adrien.mertens@example.com

