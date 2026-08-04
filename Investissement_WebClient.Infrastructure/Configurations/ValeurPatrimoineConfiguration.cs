using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class ValeurPatrimoineConfiguration : IEntityTypeConfiguration<ValeurPatrimoine>
    {
        public void Configure(EntityTypeBuilder<ValeurPatrimoine> builder)
        {
            builder.Property(h => h.Id)
                .ValueGeneratedOnAdd();

            builder.Property(h => h.Date)
                .IsRequired();
            builder.Property(h => h.InvestissementTotal)
                .HasPrecision(18, 2)
                .IsRequired();
            builder.Property(h => h.Valeur)
                .HasPrecision(18, 2)
                .IsRequired();
        }
    }
}
