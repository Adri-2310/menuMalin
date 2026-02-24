using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using menuMalin;
using menuMalin.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ========================================
// Configuration HTTP Clients
// ========================================

// 1. Client HTTP de base
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 2. Client HTTP pour l'API Backend (menuMalin.Server)
builder.Services.AddHttpClient<IHttpApiService, HttpApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5266/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// 3. Client HTTP pour l'API TheMealDB
builder.Services.AddHttpClient<IRecipeService, RecipeService>(client =>
{
    client.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
});

// ========================================
// Configuration Auth0 (OIDC)
// ========================================

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
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

// Services métier (Frontend)
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IRecipeServiceFrontend, RecipeServiceFrontend>();
builder.Services.AddScoped<IFavoriteServiceFrontend, FavoriteServiceFrontend>();
builder.Services.AddScoped<IUserRecipeService, UserRecipeService>();

await builder.Build().RunAsync();