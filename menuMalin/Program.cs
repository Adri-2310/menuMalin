using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using menuMalin;
using menuMalin.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Client HTTP de base
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 2. PHASE 3 : Client HTTP Typé pour l'API TheMealDB
// On enregistre l'interface IRecipeService liée à sa classe RecipeService
builder.Services.AddHttpClient<IRecipeService, RecipeService>(client => 
{
    client.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
});

// 3. PHASE 1.2 : Configuration Auth0 (OIDC)
builder.Services.AddOidcAuthentication(options =>
{
    // On lie ici au nom "Auth0" défini dans ton fichier JSON
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
});

await builder.Build().RunAsync();