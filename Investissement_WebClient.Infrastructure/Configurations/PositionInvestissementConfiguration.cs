using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class PositionInvestissementConfiguration : IEntityTypeConfiguration<PositionInvestissement>
    {
        public void Configure(EntityTypeBuilder<PositionInvestissement> builder)
        {
            builder.Property(e => e.Quantite)
                .HasPrecision(18, 6);

            builder.Property(e => e.PrixAchat)
                .HasPrecision(18, 4);

            builder.Property(e => e.PrixCourant)
                .HasPrecision(18, 4);
        }
    }
}
