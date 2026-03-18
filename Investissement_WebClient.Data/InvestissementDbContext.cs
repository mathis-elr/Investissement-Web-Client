using Microsoft.EntityFrameworkCore;
using Investissement_WebClient.Core.Modeles;

namespace Investissement_WebClient.Data;

public class InvestissementDbContext : DbContext
{
    public InvestissementDbContext(DbContextOptions<InvestissementDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<CompositionModele> CompositionModeles { get; set; }
    
    public DbSet<Investissement> Investissements { get; set; }
    
    public DbSet<Modele> Modeles { get; set; }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    public DbSet<Actif> Actifs { get; set; }
    
    public DbSet<HistoriquePatrimoine>  HistoriquePatrimoine { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Actif>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Nom)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Symbole)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.Isin)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.HasIndex(a => a.Isin)
                .IsUnique();
            entity.Property(e => e.Risque)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.EstEnregistre)
                .IsRequired();
        });



        modelBuilder.Entity<Modele>(entity =>
        {
            entity.HasKey(m => m.Id);
            
            entity.Property(m => m.Id)
                .ValueGeneratedOnAdd();
            entity.Property(m => m.Nom)
                .IsRequired()
                .HasMaxLength(30);
        });
        
        modelBuilder.Entity<CompositionModele>(entity =>
            {
                entity.HasKey(cm => new {cm.IdActifEnregistre,cm.IdModele});
                
                entity.HasOne(cm => cm.Modele)
                    .WithMany(m => m.Composition)
                    .HasForeignKey(cm => cm.IdModele);

                entity.HasOne(cm => cm.Actif)
                    .WithMany(a => a.Composition)
                    .HasForeignKey(cm => cm.IdActifEnregistre)
                    .OnDelete(DeleteBehavior.Restrict);
            });


        modelBuilder.Entity<Investissement>(entity =>
        {
            entity.HasKey(i => i.Id);
            
            entity.Property(i => i.Id)
                .ValueGeneratedOnAdd();
            entity.Property(i => i.DateInvest)
                .IsRequired();
            
            entity.HasOne(i => i.Modele)
                .WithMany(m => m.Investissements)
                .HasForeignKey(i => i.IdModele)
                .OnDelete(DeleteBehavior.Restrict); 
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .ValueGeneratedOnAdd();
            entity.Property(t => t.Prix)
                .IsRequired();
            entity.Property(t => t.Quantite)
                .IsRequired();
            entity.Property(t => t.Frais);

            entity.HasOne(t => t.Investissement)
                .WithMany(i => i.Transactions)
                .HasForeignKey(t => t.IdInvestissement)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Actif)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.IdActifEnregistre)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HistoriquePatrimoine>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Id)
                .ValueGeneratedOnAdd();
            entity.Property(h => h.Date)
                .IsRequired();
            entity.Property(h => h.InvestissementTotal)
                .IsRequired();
            entity.Property(h => h.Valeur)
                .IsRequired();
        });
    }
}