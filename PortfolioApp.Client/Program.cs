using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PortfolioApp.Client.Components;
using PortfolioApp.Client.Services;
using PortfolioApp.Client.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Lire l'URL de l'API depuis la configuration
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5158";

// Register TokenService
builder.Services.AddScoped<ITokenService, TokenService>();

// Register AuthMessageHandler
builder.Services.AddScoped<AuthMessageHandler>();

// Configuration HttpClient pour l'API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register all API services with AuthMessageHandler
builder.Services.AddHttpClient<IPortfolioService, PortfolioService>(client =>
{
 client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<IProjectService, ProjectService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<ISkillService, SkillService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<IToolService, ToolService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<IResumeService, ResumeService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddHttpClient<IContactService, ContactService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Toast notification service
builder.Services.AddScoped<ToastService>();

await builder.Build().RunAsync();
