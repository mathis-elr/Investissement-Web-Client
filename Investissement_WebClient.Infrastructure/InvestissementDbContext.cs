using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure;

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
            entity.HasOne(h => h.Categorie)
                .WithMany()
                .HasForeignKey(h => h.IdCategorie)
                .OnDelete(DeleteBehavior.Restrict);
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
                Libelle = "Avance Livret A"
            },
            new CategorieFlux
            {
                Id = 4,
                Libelle = "Dette Livret A"
            },
            new CategorieFlux
            {
                Id = 5,
                Libelle = "Shopping"
            },
            new CategorieFlux
            {
                Id = 6,
                Libelle = "Sport"
            },
            new CategorieFlux
            {
                Id = 7,
                Libelle = "Salaire"
            },
            new CategorieFlux
            {
                Id = 8,
                Libelle = "Aide"
            },
            new CategorieFlux
            {
                Id = 9,
                Libelle = "Autre"
            },
            new CategorieFlux
            {
                Id = 10,
                Libelle = "Investissement Trade Republic"
            },
            new CategorieFlux
            {
                Id = 11,
                Libelle = "Abonnement"
            },
            new CategorieFlux
            {
                Id = 12,
                Libelle = "Logement"
            }, new CategorieFlux
            {
                Id = 13,
                Libelle = "Cadeaux"
            }, new CategorieFlux
            {
                Id = 14,
                Libelle = "Achat plaisir"
            },
            new CategorieFlux
            {
                Id = 15,
                Libelle = "Investissement AV"
            },
            new CategorieFlux
            {
                Id = 16,
                Libelle = "Vacances"
            },
            new CategorieFlux
            {
                Id = 17,
                Libelle = "Santé"
            }
        );
    }
}