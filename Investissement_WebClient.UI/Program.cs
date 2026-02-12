using Investissement_WebClient.Core.InterfacesServices;
using Investissement_WebClient.Data;
using Investissement_WebClient.Data.Background;
using Investissement_WebClient.Data.Services;
using Investissement_WebClient.UI.Components;
using Investissement_WebClient.UI.Components.ViewsModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<InvestissementDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("ConnectionStringUbuntu")));

// builder.Services.AddDbContextFactory<InvestissementDbContext>(options =>
//     options.UseSqlite(builder.Configuration.GetConnectionString("ConnectionStringWindows")));

builder.Services.AddScoped<ITransactionService, TransactionService>();
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
