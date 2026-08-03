using Investissement_WebClient.Infrastructure.Seeders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure;

public class InvestissementDbContext(DbContextOptions<InvestissementDbContext> options) : DbContext(options)
{
    public DbSet<Utilisateur> Utilisateur { get; set; }

    public DbSet<TradeRepublicAcces> TradeRepublicAcces { get; set; }

    public DbSet<BanqueAcces> BanqueAcces { get; set; }

    public DbSet<Actif> Actif { get; set; }
    public DbSet<FluxInvestissement> FluxInvestissement { get; set; }

    public DbSet<CategorieFlux> CategorieFlux { get; set; }
    public DbSet<FluxBancaire> FluxBancaire { get; set; }

    public DbSet<ValeurPatrimoine> ValeurPatrimoine { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurations tables
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvestissementDbContext).Assembly);

        // Seeders
        CategorieFluxSeeder.Seed(modelBuilder);
    }
}