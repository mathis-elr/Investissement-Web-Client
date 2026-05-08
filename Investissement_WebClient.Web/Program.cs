using ApexCharts;
using Blazored.Toast;
using Investissement_WebClient.Application.Background;
using Investissement_WebClient.Application.Services.CreditCoop;
using Investissement_WebClient.Application.Services.Investissement;
using Investissement_WebClient.Application.Services.Patrimoine;
using Investissement_WebClient.Application.Services.Powens;
using Investissement_WebClient.Application.Services.TradeRepublic;
using Investissement_WebClient.Application.Services.YahooFinance;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Investissement_WebClient.Web.Components;
using Investissement_WebClient.Web.Components.ViewsModels;
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

var section = builder.Configuration.GetSection("PowensApiAcces");
PowensAPIAcces.ClientId = section["client_id"];
PowensAPIAcces.ClientSecret = section["client_secret"];

builder.Services.AddScoped<IInvestissementService, InvestissementService>();
builder.Services.AddScoped<IPatrimoineService, PatrimoineService>();
builder.Services.AddScoped<IFluxCreditCoopService, FluxCreditCoopService>();

builder.Services.AddScoped<IYahooDataService, YahooDataService>();
builder.Services.AddScoped<ITradeRepublicDataService, TradeRepublicDataService>();
builder.Services.AddScoped<IPowensDataService, PowensDataService>();

builder.Services.AddScoped<PatrimoineViewModel>();
builder.Services.AddScoped<InvestissementViewModel>();
builder.Services.AddScoped<BudgetViewModel>();
builder.Services.AddScoped<ProfilViewModel>();

builder.Services.AddHostedService<PatrimoineWorker>();

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

List<string> ipAutorisees = ["109.14.14.134", "127.0.0.1", "194.167.154.181"];


app.UseHttpsRedirection();

//app.Use(async (context, next) =>
//{
//    var remoteIp = context.Connection.RemoteIpAddress?.ToString();

//    if (!ipAutorisees.Contains(remoteIp))
//    {
//        context.Response.StatusCode = 403; 
//        await context.Response.WriteAsync("Acces refuse : Votre IP n'est pas autorisee.");
//        return;
//    }

//    await next.Invoke();
//});

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRequestLocalization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
