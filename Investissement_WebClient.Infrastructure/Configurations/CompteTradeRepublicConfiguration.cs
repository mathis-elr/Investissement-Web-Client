using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class CompteTradeRepublicConfiguration : IEntityTypeConfiguration<CompteTradeRepublic>
    {
        public void Configure(EntityTypeBuilder<CompteTradeRepublic> builder)
        {
            builder.HasOne(c => c.Source)
                .WithOne()
                .HasForeignKey<CompteTradeRepublic>(c => c.SourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
