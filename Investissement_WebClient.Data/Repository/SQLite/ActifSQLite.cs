using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Investissement_WebClient.Data.Repository.SQLite
{
    public class ActifSQLite : DataContext, IActifSQLite
    {
        public ActifSQLite(string connection)
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

        public void ajouterActif(ActifModele actif)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO Actif VALUES(@nom,@symbole,@type,@ISIN,@risque);";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@nom", actif.nom);
                    command.Parameters.AddWithValue("@symbole", actif.symbole);
                    command.Parameters.AddWithValue("@type", actif.type);
                    command.Parameters.AddWithValue("@ISIN", actif.isin == null ? DBNull.Value : actif.isin);
                    command.Parameters.AddWithValue("@risque", actif.risque);
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur lors de l'insertion d'un actif : {ex.Message}");
                    throw;
                }
            }
        }

        public void modifierActif(ActifModele actif)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE Actif SET symbole=@symbole, type=@type, isin=@ISIN, risque=@risque WHERE nom=@nom;";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@symbole", actif.symbole);
                    command.Parameters.AddWithValue("@type", actif.type);
                    command.Parameters.AddWithValue("@ISIN", actif.isin == null ? DBNull.Value : actif.isin);
                    command.Parameters.AddWithValue("@risque", actif.risque);
                    command.Parameters.AddWithValue("@nom", actif.nom);
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
                    string query = "DELETE FROM Actif WHERE nom=@nom;";
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
