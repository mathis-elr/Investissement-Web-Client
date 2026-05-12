using ApexCharts;
using Blazored.Toast;
using Investissement_WebClient.Application.Workers;
using Investissement_WebClient.Application.Services.FluxBancaires;
using Investissement_WebClient.Application.Services.FluxInvestissements;
using Investissement_WebClient.Application.Services.PowensApi;
using Investissement_WebClient.Application.Services.TradeRepublicApi;
using Investissement_WebClient.Application.Services.ValeurPatrimoines;
using Investissement_WebClient.Application.Services.YahooFinanceApi;
using Investissement_WebClient.Infrastructure;
using Investissement_WebClient.Web.Components;
using Investissement_WebClient.Web.Components.ViewsModels;
using Microsoft.EntityFrameworkCore;
using Investissement_WebClient.Domain.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<InvestissementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));


var section = builder.Configuration.GetSection("PowensApiAcces");
PowensConfiguration.ClientId = section["client_id"];
PowensConfiguration.ClientSecret = section["client_secret"];
PowensConfiguration.BaseUrl = section["BaseUrl"];
PowensConfiguration.RedirectUri = section["RedirectUri"];
PowensConfiguration.ConnectUrl = section["ConnectUrl"];

builder.Services.AddScoped<IFluxInvestissementService, FluxInvestissementService>();
builder.Services.AddScoped<IValeurPatrimoineService, ValeurPatrimoineService>();
builder.Services.AddScoped<IFluxBancaireService, FluxBancaireService>();

builder.Services.AddScoped<IYahooFinanceApiService, YahooFinanceApiService>();
builder.Services.AddScoped<ITradeRepublicApiService, TradeRepublicApiService>();
builder.Services.AddScoped<IPowensApiService, PowensApiService>();

builder.Services.AddScoped<InvestissementViewModel>();
builder.Services.AddScoped<PatrimoineViewModel>();
builder.Services.AddScoped<BudgetViewModel>();
builder.Services.AddScoped<ProfilViewModel>();

builder.Services.AddHostedService<EnregistrementValeurPatrimoineWorker>();

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
