using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using menuMalin.Server.Data;
using menuMalin.Server.Repositories;
using menuMalin.Server.Services;

var builder = WebApplication.CreateBuilder(args);

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