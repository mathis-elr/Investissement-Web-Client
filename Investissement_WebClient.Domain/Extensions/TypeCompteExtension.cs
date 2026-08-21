using Investissement_WebClient.Domain.Enums;

namespace Investissement_WebClient.Domain.Extensions;

public static class TypeCompteExtensions
{
    public static TypeCompte ToTypeCompte(string? typePowens)
    {
        return typePowens?.ToLowerInvariant() switch
        {
            "checking" => TypeCompte.Courant,
            "joint" => TypeCompte.Courant,
            "card" => TypeCompte.Courant,

            "savings" => TypeCompte.Epargne,
            "deposit" => TypeCompte.Epargne,
            "ldds" => TypeCompte.Epargne,
            "pel" => TypeCompte.Epargne,
            "cel" => TypeCompte.Epargne,
            "csl" => TypeCompte.Epargne,
            "cat" => TypeCompte.Epargne,
            "livret_a" => TypeCompte.Epargne,
            "livret_b" => TypeCompte.Epargne,

            "market" => TypeCompte.Investissement,
            "pea" => TypeCompte.Investissement,
            "lifeinsurance" => TypeCompte.Investissement,
            "pee" => TypeCompte.Investissement,
            "perco" => TypeCompte.Investissement,
            "per" => TypeCompte.Investissement,
            "perp" => TypeCompte.Investissement,
            "capitalisation" => TypeCompte.Investissement,
            "crowdlending" => TypeCompte.Investissement,
            "real_estate" => TypeCompte.Investissement,
            "article83" => TypeCompte.Investissement,
            "rsp" => TypeCompte.Investissement,
            "madelin" => TypeCompte.Investissement,

            "loan" => TypeCompte.Credit,
            "mortgage" => TypeCompte.Credit,
            "consumercredit" => TypeCompte.Credit,
            "revolvingcredit" => TypeCompte.Credit,

            _ => TypeCompte.Autre
        };
    }
}