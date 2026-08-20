using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class SourceConfiguration : IEntityTypeConfiguration<Source>
    {
        public void Configure(EntityTypeBuilder<Source> builder)
        {
            builder.HasOne(x => x.Utilisateur)
                .WithMany()
                .HasForeignKey(x => x.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
