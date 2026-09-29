namespace Investissement_WebClient.Domain.Enums
{
    public enum StatutConnexion
    {
        Valide = 0,               // Tout roule
        ReconnexionRequise = 1,   // Action utilisateur nécessaire (SCA, mot de passe changé, etc.)
        SiteIndisponible = 2,     // La banque est en maintenance côté Powens (attendre)
        ErreurTechnique = 3       // Bug / endpoint en panne
    }
}
