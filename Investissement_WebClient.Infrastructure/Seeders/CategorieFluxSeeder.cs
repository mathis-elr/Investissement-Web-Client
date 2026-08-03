using Investissement_WebClient.Domain.Modeles;
using Microsoft.EntityFrameworkCore;

namespace Investissement_WebClient.Infrastructure.Seeders
{
    public class CategorieFluxSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategorieFlux>().HasData(
                new CategorieFlux
                {
                    Id = 1,
                    MacroCategorie = "Vie quotidienne",
                    MicroCategorie = "Alimentation"
                },
                new CategorieFlux
                {
                    Id = 2,
                    MacroCategorie = "Vie quotidienne",
                    MicroCategorie = "Transport"
                },
                new CategorieFlux
                {
                    Id = 3,
                    MacroCategorie = "Vie quotidienne",
                    MicroCategorie = "Achat de nécéssité"
                },
                new CategorieFlux
                {
                    Id = 4,
                    MacroCategorie = "Vie quotidienne",
                    MicroCategorie = "Sport"
                },
                new CategorieFlux
                {
                    Id = 5,
                    MacroCategorie = "Vie quotidienne",
                    MicroCategorie = "Abonnement fixe"
                },
                new CategorieFlux
                {
                    Id = 6,
                    MacroCategorie = "Vie quotidienne",
                    MicroCategorie = "Logement"
                },
                new CategorieFlux
                {
                    Id = 7,
                    MacroCategorie = "Vie quotidienne",
                    MicroCategorie = "Santé"
                },
                new CategorieFlux
                {
                    Id = 8,
                    MacroCategorie = "Revenus",
                    MicroCategorie = "Salaire"
                },
                new CategorieFlux
                {
                    Id = 9,
                    MacroCategorie = "Revenus",
                    MicroCategorie = "Aide"
                },
                new CategorieFlux
                {
                    Id = 10,
                    MacroCategorie = "Revenus",
                    MicroCategorie = "Cadeau reçu"
                },
                new CategorieFlux
                {
                    Id = 11,
                    MacroCategorie = "Patrimoine",
                    MicroCategorie = "Investissement TR"
                },
                new CategorieFlux
                {
                    Id = 12,
                    MacroCategorie = "Patrimoine",
                    MicroCategorie = "Investissement AV"
                },
                new CategorieFlux
                {
                    Id = 13,
                    MacroCategorie = "Patrimoine",
                    MicroCategorie = "Epargne"
                },
                new CategorieFlux
                {
                    Id = 14,
                    MacroCategorie = "Loisirs/Plaisirs",
                    MicroCategorie = "Achat plaisir"
                },
                new CategorieFlux
                {
                    Id = 15,
                    MacroCategorie = "Loisirs/Plaisirs",
                    MicroCategorie = "Vacances"
                },

                new CategorieFlux
                {
                    Id = 16,
                    MacroCategorie = "Loisirs/Plaisirs",
                    MicroCategorie = "Abonnement plaisir"
                },

                new CategorieFlux
                {
                    Id = 17,
                    MacroCategorie = "Loisirs/Plaisirs",
                    MicroCategorie = "Achat cadeau"
                },
                new CategorieFlux
                {
                    Id = 18,
                    MicroCategorie = "Autre"
                },
                new CategorieFlux
                {
                    Id = 19,
                    MacroCategorie = "Livret A",
                    MicroCategorie = "Livret A"
                }
            );
        }
    }
}
