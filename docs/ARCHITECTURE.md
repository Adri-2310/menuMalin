# Architecture et Principes POO

Vue d'ensemble de l'architecture MenuMalin et implémentation des principes de Programmation Orientée Objet.

## Structure du Projet

```
menuMalin/
├── menuMalin.Server/          # Backend ASP.NET Core
│   ├── Controllers/           # API Endpoints
│   ├── Services/              # Logique métier
│   ├── Depots/                # Data Access Layer
│   ├── Donnees/               # Entity Framework & DbContext
│   ├── Models/                # DTOs et Entités
│   ├── Properties/            # Configuration
│   └── Program.cs             # Startup configuration
├── menuMalin/                 # Frontend Blazor WASM
│   ├── Pages/                 # Composants de pages
│   ├── Components/            # Composants réutilisables
│   ├── Services/              # Services HTTP et métier
│   ├── Shared/                # Layouts et navigation
│   └── wwwroot/               # Assets statiques
└── docs/                      # Documentation
```

## Principes POO Implémentés

### 1. Encapsulation

**Définition**: Masquer les détails internes et exposer uniquement une interface contrôlée.

**Implémentation**:

```csharp
// Service avec encapsulation
public interface IServiceFavoris
{
    Task<List<RecetteDTO>> GetFavorisAsync();
    Task<bool> AddFavoriAsync(string recipeId);
    Task<bool> RemoveFavoriAsync(string recipeId);
}

public class ServiceFavoris : IServiceFavoris
{
    private readonly IDepotFavori _depot;

    public ServiceFavoris(IDepotFavori depot)
    {
        _depot = depot; // Injecter plutôt que d'instancier directement
    }

    public async Task<List<RecetteDTO>> GetFavorisAsync()
    {
        // Logique métier cachée - le caller ne voit que l'interface
        var favoris = await _depot.GetAsync();
        return favoris.Select(f => f.ToDTO()).ToList();
    }
}
```

**Avantages**:
- Facilite les tests unitaires (injection de dépendances)
- Peut modifier l'implémentation sans casser le contrat
- Réduit le couplage entre composants

### 2. Abstraction

**Définition**: Créer une interface simple pour masquer la complexité interne.

**Implémentation**:

```csharp
// Couche Dépôt: abstrait la persistance des données
public interface IDepotRecette
{
    Task<Recette?> GetByIdAsync(string id);
    Task<List<Recette>> GetAllAsync();
    Task AddAsync(Recette recette);
    Task UpdateAsync(Recette recette);
    Task DeleteAsync(string id);
}

public class DepotRecette : IDepotRecette
{
    private readonly ApplicationDbContext _context;

    public async Task<Recette?> GetByIdAsync(string id)
    {
        return await _context.Recettes.FirstOrDefaultAsync(r => r.Id == id);
    }

    // L'implémentation EF Core est cachée derrière l'interface
}

// Le service utilise l'interface, pas le contexte EF directement
public class ServiceRecette
{
    private readonly IDepotRecette _depot;

    public async Task<RecetteDTO?> GetRecipeAsync(string id)
    {
        var recette = await _depot.GetByIdAsync(id);
        return recette?.ToDTO();
    }
}
```

**Pattern SOLID - Dependency Inversion (DIP)**:
- Service dépend d'une abstraction (IDepotRecette), pas de l'implémentation concrète
- Facilite les tests et les changements de technologie

### 3. Héritage et Hiérarchie

**Définition**: Créer une hiérarchie de classes/interfaces pour réutiliser le code et spécialiser le comportement.

**Implémentation - Exception Hierarchy**:

```csharp
// Classe de base: abstrait les erreurs métier
public abstract class ErreurMetierException : Exception
{
    public string Code { get; }

    protected ErreurMetierException(string message, string code)
        : base(message)
    {
        Code = code;
    }
}

// Spécialisations concrètes
public class ErreurApiException : ErreurMetierException
{
    public int StatusCode { get; }

    public ErreurApiException(int statusCode, string contenu)
        : base($"API Error {statusCode}: {contenu}", "API_ERROR")
    {
        StatusCode = statusCode;
    }
}

public class ErreurReseauException : ErreurMetierException
{
    public ErreurReseauException(string message, Exception? inner = null)
        : base(message, "NETWORK_ERROR")
    {
    }
}

// Utilisation polymorphe
try
{
    var data = await _httpClient.GetAsync(...);
}
catch (ErreurApiException ex) when (ex.StatusCode == 401)
{
    // Gérer auth error
}
catch (ErreurReseauException ex)
{
    // Gérer network error
}
```

**Avantages**:
- Hiérarchie claire des erreurs
- Gestion spécialisée par type d'erreur
- Code maintenable et évolutif

### 4. Polymorphisme

**Définition**: Même interface, différentes implémentations selon le contexte.

**Implémentation - Multiple Services**:

