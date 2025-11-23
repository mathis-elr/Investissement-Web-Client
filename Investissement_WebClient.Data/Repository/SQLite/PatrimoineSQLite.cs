using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    string query = "SELECT symbole FROM Actif;";
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

        public Dictionary<string, double> ReadQuantiteInvestitParActif()
        {
            Dictionary<string,double> quantiteParActif = new Dictionary<string, double>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT actif,SUM(quantite) FROM [Transaction] GROUP BY actif;";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        quantiteParActif[reader.GetString(0)] = reader.GetDouble(1);
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
                    string query = "SELECT DISTINCT Actif.nom, Actif.symbole FROM [Transaction] JOIN Actif ON Actif.nom = [Transaction].actif";
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

        //public double GetQuantiteTotaleActif(string nomActif)
        //{
        //    using (var connection = new SqliteConnection(_connexion))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            double quantiteTotale = 0;
        //            string query = "SELECT SUM(quantite) FROM [transaction] WHERE actif=@actif GROUP BY actif;";
        //            var command = new SqliteCommand(query, connection);
        //            command.Parameters.AddWithValue("@actif", nomActif);
        //            quantiteTotale = (double)command.ExecuteScalar();

        //            return quantiteTotale;
        //        }
        //        catch (SqliteException ex)
        //        {
        //            Debug.WriteLine($"Erreur selection quantite totale d'un actif : {ex.Message}");
        //            throw;
        //        }
        //    }
        //}
    }
}
