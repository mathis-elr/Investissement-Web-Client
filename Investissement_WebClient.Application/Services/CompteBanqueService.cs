using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.FluxBancaires;
using Investissement_WebClient.Application.Interfaces.APIs;

namespace Investissement_WebClient.Application.Services
{
    public class CompteBanqueService(ICompteBanqueRepository compteBanqueRepository,
                                     ILogoDevApiService logoDevApiService) : ICompteBanqueService
    {
        private readonly ICompteBanqueRepository _compteBanqueRepository = compteBanqueRepository;
        private readonly ILogoDevApiService _logoDevApiService = logoDevApiService;

        public async Task<List<SourceDto>> GetAllByUserId(int userId)
        {
            var comptes = await _compteBanqueRepository.GetAllByUserId(userId);
            return comptes.Select(c => new SourceDto
            {
                Id = c.Id,
                NomSource = c.Banque.Nom,
                NomCompte = c.Nom,
                TypeCompte = c.TypeCompte,
                LogoUrl = _logoDevApiService.GetUrlLogoByName(c.Banque.Nom)
            }).ToList();
        }
    }
}
