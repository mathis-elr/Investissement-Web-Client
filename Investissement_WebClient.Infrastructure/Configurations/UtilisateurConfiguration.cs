using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Investissement_WebClient.Infrastructure.Configurations
{
    internal class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
    {
        public void Configure(EntityTypeBuilder<Utilisateur> builder)
        {
            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
