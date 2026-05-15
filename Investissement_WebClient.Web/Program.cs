    using ApexCharts;
using Blazored.Toast;
using Investissement_WebClient.Application.Services.Actifs;
using Investissement_WebClient.Application.Services.Authentification;
using Investissement_WebClient.Application.Services.Encrypt;
using Investissement_WebClient.Application.Services.FluxBancaires;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.PowensApi;
using Investissement_WebClient.Application.Services.TradeRepublicApi;
using Investissement_WebClient.Application.Services.ValeurPatrimoines;
using Investissement_WebClient.Application.Services.YahooFinanceApi;
using Investissement_WebClient.Application.Workers;
using Investissement_WebClient.Domain.Configurations;
using Investissement_WebClient.Infrastructure;
using Investissement_WebClient.Web.Components;
using Investissement_WebClient.Web.Components.ViewsModels;
using Investissement_WebClient.Web.GestionSession;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

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


var sectionPowens = builder.Configuration.GetSection("PowensApi");
PowensApiConfiguration.ClientId = sectionPowens["client_id"] ?? throw new InvalidOperationException("La config 'PowensApi:ClientId' est absente.");
PowensApiConfiguration.ClientSecret = sectionPowens["client_secret"] ?? throw new InvalidOperationException("La config 'PowensApi:ClientSecret' est absente.");
PowensApiConfiguration.BaseUri = sectionPowens["BaseUri"] ?? throw new InvalidOperationException("La config 'PowensApi:BaseUri' est absente.");
PowensApiConfiguration.ConnectEndPoint = sectionPowens["ConnectEndPoint"] ?? throw new InvalidOperationException("La config 'PowensApi:ConnectEndPoint' est absente.");
PowensApiConfiguration.TokenEndPoint = sectionPowens["TokenEndPoint"] ?? throw new InvalidOperationException("La config 'PowensApi:TokenEndPoint' est absente.");
PowensApiConfiguration.AccountsEndPoint = sectionPowens["AccountsEndPoint"] ?? throw new InvalidOperationException("La config 'PowensApi:AccountsEndPoint' est absente.");
PowensApiConfiguration.RedirectUri = sectionPowens["RedirectUri"] ?? throw new InvalidOperationException("La config 'PowensApi:RedirectUri' est absente.");

var sectionTR = builder.Configuration.GetSection("TradeRepublicApi");
TradeRepublicApiConfiguration.MasterKey = sectionTR["MasterKey"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:MasterKey' est absente.");
TradeRepublicApiConfiguration.BaseUri = sectionTR["BaseUri"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:BaseUri' est absente.");
TradeRepublicApiConfiguration.RequestSmsEndPoint = sectionTR["RequestSmsEndPoint"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:RequestSmsEndPoint' est absente.");
TradeRepublicApiConfiguration.ConfirmSmsEndPoint = sectionTR["ConfirmSmsEndPoint"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:ConfirmSmsEndPoint' est absente.");
TradeRepublicApiConfiguration.DatasEndPoint = sectionTR["DatasEndPoint"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:DatasEndPoint' est absente.");
TradeRepublicApiConfiguration.CleeApiKey = sectionTR["CleeApiKey"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:Key' est absente.");
TradeRepublicApiConfiguration.CleeApiValue = sectionTR["CleeApiValue"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:Value' est absente.");
TradeRepublicApiConfiguration.NumTelKey = sectionTR["NumTelKey"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:NumTelKey' est absente.");
TradeRepublicApiConfiguration.NumTelValue = sectionTR["NumTelValue"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:NumTelValue' est absente.");
TradeRepublicApiConfiguration.PinKey = sectionTR["PinKey"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:PinKey' est absente.");
TradeRepublicApiConfiguration.PinValue = sectionTR["PinValue"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:PinValue' est absente.");
TradeRepublicApiConfiguration.DernierIdEnregistreKey = sectionTR["DernierIdEnregistreKey"] ?? throw new InvalidOperationException("La config 'TradeRepublicApi:DernierIdEnregistreKey' est absente.");

var sectionYahoo = builder.Configuration.GetSection("YahooFinanceApi");
YahooFinanceApiConfiguration.BaseUri = sectionYahoo["BaseUri"] ?? throw new InvalidOperationException("La config 'YahooFinanceApi:BaseUri' est absente.");
YahooFinanceApiConfiguration.SearchEndPoint = sectionYahoo["SearchEndPoint"] ?? throw new InvalidOperationException("La config 'YahooFinanceApi:SearchEndPoint' est absente.");



builder.Services.AddScoped<IFluxInvestissementService, FluxInvestissementService>();
builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();
builder.Services.AddScoped<IValeurPatrimoineService, ValeurPatrimoineService>();
builder.Services.AddScoped<IFluxBancaireService, FluxBancaireService>();
builder.Services.AddScoped<IActifService, ActifService>();

builder.Services.AddScoped<IYahooFinanceApiService, YahooFinanceApiService>();
builder.Services.AddScoped<ITradeRepublicApiService, TradeRepublicApiService>();
builder.Services.AddScoped<IPowensApiService, PowensApiService>();

builder.Services.AddHttpClient<ITradeRepublicApiService, TradeRepublicApiService>();
builder.Services.AddScoped<IYahooFinanceApiService, YahooFinanceApiService>();
builder.Services.AddHttpClient<IPowensApiService, PowensApiService>();

builder.Services.AddScoped<IEncryptService, EncryptService>();

builder.Services.AddScoped<InvestissementViewModel>();
builder.Services.AddScoped<PatrimoineViewModel>();
builder.Services.AddScoped<InscriptionViewModel>();
builder.Services.AddScoped<BudgetViewModel>();
builder.Services.AddScoped<ProfilViewModel>();
builder.Services.AddScoped<ConnexionViewModel>();

builder.Services.AddHostedService<EnregistrementValeurPatrimoineWorker>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/connexion";
    });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();

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
    .AddInteractiveServerRenderMode();

// requette vide pour cron job
app.MapGet("/api/update-data", async (BudgetViewModel bvm) =>
{
    await bvm.StartLoadData();
    return Results.Empty; 
});

app.Run();