```csharp
// Interface commune
public interface IServiceRecette
{
    Task<RecetteDTO?> GetRecipeAsync(string id);
    Task<List<RecetteDTO>> SearchAsync(string query);
}

// Implémentation 1: TheMealDB API
public class ServiceMealDB : IServiceRecette
{
    private readonly HttpClient _httpClient;

    public async Task<RecetteDTO?> GetRecipeAsync(string id)
    {
        // Appel API TheMealDB
        var response = await _httpClient.GetAsync($"lookup.php?i={id}");
        var data = await response.Content.ReadAsAsync<MealDBResponse>();
        return data?.meals?[0].ToDTO();
    }
}

// Implémentation 2: Base de données locale
public class ServiceRecetteUtilisateur : IServiceRecette
{
    private readonly IDepotRecetteUtilisateur _depot;

    public async Task<RecetteDTO?> GetRecipeAsync(string id)
    {
        // Requête base de données
        var recette = await _depot.GetByIdAsync(id);
        return recette?.ToDTO();
    }
}

// Utilisation - le code client ne sait pas quelle implémentation est utilisée
public class ControleurRecette
{
    private readonly IServiceRecette _serviceAPI;
    private readonly IServiceRecette _serviceLocal;

    public async Task<ActionResult> GetRecipe(string id)
    {
        // Essayer local d'abord, puis l'API
        var recipe = await _serviceLocal.GetRecipeAsync(id)
                  ?? await _serviceAPI.GetRecipeAsync(id);
        return Ok(recipe);
    }
}
```

**Avantages**:
- Flexibilité: changer la source de données facilement
- Testabilité: utiliser des mocks au lieu des vraies implémentations
- Extensibilité: ajouter de nouvelles sources sans modifier le code existant

### 5. Composition

