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
            entity.HasIndex(a => a.ISIN)
                .IsUnique();
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