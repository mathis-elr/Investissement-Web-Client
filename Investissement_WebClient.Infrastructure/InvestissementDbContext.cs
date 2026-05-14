using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure;

public class InvestissementDbContext : DbContext
{
    public InvestissementDbContext(DbContextOptions<InvestissementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Actif> Actif { get; set; }

    public DbSet<FluxInvestissement> FluxInvestissement { get; set; }

    public DbSet<CategorieFlux> CategorieFlux { get; set; }

    public DbSet<FluxBancaire> FluxBancaire { get; set; }

    public DbSet<ValeurPatrimoine> ValeurPatrimoine { get; set; }

    public DbSet<BanqueAcces> BanqueAcces { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Actif>(entity =>
        {
            entity.Property(e => e.ISIN)
                .HasMaxLength(12)
                .IsFixedLength();
        });

        modelBuilder.Entity<FluxInvestissement>(entity =>
        {
            entity.Property(t => t.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.Quantite)
                .HasPrecision(18, 6);
            entity.Property(e => e.Prix)
                .HasPrecision(18, 4);
            entity.Property(e => e.Total)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<ValeurPatrimoine>(entity =>
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

        modelBuilder.Entity<FluxBancaire>(entity =>
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
                MacroCategorie = "Vie quotidienne",
                MicroCategorie = "Alimentation"
            },
            new CategorieFlux
            {
                Id = 2,
                MacroCategorie = "Vie quotidienne",
                MicroCategorie = "Transport"
            },
            new CategorieFlux
            {
                Id = 4,
                MicroCategorie = "Livret A"
            },
            new CategorieFlux
            {
                Id = 5,
                MacroCategorie = "Vie quotidienne",
                MicroCategorie = "Achat de nécéssité"
            },
            new CategorieFlux
            {
                Id = 6,
                MacroCategorie = "Vie quotidienne",
                MicroCategorie = "Sport"
            },
            new CategorieFlux
            {
                Id = 7,
                MacroCategorie = "Revenus",
                MicroCategorie = "Salaire"
            },
            new CategorieFlux
            {
                Id = 8,
                MacroCategorie = "Revenus",
                MicroCategorie = "Aide"
            },
            new CategorieFlux
            {
                Id = 9,
                MacroCategorie = "Autre",
                MicroCategorie = "Autre"
            },
            new CategorieFlux
            {
                Id = 10,
                MacroCategorie = "Patrimoine",
                MicroCategorie = "Investissement TR"
            },
            new CategorieFlux
            {
                Id = 11,
                MacroCategorie = "Vie quotidienne",
                MicroCategorie = "Abonnement fixe"
            },
            new CategorieFlux
            {
                Id = 12,
                MacroCategorie = "Vie quotidienne",
                MicroCategorie = "Logement"
            },
            new CategorieFlux
            {
                Id = 13,
                MacroCategorie = "Revenus",
                MicroCategorie = "Cadeau reçu"
            },
            new CategorieFlux
            {
                Id = 14,
                MacroCategorie = "Loisirs/Plaisirs",
                MicroCategorie = "Achat plaisir"
            },
            new CategorieFlux
            {
                Id = 15,
                MacroCategorie = "Patrimoine",
                MicroCategorie = "Investissement AV"
            },
            new CategorieFlux
            {
                Id = 16,
                MacroCategorie = "Loisirs/Plaisirs",
                MicroCategorie = "Vacances"
            },
            new CategorieFlux
            {
                Id = 17,
                MacroCategorie = "Vie quotidienne",
                MicroCategorie = "Santé"
            },
            new CategorieFlux
            {
                Id = 18,
                MacroCategorie = "Loisirs/Plaisirs",
                MicroCategorie = "Abonnement plaisir"
            },
            new CategorieFlux
            {
                Id = 19,
                MacroCategorie = "Patrimoine",
                MicroCategorie = "Epargne"
            },
            new CategorieFlux
            {
                Id = 20,
                MacroCategorie = "Loisirs/Plaisirs",
                MicroCategorie = "Achat cadeau"
            }
        );
    }
}