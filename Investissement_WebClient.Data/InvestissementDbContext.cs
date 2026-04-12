using Microsoft.EntityFrameworkCore;
using Investissement_WebClient.Core.Modeles;

namespace Investissement_WebClient.Data;

public class InvestissementDbContext : DbContext
{
    public InvestissementDbContext(DbContextOptions<InvestissementDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Transaction> Transactions { get; set; }
    
    public DbSet<FluxBancaire> FluxBancaires { get; set; }
    
    public DbSet<HistoriquePatrimoine>  HistoriquePatrimoine { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id)
                .ValueGeneratedNever();

            entity.Property(e => e.ISIN)
                .HasMaxLength(12)
                .IsFixedLength();
            
            entity.Property(e => e.Quantite)
                .HasPrecision(18, 8);
            entity.Property(e => e.Prix)
                .HasPrecision(18, 4);
            entity.Property(e => e.Total)
                .HasPrecision(18, 2);
        });
        
        modelBuilder.Entity<FluxBancaire>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id)
                .ValueGeneratedNever();
            
            entity.Property(e => e.Montant)
                .HasPrecision(18, 2);
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