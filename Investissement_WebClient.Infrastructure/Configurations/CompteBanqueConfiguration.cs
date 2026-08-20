using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class CompteBanqueConfiguration : IEntityTypeConfiguration<CompteBanque>
    {
        public void Configure(EntityTypeBuilder<CompteBanque> builder)
        {
            builder.Property(c => c.Solde)
                .HasPrecision(18, 2);
        }
    }
}
