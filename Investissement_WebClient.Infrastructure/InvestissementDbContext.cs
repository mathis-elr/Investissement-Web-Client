using Investissement_WebClient.Infrastructure.Seeders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure;

public class InvestissementDbContext(DbContextOptions<InvestissementDbContext> options) : DbContext(options)
{
    public DbSet<Utilisateur> Utilisateur { get; set; }

    public DbSet<CompteTradeRepublic> TradeRepublicAcces { get; set; }

    public DbSet<Source> Source { get; set; }
    public DbSet<UtilisateurPowens> UtilisateurPowens { get; set; }
    public DbSet<Banque> Banque { get; set; }
    public DbSet<CompteBanque> CompteBanque { get; set; }

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