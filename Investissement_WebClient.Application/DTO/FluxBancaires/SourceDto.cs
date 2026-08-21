using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.DTO.FluxBancaires
{
    public class SourceDto
    {
        public int Id { get; set; }

        public string NomSource { get; set; } = string.Empty;

        public string NomCompte { get; set; } = string.Empty;

        public TypeCompte TypeCompte { get; set; }
    }
}
