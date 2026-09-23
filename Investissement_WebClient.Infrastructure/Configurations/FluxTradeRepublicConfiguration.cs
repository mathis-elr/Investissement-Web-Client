using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class FluxTradeRepublicConfiguration : IEntityTypeConfiguration<FluxTradeRepublic>
    {
        public void Configure(EntityTypeBuilder<FluxTradeRepublic> builder)
        {
            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.Quantite)
                .HasPrecision(18, 6);
            builder.Property(e => e.Prix)
                .HasPrecision(18, 4);
            builder.Property(e => e.Frais)
                .HasPrecision(18, 2);
            builder.Property(e => e.Total)
                .HasPrecision(18, 2);
        }
    }
}
