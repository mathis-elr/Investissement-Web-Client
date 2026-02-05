using Investissement_WebClient.Data.Repository.Interfaces;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Investissement_WebClient.Data.Repository.SQLite
{
    public class PatrimoineSQLite : DataContext,IPatrimoineSQLite
    {
        public PatrimoineSQLite(string connexion)
        {
            _connexion = connexion;
        }

        public List<string> GetSymboles()
        {
            List<string> symboles = new List<string>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT symbole FROM ActifEnregistre;";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        symboles.Add(reader.GetString(0));
                    }

                    return symboles;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des quantite par actif : {ex.Message}");
                    throw;
                }
            }
        }

        public Dictionary<string,(double,double)> GetQuantiteInvestitParActif()
        {
            Dictionary<string, (double, double)> quantiteParActif = new Dictionary<string, (double, double)>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT actif,SUM(quantite),SUM(quantite*prix) FROM [Transaction] GROUP BY actif;";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        quantiteParActif[reader.GetString(0)] = (reader.GetDouble(1),reader.GetDouble(2));
                    }

                    return quantiteParActif;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des quantite par actif : {ex.Message}");
                    throw;
                }
            }
        }

        public Dictionary<string, string> GetSymboleParActif()
        {

            Dictionary<string, string> symboleParActif = new Dictionary<string, string>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT DISTINCT ActifEnregistre.nom, ActifEnregistre.symbole FROM [Transaction] JOIN ActifEnregistre ON ActifEnregistre.nom = [Transaction].actif";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        symboleParActif[reader.GetString(0)] = reader.GetString(1);
                    }

                    return symboleParActif;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des quantite par actif : {ex.Message}");
                    throw;
                }
            }
        }

        public Dictionary<DateTime, double> GetQuantiteInvestitParDate()
        {
            Dictionary<DateTime, double> quantiteParDate = new Dictionary<DateTime, double>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT date,quantiteEUR FROM InvestissementTotal ORDER BY date;";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        quantiteParDate[reader.GetDateTime(0)] = reader.GetDouble(1);
                    }

                    return quantiteParDate;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des quantite investit par date : {ex.Message}");
                    throw;
                }
            }
        }

        public Dictionary<DateTime, double> GetValeurPatrimoineParDate()
        {
            Dictionary<DateTime, double> valeurParDate = new Dictionary<DateTime, double>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT date,valeurEUR FROM ValeurPatrimoine ORDER BY date;";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        valeurParDate[reader.GetDateTime(0)] = reader.GetDouble(1);
                    }

                    return valeurParDate;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des quantite investit par date : {ex.Message}");
                    throw;
                }
            }
        }
    }
}
