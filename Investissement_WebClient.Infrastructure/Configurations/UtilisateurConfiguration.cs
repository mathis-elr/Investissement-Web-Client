using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
    {
        public void Configure(EntityTypeBuilder<Utilisateur> builder)
        {
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasOne(u => u.UtilisateurPowens)
                .WithOne(up => up.Utilisateur)
                .HasForeignKey<UtilisateurPowens>(up => up.UtilisateurId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
