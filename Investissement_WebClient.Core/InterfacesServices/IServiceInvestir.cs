using Investissement_WebClient.Core.Modeles.DTO;

namespace Investissement_WebClient.Core.InterfacesServices;

public interface IServiceInvestir
{
    Task<IEnumerable<(int Id, string Nom)>> GetModeles();
    
    Task<IEnumerable<PreparationTransaction>> GetCompositionModele(int idModele);

    Task SaveInvestissement(int? idModele, DateTime dateInvest,List<PreparationTransaction> transactionsInvestissement);
}