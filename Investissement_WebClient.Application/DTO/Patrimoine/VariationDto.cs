using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.DTO.Patrimoine;

public class VariationDto
{
    public required Periode Periode { get; set; }

    public decimal VariationPourcentage { get; set; }

    public decimal VariationValeure { get; set; }
}