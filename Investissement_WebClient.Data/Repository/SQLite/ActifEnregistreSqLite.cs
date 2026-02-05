using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Investissement_WebClient.Data.Repository.SQLite
{
    public class ActifEnregistreSqLite : DataContext, IActifEnregistreSQLite
    {
        public ActifEnregistreSqLite(string connection)
        {
            _connexion = connection;
        }

        public List<ActifModele> ReadActifs()
        {
            List<ActifModele> actifs = new List<ActifModele>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Actif";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        actifs.Add(new ActifModele(reader.GetString(0),
                            reader.GetString(1),
                            reader.IsDBNull(2) ? null : reader.GetString(2),
                            reader.GetString(3)
                            ));
                    }

                    return actifs;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des actifs : {ex.Message}");
                    throw;
                }
            }
        }

        public List<ActifEnresgistreModele> ReadActifsEnregistre()
        {
            List<ActifEnresgistreModele> actifs = new List<ActifEnresgistreModele>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM ActifEnregistre";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        actifs.Add(new ActifEnresgistreModele(reader.GetString(0),
                                             reader.GetString(1),
                                             reader.GetString(2),
                                             reader.IsDBNull(3) ? null : reader.GetString(3),
                                             reader.GetString(4)));
                    }

                    return actifs;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des actifs : {ex.Message}");
                    throw;
                }
            }
        }

        public void ajouterActif(ActifEnresgistreModele actifEnresgistre)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO ActifEnregistre VALUES(@nom,@symbole,@type,@ISIN,@risque);";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@nom", actifEnresgistre.nom);
                    command.Parameters.AddWithValue("@symbole", actifEnresgistre.symbole);
                    command.Parameters.AddWithValue("@type", actifEnresgistre.type);
                    command.Parameters.AddWithValue("@ISIN", actifEnresgistre.isin == null ? DBNull.Value : actifEnresgistre.isin);
                    command.Parameters.AddWithValue("@risque", actifEnresgistre.risque);
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur lors de l'insertion d'un actif : {ex.Message}");
                    throw;
                }
            }
        }

        public void modifierActif(ActifEnresgistreModele actifEnresgistre)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE ActifEnregistre SET symbole=@symbole, type=@type, isin=@ISIN, risque=@risque WHERE nom=@nom;";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@symbole", actifEnresgistre.symbole);
                    command.Parameters.AddWithValue("@type", actifEnresgistre.type);
                    command.Parameters.AddWithValue("@ISIN", actifEnresgistre.isin == null ? DBNull.Value : actifEnresgistre.isin);
                    command.Parameters.AddWithValue("@risque", actifEnresgistre.risque);
                    command.Parameters.AddWithValue("@nom", actifEnresgistre.nom);
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur lors de la modif d'un actif : {ex.Message}");
                    throw;
                }
            }
        }

        public void supprActif(string nom)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM ActifEnregistre WHERE nom=@nom;";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@nom", nom);
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur lors de la suppresion de l'actif {nom} : {ex.Message}");
                    throw;
                }
            }
        }
    }
}