**Définition**: Construire des objets complexes en combinant des objets simples (préférer la composition à l'héritage).

**Implémentation - Service Composition**:

```csharp
// Services simples et spécialisés
public interface IServiceEmail
{
    Task SendAsync(string to, string subject, string body);
}

public interface IServiceNotification
{
    Task NotifyAsync(string userId, string message);
}

public interface IServiceAudit
{
    Task LogAsync(string action, string userId, string details);
}

// Service complexe: compose les services simples
public class ServiceRecetteUtilisateur
{
    private readonly IDepotRecetteUtilisateur _depot;
    private readonly IServiceEmail _serviceEmail;
    private readonly IServiceNotification _serviceNotification;
    private readonly IServiceAudit _serviceAudit;

    public ServiceRecetteUtilisateur(
        IDepotRecetteUtilisateur depot,
        IServiceEmail serviceEmail,
        IServiceNotification serviceNotification,
        IServiceAudit serviceAudit)
    {
        _depot = depot;
        _serviceEmail = serviceEmail;
        _serviceNotification = serviceNotification;
        _serviceAudit = serviceAudit;
    }

    public async Task CreateAsync(CreateRecetteRequest request, string userId)
    {
        // 1. Créer l'entité
        var recette = new RecetteUtilisateur { ... };
        await _depot.AddAsync(recette);

        // 2. Notifier l'utilisateur
        await _serviceNotification.NotifyAsync(
            userId,
            "Votre recette a été créée avec succès");

        // 3. Envoyer un email
        await _serviceEmail.SendAsync(
            user.Email,
            "Nouvelle Recette",
            $"Vous avez créé: {recette.Titre}");

        // 4. Logger l'action
        await _serviceAudit.LogAsync(
            "CREATE_RECIPE",
            userId,
            $"Created recipe: {recette.Id}");
    }
}
```

**Avantages**:
- Chaque service a une responsabilité unique
- Facile à tester isolément
- Réutilisable dans d'autres services

## Patterns et Architectures

### Repository Pattern

Sépare la logique d'accès aux données de la logique métier:

```
Controller → Service → Repository → DbContext → Database
   ↑           ↑          ↑            ↑
   API Endpoint  Business Logic   Data Access   Persistence
```

**Avantage**: Changer de base de données nécessite seulement de modifier le Repository.

### Dependency Injection (DI)

Configuration dans `Program.cs`:

```csharp
// Enregistrer les services
builder.Services.AddScoped<IDepotUtilisateur, DepotUtilisateur>();
builder.Services.AddScoped<IServiceUtilisateur, ServiceUtilisateur>();
builder.Services.AddScoped<IServiceEmail, ServiceEmail>();

// ASP.NET injecte automatiquement dans les constructeurs
public class ControleurUtilisateur
{
    private readonly IServiceUtilisateur _service;

    public ControleurUtilisateur(IServiceUtilisateur service)
    {
        _service = service; // Injecté automatiquement
    }
}
```

### Lifetime Management

- **Transient**: Nouvelle instance à chaque utilisation (stateless)
- **Scoped**: Nouvelle instance par requête HTTP (par défaut pour les services)
- **Singleton**: Une instance pour toute l'application (HttpClient, logger)

```csharp
builder.Services.AddTransient<ITemporaryService, TemporaryService>();
builder.Services.AddScoped<IServiceUtilisateur, ServiceUtilisateur>();
builder.Services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
```

## Principes SOLID

### S - Single Responsibility
Chaque classe a une seule raison de changer.

```csharp
// ❌ Mauvais: classe mixte
public class ServiceUtilisateur
{
    public async Task RegisterAsync(RegisterRequest request)
    {
        // Valider
        // Hashage password
        // Créer utilisateur
        // Envoyer email
        // Logger
    }
}

// ✅ Bon: séparation des responsabilités
public class ServiceUtilisateur
{
    private readonly IServiceEmail _email;
    private readonly IServicePassword _password;
    private readonly IServiceAudit _audit;
    private readonly IDepotUtilisateur _depot;

    public async Task RegisterAsync(RegisterRequest request)
    {
        var hashedPassword = await _password.HashAsync(request.Password);
        var user = new Utilisateur { ... };
        await _depot.AddAsync(user);
        await _email.SendWelcomeAsync(user.Email);
        await _audit.LogAsync("USER_REGISTERED", user.Id);
    }
}
```

### O - Open/Closed
Ouvert à l'extension, fermé à la modification.

```csharp
// Extensible via polymorphisme
public interface IServiceNotification
{
    Task SendAsync(string userId, string message);
}

// Nouvelles implémentations sans modifier le code existant
public class ServiceEmailNotification : IServiceNotification { }
public class ServiceSmsNotification : IServiceNotification { }
public class ServicePushNotification : IServiceNotification { }
```

### L - Liskov Substitution
Les sous-classes peuvent remplacer la classe parent sans casser la logique.

```csharp
public class ServiceRecetteMealDB : IServiceRecette { }
public class ServiceRecetteLocal : IServiceRecette { }

// Les deux implémentations sont interchangeables
IServiceRecette service = useLocal ?
    new ServiceRecetteLocal() :
    new ServiceRecetteMealDB();
```

### I - Interface Segregation
Préférer plusieurs petites interfaces à une grande.

```csharp
// ❌ Mauvais: grosse interface
public interface IServiceUtilisateur
{
    Task CreateAsync(...);
    Task UpdateAsync(...);
    Task DeleteAsync(...);
    Task LoginAsync(...);
    Task LogoutAsync(...);
    Task ResetPasswordAsync(...);
    Task SendVerificationEmailAsync(...);
}

// ✅ Bon: interfaces spécialisées
public interface IServiceUtilisateurAuth { Task LoginAsync(...); }
public interface IServiceUtilisateurProfile { Task UpdateAsync(...); }
public interface IServiceUtilisateurPassword { Task ResetAsync(...); }
```

### D - Dependency Inversion
Dépendre d'abstractions, pas de concrétions.

```csharp
// ❌ Mauvais: dépendance directe
public class ServiceRecette
{
    private readonly DepotRecette _depot = new();
}

// ✅ Bon: injection d'interface
public class ServiceRecette
{
    private readonly IDepotRecette _depot;

    public ServiceRecette(IDepotRecette depot)
    {
        _depot = depot;
    }
}
```

## Flux de Données

### Requête Utilisateur: Créer une Recette

```
Frontend (Blazor)
    ↓ HTTP POST /api/recipes
Backend (ASP.NET)
    ↓ CreateRecetteRequest (DTO)
Controller
    ↓ ServiceRecetteUtilisateur.CreateAsync(request, userId)
Service (Logique Métier)
    ↓ DepotRecetteUtilisateur.AddAsync(recette)
Repository (Data Access)
    ↓ DbContext.Recettes.AddAsync()
Entity Framework
    ↓ INSERT INTO recettes ...
MySQL Database
```

Réponse:
```
HTTP 201 Created + RecetteDTO
```

### Authentification et Autorisation

```
1. User Login (Frontend)
   ↓
2. ControleurAuthentification.LoginAsync()
   ↓
3. ServiceUtilisateur.ValidateCredentialsAsync()
   ↓
4. Créer ClaimsPrincipal avec ClaimsIdentity
   ↓
5. SignInAsync() → Cookie HttpOnly
   ↓
6. Frontend stocke userId dans localStorage
   ↓
7. Requêtes suivantes: Cookie automatiquement envoyé
```

## Résumé POO

| Principe | Utilisation | Bénéfice |
|----------|-----------|---------|
| Encapsulation | Services avec interfaces | Testabilité, découplage |
| Abstraction | Couche Repository, DTOs | Maintenabilité, évolutivité |
| Héritage | Hiérarchie d'exceptions | Code réutilisable, gestion centralisée |
| Polymorphisme | Multiple services (MealDB, Local) | Flexibilité, extensibilité |
| Composition | Services composés | Responsabilité unique, testabilité |

