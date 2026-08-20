using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Domain.Modeles
{
    public class Source
    {
        public int Id { get; set; }

        public string Nom { get; set; } = string.Empty;

        public TypeSource Type { get; set; }

        public TypeCompte TypeCompte { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; } = null!;
    }
}
