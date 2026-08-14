using Investissement_WebClient.Application.Interfaces.Repositories;
using Investissement_WebClient.Application.Interfaces.Services;
using Investissement_WebClient.Application.DTO.Auth;
using Investissement_WebClient.Domain.Modeles;
using Microsoft.AspNetCore.Identity;

namespace Investissement_WebClient.Application.Services
{
    public class AuthentificationService(IUtilisateurRepository utilisateurRepository) : IAuthentificationService
    {
        private readonly IUtilisateurRepository _utilisateurRepository = utilisateurRepository; 

        public async Task<int> Inscription(InscriptionDto infosInscription)
        {
            var utilisateur = await _utilisateurRepository.GetByEmail(infosInscription.Email);
            if (utilisateur != null)
                throw new Exception("Un compte avec cette adresse e-mail existe déjà.");

            var newUser = new Utilisateur
            {
                Email = infosInscription.Email,
                Prenom = char.ToUpper(infosInscription.Prenom[0]) + infosInscription.Prenom.Substring(1).ToLower(),
                MdpHash = HashPassword(infosInscription.Mdp), 
                DateCreationCompte = DateTime.Now
            };

            await _utilisateurRepository.Add(newUser);

            return newUser.Id;
        }

        public async Task<Utilisateur> Connexion(ConnexionDto infosConnexion)
        {
            var utilisateur = await _utilisateurRepository.GetByEmail(infosConnexion.Email);

            if (utilisateur == null)
                throw new Exception("Adresse e-mail incorrect.");
            else if(!VerifyPassword(infosConnexion.Mdp, utilisateur.MdpHash))
                throw new Exception("Mot de passe incorrect.");

            return utilisateur;
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
