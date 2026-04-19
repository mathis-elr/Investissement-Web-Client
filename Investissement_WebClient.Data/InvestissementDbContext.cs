using Microsoft.EntityFrameworkCore;
using Investissement_WebClient.Core.Modeles;

namespace Investissement_WebClient.Data;

public class InvestissementDbContext : DbContext
{
    public InvestissementDbContext(DbContextOptions<InvestissementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transaction { get; set; }

    public DbSet<FluxTradeRepublic> FluxTradeRepublic { get; set; }

    public DbSet<CategorieFlux> CategorieFlux { get; set; }

    public DbSet<FluxCreditCoop> FluxCreditCoop { get; set; }

    public DbSet<HistoriquePatrimoine> HistoriquePatrimoine { get; set; }

    public DbSet<CreditCoopAcces> CreditCoopAcces { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(t => t.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.ISIN)
                .HasMaxLength(12)
                .IsFixedLength();

            entity.Property(e => e.Quantite)
                .HasPrecision(18, 6);
            entity.Property(e => e.Prix)
                .HasPrecision(18, 4);
            entity.Property(e => e.Total)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<FluxTradeRepublic>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.Montant)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<HistoriquePatrimoine>(entity =>
        {
            entity.Property(h => h.Id)
                .ValueGeneratedOnAdd();
            entity.Property(h => h.Date)
                .IsRequired();
            entity.Property(h => h.InvestissementTotal)
                .IsRequired();
            entity.Property(h => h.Valeur)
                .IsRequired();
        });

        modelBuilder.Entity<FluxCreditCoop>(entity =>
        {
            entity.Property(h => h.Id)
                .ValueGeneratedNever();
            entity.Property(h => h.Date)
                .IsRequired();
            entity.Property(h => h.Valeur)
                .IsRequired();
        });

        modelBuilder.Entity<CategorieFlux>().HasData(
            new CategorieFlux
            {
                Id = 1,
                Libelle = "Alimentation"
            },
            new CategorieFlux
            {
                Id = 2,
                Libelle = "Transport"
            },
            new CategorieFlux
            {
                Id = 3,
                Libelle = "Virement emis"
            },
            new CategorieFlux
            {
                Id = 4,
                Libelle = "Virement reçu"
            },
            new CategorieFlux
            {
                Id = 5,
                Libelle = "Virement emis livret A"
            },
            new CategorieFlux
            {
                Id = 6,
                Libelle = "Virement reçu livret A"
            },
            new CategorieFlux
            {
                Id = 7,
                Libelle = "Shopping"
            },
            new CategorieFlux
            {
                Id = 8,
                Libelle = "Sport"
            } ,
            new CategorieFlux
            {
                Id = 9,
                Libelle = "Salaire"
            },
            new CategorieFlux
            {
                Id = 10,
                Libelle = "APL"
            },
            new CategorieFlux
            {
                Id = 11,
                Libelle = "Autre"
            }
        );
    }
}