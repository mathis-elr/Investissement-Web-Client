using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class ActifConfiguration : IEntityTypeConfiguration<Actif>
    {
        public void Configure(EntityTypeBuilder<Actif> builder)
        {
            builder.Property(e => e.ISIN)
                .HasMaxLength(12)
                .IsFixedLength();
        }
    }
}
