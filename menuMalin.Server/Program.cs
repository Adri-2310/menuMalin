using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

// Ajouter l'authentification JWT avec Auth0
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Settings.Domain}";
        options.Audience = auth0Settings.Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
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
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();

// Enregistrer les Services (Scoped)
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// Ajouter CORS si nécessaire (pour le frontend Blazor)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7216",
            "http://localhost:5149"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
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

app.UseHttpsRedirection();

// Utiliser CORS
app.UseCors("AllowBlazor");

// Ajouter l'authentification avant l'autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();