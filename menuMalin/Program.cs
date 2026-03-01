using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using menuMalin;
using menuMalin.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Charger les URLs depuis appsettings.json
var config = builder.Configuration;
var backendUrl = config["ApiConfig:BackendUrl"] ?? "https://localhost:7057";
var frontendUrl = config["ApiConfig:FrontendUrl"] ?? "https://localhost:7777";

// ========================================
// Configuration HTTP Clients (BFF Mode)
// ========================================

// 1. Client HTTP de base
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 2. Client HTTP pour l'API Backend (avec credentials pour cookies)
builder.Services.AddHttpClient<IServiceApiHttp, ServiceApiHttp>(client =>
{
    client.BaseAddress = new Uri($"{backendUrl}/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigureHttpClient(client =>
{
    // Inclure les cookies dans les requêtes (automatique en mode BFF)
});

// ========================================
// Configuration des Services Frontend
// ========================================

// LocalStorage (Blazored.LocalStorage)
builder.Services.AddBlazoredLocalStorage();

// LocalStorageService (pour le profil utilisateur)
builder.Services.AddScoped<ServiceStockageLocal>();

// ThemeService (pour le thème dark/light)
builder.Services.AddScoped<IServiceTheme, ServiceTheme>();

// État partagé d'authentification (Singleton pour cohérence globale)
builder.Services.AddSingleton<ServiceEtatAuthentification>();

// Services d'authentification (BFF) - avec HttpClient dédié
builder.Services.AddHttpClient<IServiceAuthentification, ServiceAuthentification>(client =>
{
    client.BaseAddress = new Uri($"{backendUrl}/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Services métier (Frontend)
builder.Services.AddScoped<IServiceContact, ServiceContact>();
builder.Services.AddHttpClient<IServiceRecette, ServiceRecette>(client =>
{
    client.BaseAddress = new Uri($"{backendUrl}/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped<IServiceRecetteFrontend, ServiceRecetteFrontend>();
builder.Services.AddScoped<IServiceFavorisFrontend, ServiceFavorisFrontend>();
builder.Services.AddScoped<IServiceRecetteUtilisateur, ServiceRecetteUtilisateur>();
builder.Services.AddScoped<IServiceAuthPage, ServiceAuthPage>();

// Service d'upload d'images
builder.Services.AddHttpClient<IServiceTeleversement, ServiceTeleversement>(client =>
{
    client.BaseAddress = new Uri($"{backendUrl}/api/");
});

await builder.Build().RunAsync();
