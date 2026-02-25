using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using menuMalin;
using menuMalin.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ========================================
// Configuration HTTP Clients (BFF Mode)
// ========================================

// 1. Client HTTP de base
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 2. Client HTTP pour l'API Backend (avec credentials pour cookies)
builder.Services.AddHttpClient<IHttpApiService, HttpApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7057/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigureHttpClient(client =>
{
    // Inclure les cookies dans les requêtes (automatique en mode BFF)
});

// 3. Client HTTP pour l'API TheMealDB
builder.Services.AddHttpClient<IRecipeService, RecipeService>(client =>
{
    client.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
});

// ========================================
// Configuration des Services Frontend
// ========================================

// LocalStorage (Blazored.LocalStorage)
builder.Services.AddBlazoredLocalStorage();

// LocalStorageService (pour le profil utilisateur)
builder.Services.AddScoped<LocalStorageService>();

// ThemeService (pour le thème dark/light)
builder.Services.AddScoped<IThemeService, ThemeService>();

// Services d'authentification (BFF) - avec HttpClient dédié
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7057/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Services métier (Frontend)
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IRecipeServiceFrontend, RecipeServiceFrontend>();
builder.Services.AddScoped<IFavoriteServiceFrontend, FavoriteServiceFrontend>();
builder.Services.AddScoped<IUserRecipeService, UserRecipeService>();

// Service d'upload d'images
builder.Services.AddHttpClient<IUploadService, UploadService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7057/api/");
});

await builder.Build().RunAsync();
