using Investissement_WebClient.Application.DTO;

namespace Investissement_WebClient.Application.Interfaces.Services
{
    public interface IPositionInvestissementService
    {
        Task MapperInvestissements(List<PositionInvestissementImportDto>? positions, int compteBanqueId);
    }
}
