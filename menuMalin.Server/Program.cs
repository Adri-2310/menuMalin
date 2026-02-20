using Microsoft.EntityFrameworkCore;
using menuMalin.Server.Data;

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

// Ajouter OpenApi pour la documentation
builder.Services.AddOpenApi();

// Ajouter les contrôleurs
builder.Services.AddControllers();

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

app.UseAuthorization();

app.MapControllers();

app.Run();