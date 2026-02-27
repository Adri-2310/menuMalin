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
    // Pour cross-origin HTTPS→HTTPS (frontend 7777, backend 7057), utiliser None
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
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

    // Correlation cookie doit permettre cross-origin
    options.CorrelationCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.CorrelationCookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.NonceCookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.NonceCookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;

    // Gérer les erreurs d'authentification (ex: utilisateur refuse l'autorisation)
    options.Events = new OpenIdConnectEvents
    {
        OnRemoteFailure = context =>
        {
            // Si l'utilisateur refuse l'autorisation, rediriger vers l'accueil
            if (context.Failure?.Message?.Contains("access_denied") == true)
            {
                System.Console.WriteLine("⚠️ Utilisateur a refusé l'autorisation - redirection vers l'accueil");
                context.Response.Redirect("https://localhost:7777/");
                context.HandleResponse();
                return System.Threading.Tasks.Task.CompletedTask;
            }

            // Pour les autres erreurs, les laisser se propager normalement
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

// Ajouter CORS si nécessaire (pour le frontend Blazor)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7216",
            "http://localhost:5149",
            "https://localhost:7777"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
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

// Utiliser CORS
app.UseCors("AllowBlazor");

// Ajouter l'authentification avant l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();