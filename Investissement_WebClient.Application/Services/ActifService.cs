using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Services
{
    public class ActifService(IActifRepository actifRepository) : IActifService
    {
        private readonly IActifRepository _actifRepository = actifRepository;

        private readonly List<string> _motsInutiles = ["EUR", "(ACC)", "PEA", "SWAP", "(DIST)","ESG"];

        public async Task<List<Actif>> GetAll()
        {
            return await _actifRepository.GetAll();
        }

        public async Task<IEnumerable<string>> GetTickers()
        {
            return await _actifRepository.GetAllTickers();
        }

        public string NettoyerLibelle(string libelle)
        {
            if (string.IsNullOrWhiteSpace(libelle))
                return string.Empty;

            var motsNettoyes = libelle.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(mot => !_motsInutiles.Contains(mot, StringComparer.OrdinalIgnoreCase));

            string resultat = string.Join(" ", motsNettoyes);

            return resultat.Trim();
        }

        public async Task<int> AddActif(Actif actif)
        {
            return await _actifRepository.Add(actif);
        }
    }
}
