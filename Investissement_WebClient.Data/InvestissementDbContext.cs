using Microsoft.EntityFrameworkCore;
using Investissement_WebClient.Core.Modeles;

namespace Investissement_WebClient.Data;

public class InvestissementDbContext : DbContext
{
    public InvestissementDbContext(DbContextOptions<InvestissementDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<ActifEnregistre> ActifEnregistres { get; set; }
    
    public DbSet<CompositionModele> CompositionModeles { get; set; }
    
    public DbSet<Investissement> Investissements { get; set; }
    
    public DbSet<Modele> Modeles { get; set; }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    public DbSet<Actif> Actifs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActifEnregistre>(entity =>
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
                    .WithMany(m => m.CompositionModeles)
                    .HasForeignKey(cm => cm.IdModele);
                entity.HasOne(cm => cm.ActifEnregistre)
                    .WithMany(a => a.CompositionModeles)
                    .HasForeignKey(cm => cm.IdActifEnregistre);
            });


        modelBuilder.Entity<Investissement>(entity =>
        {
            entity.HasKey(i => i.Id);
            
            entity.Property(i => i.Id)
                .ValueGeneratedOnAdd();
            entity.Property(i => i.dateInvest)
                .IsRequired();
            
            entity.HasOne(i => i.Modele)
                .WithMany(m => m.Investissements)
                .HasForeignKey(i => i.IdModele);
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
                .WithMany(t => t.Transactions)
                .HasForeignKey(t => t.IdInvestissement);
            entity.HasOne(t => t.ActifEnregistre)
                .WithMany()
                .HasForeignKey(t => t.IdActifEnregistre);
        });

        modelBuilder.Entity<Actif>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(a => a.Type)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(a => a.Isin)
                    .HasMaxLength(12);
                entity.Property(a => a.Symbole)
                    .IsRequired()
                    .HasMaxLength(20);
            });
    }
}