using ApexCharts;
using Blazored.Toast;
using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Data;
using Investissement_WebClient.Data.Background;
using Investissement_WebClient.Data.Services;
using Investissement_WebClient.UI.Components;
using Investissement_WebClient.UI.Components.ViewsModels;
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

//builder.Services.AddDbContextFactory<InvestissementDbContext>(options => 
//    options.UseSqlite(builder.Configuration.GetConnectionString("ConnectionStringUbuntu")));

//builder.Services.AddDbContextFactory<InvestissementDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("ConnectionStringWindows")));

builder.Services.AddDbContextFactory<InvestissementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));


builder.Services.AddScoped<IInvestirService, InvestirService>();
builder.Services.AddScoped<IActifService, ActifService>();
builder.Services.AddScoped<IModeleService, ModeleService>();
builder.Services.AddScoped<IPatrimoineService, PatrimoineService>();
builder.Services.AddScoped<IYahooDataService, YahooDataService>();

builder.Services.AddScoped<InvestirViewModel>();
builder.Services.AddScoped<ActifViewModel>();
builder.Services.AddScoped<ModeleViewModel>();
builder.Services.AddScoped<PatrimoineViewModel>();
builder.Services.AddScoped<BourseViewModel>();

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

app.Use(async (context, next) =>
{
    var remoteIp = context.Connection.RemoteIpAddress?.ToString();

    if (!ipAutorisees.Contains(remoteIp))
    {
        context.Response.StatusCode = 403; 
        await context.Response.WriteAsync("Acces refuse : Votre IP n'est pas autorisee.");
        return;
    }

    await next.Invoke();
});

app.UseStaticFiles();
app.UseAntiforgery();

app.UseRequestLocalization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
