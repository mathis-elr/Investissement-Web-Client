using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Application.Services
{
    public class CompteTradeRepubliqueService(ITradeRepublicAccesRepository tradeRepublicAccesRepository,
                                              ILogoDevApiService logoDevApiService) : ICompteTradeRepubliqueService
    {
        private readonly ITradeRepublicAccesRepository _tradeRepublicAccesRepository = tradeRepublicAccesRepository;
        private readonly ILogoDevApiService _logoDevApiService = logoDevApiService;

        public async Task<SourceDto?> GetByUserId(int userId)
        {
            var compte = await _tradeRepublicAccesRepository.GetByUserId(userId);
            if(compte == null)
                return null;

            return new SourceDto
            {
                Id = -1,
                NomSource = "Trade Republic",
                NomCompte = "Portefeuille",
                TypeCompte = TypeCompte.Investissement,
                LogoUrl = _logoDevApiService.GetUrlLogoByName("Trade Republic")
            };
        }
    }
}
