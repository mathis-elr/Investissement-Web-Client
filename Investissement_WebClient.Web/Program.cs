using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Infrastructure.APIs.TradeRepublic;
using Investissement_WebClient.Web.Components.ViewsModels.Budget;
using Investissement_WebClient.Infrastructure.APIs.YahooFinance;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.Services.Encrypt;
using Investissement_WebClient.Infrastructure.Repositories;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Infrastructure.APIs.Powens;
using Investissement_WebClient.Web.Components.ViewsModels;
using Investissement_WebClient.Infrastructure.Workers;
using Microsoft.AspNetCore.Components.Authorization;
using Investissement_WebClient.Application.Services;
using Investissement_WebClient.Web.GestionSession;
using Investissement_WebClient.Infrastructure;
using Investissement_WebClient.Web.Components;
using Microsoft.EntityFrameworkCore;
using Blazored.Toast;
using ApexCharts;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "fr-FR" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<InvestissementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

builder.Services.Configure<CryptOptions>(
    builder.Configuration.GetSection("Security"));

builder.Services.Configure<PowensApiOptions>(
    builder.Configuration.GetSection("PowensApi"));

builder.Services.Configure<TradeRepublicApiOptions>(
    builder.Configuration.GetSection("TradeRepublicApi"));

builder.Services.Configure<YahooFinanceApiOptions>(
    builder.Configuration.GetSection("YahooFinanceApi"));


// services
builder.Services.AddScoped<IFluxInvestissementService, FluxInvestissementService>();
builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();
builder.Services.AddScoped<IValeurPatrimoineService, ValeurPatrimoineService>();
builder.Services.AddScoped<IFluxBancaireService, FluxBancaireService>();
builder.Services.AddScoped<IActifService, ActifService>();

builder.Services.AddScoped<IYahooFinanceApiService, YahooFinanceApiService>();

builder.Services.AddHttpClient<ITradeRepublicApiService, TradeRepublicApiService>();
builder.Services.AddHttpClient<IPowensApiService, PowensApiService>();

builder.Services.AddScoped<ICryptService, CryptService>();

// views models
builder.Services.AddScoped<InvestissementViewModel>();
builder.Services.AddScoped<InscriptionViewModel>();
builder.Services.AddScoped<PatrimoineViewModel>();
builder.Services.AddScoped<ConnexionViewModel>();
builder.Services.AddScoped<BudgetViewModel>();
builder.Services.AddScoped<DashboardViewModel>();


// repositories
builder.Services.AddScoped<IFluxInvestissementRepository, FluxInvestissementRepository>();
builder.Services.AddScoped<ITradeRepublicAccesRepository, TradeRepublicAccesRepository>();
builder.Services.AddScoped<IValeurPatrimoineRepository, ValeurPatrimoineRepository>();
builder.Services.AddScoped<ICategorieFluxRepository, CategorieFluxRepository>();
builder.Services.AddScoped<IFluxBancaireRepository, FluxBancaireRepository>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IBanqueAccesRepository, BanqueAccesRepository>();
builder.Services.AddScoped<IActifRepository, ActifRepository>();


// workers
builder.Services.AddHostedService<EnregistrementValeurPatrimoineWorker>();
builder.Services.AddSingleton<RecuperationFluxBancairesWorker>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<RecuperationFluxBancairesWorker>());


builder.Services.AddAuthentication("Manual")
    .AddCookie("Manual", options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorizationCore();

// 2. On garde ton Provider et ton LocalStorage
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<SessionService>();


builder.Services.AddApexCharts();
builder.Services.AddBlazoredToast();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRequestLocalization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.Run();
