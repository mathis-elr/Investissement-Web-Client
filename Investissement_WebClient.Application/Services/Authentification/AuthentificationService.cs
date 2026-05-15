using Investissement_WebClient.Application.ViewsModels;
using Investissement_WebClient.Domain.Modeles;
using Investissement_WebClient.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Investissement_WebClient.Application.Services.Authentification
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IDbContextFactory<InvestissementDbContext> _dbFactory;

        public AuthentificationService(IDbContextFactory<InvestissementDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task Inscription(InscriptionVM infosInscription)
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            if(await dbContext.Utilisateur.FirstOrDefaultAsync(u => u.Email == infosInscription.Email) != null)
                throw new Exception("Un compte avec cette adresse e-mail existe déjà.");

            var newUser = new Utilisateur
            {
                Email = infosInscription.Email,
                Prenom = char.ToUpper(infosInscription.Prenom[0]) + infosInscription.Prenom.Substring(1).ToLower(),
                MdpHash = HashPassword(infosInscription.Mdp), 
                DateCreationCompte = DateTime.Now
            };

            dbContext.Utilisateur.Add(newUser);
            await dbContext.SaveChangesAsync();
        }

        public async Task Connexion(ConnexionVM infosConnexion)
        {
            using var dbContext = await _dbFactory.CreateDbContextAsync();
            var user = await dbContext.Utilisateur.FirstOrDefaultAsync(u => u.Email == infosConnexion.Email);
            if (user == null)
                throw new Exception("Adresse e-mail incorrect.");
            else if(!VerifyPassword(infosConnexion.Mdp, user.MdpHash))
                throw new Exception("Mot de passe incorrect.");

        }

        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<Utilisateur>();
            return hasher.HashPassword(null!, password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hasher = new PasswordHasher<Utilisateur>();
            return hasher.VerifyHashedPassword(null!, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
