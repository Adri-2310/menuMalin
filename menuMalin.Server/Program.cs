using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using menuMalin.Server.Donnees;
using menuMalin.Server.Depots;
using menuMalin.Server.Depots.Interfaces;
using menuMalin.Server.Services;
using menuMalin.Server.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Configuration Serilog pour les fichiers de log
// ========================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: "logs/menuMalin-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

// ========================================
// Configuration des Services
// ========================================

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
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Cookie.Name = ".AspNetCore.Cookies";
    options.Cookie.HttpOnly = true;
    // En mode développement, frontend et backend sur ports différents - utiliser Lax pour permettre les cookies cross-site
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    options.Cookie.Path = "/";
    options.Events.OnSignedIn += context =>
    {
        // Logs pour debug
        System.Console.WriteLine($"✅ Utilisateur signed in: {context.Principal?.Identity?.Name}");
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
});

// Ajouter OpenApi pour la documentation
builder.Services.AddOpenApi();

// Ajouter les contrôleurs
builder.Services.AddControllers();

// Enregistrer TheMealDB HttpClient
builder.Services.AddHttpClient<IServiceMealDB, ServiceMealDB>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
    });

// Enregistrer les Dépôts (Scoped)
builder.Services.AddScoped<IDepotUtilisateur, DepotUtilisateur>();
builder.Services.AddScoped<IDepotRecette, DepotRecette>();
builder.Services.AddScoped<IDepotFavori, DepotFavori>();
builder.Services.AddScoped<IDepotRecetteUtilisateur, DepotRecetteUtilisateur>();
builder.Services.AddScoped<IDepotMessage, DepotMessage>();

// Enregistrer les Services (Scoped)
builder.Services.AddScoped<IServiceUtilisateur, ServiceUtilisateur>();
builder.Services.AddScoped<IServiceRecette, ServiceRecette>();
builder.Services.AddScoped<IServiceFavoris, ServiceFavoris>();
builder.Services.AddScoped<IServiceRecetteUtilisateur, ServiceRecetteUtilisateur>();

// Enregistrer le Service Email (Scoped pour éviter les problèmes avec les dépendances Scoped)
builder.Services.AddScoped<IServiceEmail, ServiceEmail>();

// Configuration CORS (développement: frontend et backend peuvent être sur le même serveur ou ports différents)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7777", "https://localhost:7057")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // IMPORTANT: permettre les cookies
    });
});

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

// Appliquer CORS (avant l'authentification et l'autorisation)
app.UseCors("AllowFrontend");

// Ajouter l'authentification avant l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback vers index.html pour les routes Blazor (SPA routing)
app.MapFallbackToFile("index.html");

app.Run();