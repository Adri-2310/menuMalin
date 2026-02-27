using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using menuMalin.Server.Data;
using menuMalin.Server.Auth;
using menuMalin.Server.Repositories;
using menuMalin.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Configuration des Services
// ========================================

// Configuration Auth0Settings depuis appsettings.json
var auth0Settings = new Auth0Settings();
builder.Configuration.GetSection("Auth0").Bind(auth0Settings);
builder.Services.AddSingleton(auth0Settings);

// Ajouter Entity Framework Core avec MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// BFF: Ajouter l'authentification par Cookies (au lieu de JWT Bearer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Auth0";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Cookie.Name = ".AspNetCore.Cookies";
    options.Cookie.HttpOnly = true;
    // En mode Hosted, frontend et backend sont sur le même port - utiliser Strict
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    options.Cookie.Path = "/";
    options.Events.OnSignedIn += context =>
    {
        // Logs pour debug
        System.Console.WriteLine($"✅ User signed in: {context.Principal?.Identity?.Name}");
        System.Console.WriteLine($"   Cookie created with SameSite=None; Secure; Path=/");
        return System.Threading.Tasks.Task.CompletedTask;
    };
    options.Events.OnValidatePrincipal += context =>
    {
        System.Console.WriteLine($"🔐 Cookie validation: IsAuthenticated={context.Principal?.Identity?.IsAuthenticated}");
        return System.Threading.Tasks.Task.CompletedTask;
    };
    // Gérer les redirects pour les endpoints API
    // Au lieu de rediriger vers la page de login, retourner 401 JSON
    options.Events.OnRedirectToLogin = context =>
    {
        // Si c'est une requête API (fetch depuis Blazor), retourner 401 au lieu de rediriger
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            System.Console.WriteLine($"⚠️ API endpoint {context.Request.Path} accédé sans authentification - retour 401");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\":\"Not authenticated\"}");
        }
        // Pour les autres requêtes (pages HTML du navigateur), garder le redirect
        context.Response.Redirect(context.RedirectUri);
        return System.Threading.Tasks.Task.CompletedTask;
    };
})
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = $"https://{auth0Settings.Domain}";
    options.ClientId = auth0Settings.ClientId;
    options.ClientSecret = auth0Settings.ClientSecret;
    options.ResponseType = "code";
    options.CallbackPath = new PathString("/api/auth/callback");

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    // En mode Hosted (même port), utiliser Strict pour la sécurité
    options.CorrelationCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.CorrelationCookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    options.NonceCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.NonceCookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;

    // Gérer les erreurs d'authentification et configurer le redirect post-authentification
    options.Events = new OpenIdConnectEvents
    {
        OnRemoteFailure = context =>
        {
            // Si l'utilisateur refuse l'autorisation, rediriger vers l'accueil
            if (context.Failure?.Message?.Contains("access_denied") == true)
            {
                System.Console.WriteLine("⚠️ Utilisateur a refusé l'autorisation - redirection vers l'accueil");
                context.Response.Redirect("https://localhost:7057/");
                context.HandleResponse();
                return System.Threading.Tasks.Task.CompletedTask;
            }

            // Pour les autres erreurs, les laisser se propager normalement
            return System.Threading.Tasks.Task.CompletedTask;
        },
        OnRedirectToIdentityProviderForSignOut = context =>
        {
            // Redirige vers le frontend après la déconnexion
            context.Response.Redirect("https://localhost:7057/");
            context.HandleResponse();
            return System.Threading.Tasks.Task.CompletedTask;
        },
        OnTicketReceived = context =>
        {
            // Après l'authentification réussie, rediriger vers le frontend
            // Retourner une page HTML avec un délai au lieu de rediriger directement
            // Cela donne au navigateur le temps d'établir le cookie cross-origin
            System.Console.WriteLine($"✅ Authentification réussie - Redirection vers le frontend");

            var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>Authentification...</title>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
            background: linear-gradient(135deg, #2d6a4f 0%, #40916c 100%);
        }
        .container {
            text-align: center;
            background: white;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
        }
        .spinner {
            border: 4px solid #f3f3f3;
            border-top: 4px solid #2d6a4f;
            border-radius: 50%;
            width: 40px;
            height: 40px;
            animation: spin 1s linear infinite;
            margin: 0 auto 20px;
        }
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        h1 {
            color: #2d6a4f;
            margin: 0;
            font-size: 24px;
        }
        p {
            color: #666;
            margin: 10px 0 0;
            font-size: 14px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='spinner'></div>
        <h1>Authentification réussie</h1>
        <p>Redirection en cours...</p>
    </div>
    <script>
        setTimeout(function() {
            window.location.href = 'https://localhost:7057/';
        }, 1500);
    </script>
</body>
</html>";

            context.HttpContext.Response.ContentType = "text/html";
            context.HttpContext.Response.WriteAsync(html);
            context.HandleResponse();
            return System.Threading.Tasks.Task.CompletedTask;
        }
    };
});

// Ajouter OpenApi pour la documentation
builder.Services.AddOpenApi();

// Ajouter les contrôleurs
builder.Services.AddControllers();

// Enregistrer TheMealDB HttpClient
builder.Services.AddHttpClient<ITheMealDBService, TheMealDBService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
    });

// Enregistrer les Repositories (Scoped)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddScoped<IUserRecipeRepository, UserRecipeRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Enregistrer les Services (Scoped)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IUserRecipeService, UserRecipeService>();

// Enregistrer le Service Email (Scoped pour éviter les problèmes avec les dépendances Scoped)
builder.Services.AddScoped<IEmailService, EmailService>();

// Note: CORS n'est plus nécessaire en mode Hosted Blazor (frontend + backend sur le même port)

// ========================================
// Configuration du Pipeline HTTP
// ========================================

var app = builder.Build();

// Appliquer les migrations automatiquement au démarrage (DEV seulement)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    app.MapOpenApi();
}

// HTTPS redirection seulement en production
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

// Servir les fichiers statiques du frontend Blazor WASM (doit être avant UseAuthentication)
app.UseStaticFiles();

// Ajouter l'authentification avant l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback vers index.html pour les routes Blazor (SPA routing)
app.MapFallbackToFile("index.html");

app.Run();