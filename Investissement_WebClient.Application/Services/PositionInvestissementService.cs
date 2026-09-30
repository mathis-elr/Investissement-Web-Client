using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.Interfaces.APIs;
using Investissement_WebClient.Application.DTO;
using Investissement_WebClient.Domain.Modeles;

namespace Investissement_WebClient.Application.Services
{
    public class PositionInvestissementService(IPositionInvestissementRepository positionInvestissementRepository,
                                               IYahooFinanceApiService yahooFinanceApiService,
                                               IActifService actifService) : IPositionInvestissementService
    {
        public IPositionInvestissementRepository _positionInvestissementRepository = positionInvestissementRepository;
        public IYahooFinanceApiService _yahooFinanceApiService = yahooFinanceApiService;
        public IActifService _actifService = actifService;

        public async Task<IEnumerable<PositionInvestissementDto>> GetPositionsByCompte(int compteId)
        {
            var positions = await _positionInvestissementRepository.GetByCompteBanqueId(compteId);
            return positions.Select(p =>
                new PositionInvestissementDto
                {
                    Actif = p.Actif!,
                    Quantite = p.Quantite,
                    PrixAchat = p.PrixAchat,
                    PrixCourant = p.PrixCourant,
                    DateCours = p.DateCours,
                    DerniereMaj = p.DerniereMaj,
                    CompteBanque = p.CompteBanque!
                }).ToList();
        }

        public async Task MapperInvestissements(List<PositionInvestissementImportDto>? positions, int compteBanqueId)
        {
            if (positions == null || positions.Count == 0)
                return;

            var positionsExistantes = await _positionInvestissementRepository.GetByCompteBanqueId(compteBanqueId);
            var actifsLocaux = await _actifService.GetAll();

            var positionsAInserer = new List<PositionInvestissement>();

            foreach (var position in positions)
            {
                var actif = actifsLocaux.FirstOrDefault(a =>
                    (position.ISIN && a.ISIN == position.Code) ||
                    (!position.ISIN && (a.Ticker == position.Code || a.Libelle == position.Label)));

                if (actif == null)
                {
                    actif = await NouvelActif(position);
                    actifsLocaux.Add(actif);
                }

                var positionExistante = positionsExistantes.FirstOrDefault(p => p.ActifId == actif.Id)
                        ?? positionsAInserer.FirstOrDefault(p => p.ActifId == actif.Id);

                if (positionExistante == null)
                {
                    positionsAInserer.Add(new PositionInvestissement
                    {
                        ActifId = actif.Id,
                        Quantite = position.Quantite,
                        PrixAchat = position.PrixAchat,
                        PrixCourant = position.PrixCourant,
                        DateCours = position.DatePrixCourant,
                        DerniereMaj = DateTime.UtcNow,
                        CompteBanqueId = compteBanqueId,
                    });
                }
                else
                {
                    positionExistante.Quantite = position.Quantite;
                    positionExistante.PrixAchat = position.PrixAchat;
                    positionExistante.PrixCourant = position.PrixCourant;
                    positionExistante.DateCours = position.DatePrixCourant;
                    positionExistante.DerniereMaj = DateTime.UtcNow;
                }
            }

            if(positionsAInserer.Count > 0) await _positionInvestissementRepository.AddRange(positionsAInserer);

            await _positionInvestissementRepository.SaveChanges();
        }

        private async Task<Actif> NouvelActif(PositionInvestissementImportDto position)
        {
            var nouvelActif = new Actif
            {
                Libelle = _actifService.NettoyerLibelle(position.Label),
                ISIN = position.ISIN ? position.Code : null,
                Ticker = position.ISIN ? await _yahooFinanceApiService.GetTickerByIsinAsync(position.Code) : position.Code
            };

            var actifId = await _actifService.AddActif(nouvelActif);
            nouvelActif.Id = actifId;

            return nouvelActif;
        }
    }
}
