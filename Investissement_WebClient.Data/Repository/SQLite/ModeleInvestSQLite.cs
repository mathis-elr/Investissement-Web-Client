using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Investissement_WebClient.Data.Repository.SQLite
{
    public class ModeleInvestSQLite : DataContext, IModeleInvestSQLite
    {
        public ModeleInvestSQLite(string connexion)
        {
            _connexion = connexion;
        }

        public List<string> ReadNomsActif()
        {
            List<string> nomsActifs = new List<string>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT nom FROM ActifEnregistre";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        nomsActifs.Add(reader.GetString(0));
                    }

                    return nomsActifs;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des noms des actifs : {ex.Message}");
                    throw;
                }
            }
        }

        public List<string> ReadModeles()
        {
            List<string> modeles = new List<string>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT DISTINCT modele FROM TransactionsModele ORDER BY modele;";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        modeles.Add(reader.GetString(0));
                    }

                    return modeles;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des noms des modeles : {ex.Message}");
                    throw;
                }
            }
        }

        public List<TransactionModele> ReadTransactionsModele(string modele)
        {
            List<TransactionModele> modeles = new List<TransactionModele>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM TransactionsModele WHERE modele=@modele;";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@modele", modele);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        modeles.Add(new TransactionModele(reader.GetString(1), reader.GetString(2), reader.IsDBNull(3) ? null : reader.GetDouble(3)));
                    }

                    return modeles;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur recuperation des transactions associé au modele {modele} : {ex.Message}");
                    throw;
                }
            }
        }

        public void AjouterModele(List<TransactionModele> transactionsModele)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                connection.Open();

                using (var dbTransaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO TransactionsModele (modele,actif,quantite) VALUES (@modele,@actif,@quantite);";
                        using (var command = new SqliteCommand(query, connection, dbTransaction))
                        {
                            command.Parameters.Add("@modele", SqliteType.Text);
                            command.Parameters.Add("@actif", SqliteType.Text);
                            command.Parameters.Add("@quantite", SqliteType.Real);

                            foreach (var transaction in transactionsModele)
                            {
                                command.Parameters["@modele"].Value = transaction.modele;
                                command.Parameters["@actif"].Value = transaction.actif;
                                command.Parameters["@quantite"].Value = transaction.quantite == null ? DBNull.Value : transaction.quantite;

                                command.ExecuteNonQuery();
                            }
                        }

                        dbTransaction.Commit();
                    }
                    catch (SqliteException ex)
                    {
                        // Si une erreur se produit, annuler toutes les insertions
                        dbTransaction.Rollback();
                        Debug.WriteLine($"Erreur insertion transaction modele : {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public void SupprimerModele(string modele)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM TransactionsModele WHERE modele=@modele;";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@modele", modele);
                    var reader = command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur de suppression du modele {modele} : {ex.Message}");
                    throw;
                }
            }
        }
    }
}
