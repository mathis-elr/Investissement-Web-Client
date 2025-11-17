using Investissement_WebClient.Data.Modeles;
using Investissement_WebClient.Data.Repository.Interfaces;
using Microsoft.Data.Sqlite;
using System.Diagnostics;

namespace Investissement_WebClient.Data.Repository.SQLite
{
    public class InvestirSQLite : DataContext, IInvestirSQLite
    {
        public InvestirSQLite(string connection)
        {
            _connexion = connection;
        }

        public List<ModeleInvest> ReadNomModeles()
        {
            List<ModeleInvest> modeles = new List<ModeleInvest>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM ModeleInvest";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        modeles.Add(new ModeleInvest(reader.GetInt64(0), reader.GetString(1)));
                    }

                    return modeles;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur de connexion : {ex.Message}");
                    throw;
                }
            }

        }

        public List<string> ReadNomActifs()
        {
            List<string> nomActifs = new List<string>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT nom FROM Actif";
                    var command = new SqliteCommand(query, connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        nomActifs.Add(reader.GetString(0));
                    }

                    return nomActifs;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur de connexion : {ex.Message}");
                    throw;
                }
            }
        }

        public List<TransactionModele> ReadTransactionsModele(long idModele)
        {
            List<TransactionModele> transactionsModele = new List<TransactionModele>();
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM TransactionsModele WHERE idModele=@id";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@id", idModele);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        transactionsModele.Add(new TransactionModele(
                            reader.GetInt64(0),
                            reader.GetString(1),
                            reader.IsDBNull(2) ? (long?)null : reader.GetInt64(2),
                            reader.GetInt64(3)));
                    }

                    return transactionsModele;
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur de connexion : {ex.Message}");
                    throw;
                }
            }
        }

        public void AddInvest(List<Transaction> transactions)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                connection.Open();

                // Démarrer une transaction SQLite pour le lot (crucial pour la performance)
                using (var dbTransaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO [Transaction] (date,actif,quantite,prix) VALUES (@date,@actif,@quantite,@prix);";
                        // Réutiliser l'objet de commande dans la boucle
                        using (var command = new SqliteCommand(query, connection, dbTransaction))
                        {
                            // Ajouter les paramètres une seule fois pour la réutilisation
                            command.Parameters.Add("@date", SqliteType.Text);
                            command.Parameters.Add("@actif", SqliteType.Text);
                            command.Parameters.Add("@quantite", SqliteType.Real); // Type approprié pour double?
                            command.Parameters.Add("@prix", SqliteType.Real);    

                            foreach (var transaction in transactions)
                            {
                                // Mettre à jour les valeurs des paramètres dans la boucle
                                command.Parameters["@date"].Value = transaction.date.ToString("yyyy-MM-dd");
                                command.Parameters["@actif"].Value = transaction.actif ?? (object)DBNull.Value;
                                command.Parameters["@quantite"].Value = transaction.quantite.HasValue ? (object)transaction.quantite.Value : (object)DBNull.Value;
                                command.Parameters["@prix"].Value = transaction.prix.HasValue ? (object)transaction.prix.Value : (object)DBNull.Value;

                                command.ExecuteNonQuery();
                            }
                        }

                        // Si tout s'est bien passé, valider (commettre) toutes les insertions
                        dbTransaction.Commit();
                    }
                    catch (SqliteException ex)
                    {
                        // Si une erreur se produit, annuler toutes les insertions
                        dbTransaction.Rollback();
                        Debug.WriteLine($"Erreur insertion transaction SQLite : {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public double getQuantiteTotalePrecedente(DateTime date)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT quantiteEUR FROM InvestissementTotal WHERE date < @date ORDER BY date DESC LIMIT 1;";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    var res = command.ExecuteScalar();
                    if (res == null || res == DBNull.Value)
                    {
                        return Convert.ToInt64(0);
                    }
                    return Convert.ToInt64(res);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, "erreur recuperation quantite totale investit precedent une date donné");
                    throw;
                }
            }
        }

        public double getQuantiteTotaleSuivante(DateTime date)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT quantiteEUR FROM InvestissementTotal WHERE date > @date ORDER BY date DESC LIMIT 1;";
                    var command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    var res = command.ExecuteScalar();
                    if (res == null || res == DBNull.Value)
                    {
                        return Convert.ToInt64(0);
                    }
                    return Convert.ToInt64(res);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, "erreur recuperation quantite totale investit suivant une date donné");
                    throw;
                }
            }
        }

        public DateTime? getDateDernierInvest()
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT date FROM InvestissementTotal ORDER BY date DESC LIMIT 1;";
                    var command = new SqliteCommand(query, connection);
                    var res = command.ExecuteScalar();
                    if (res == null || res == DBNull.Value)
                    {
                        return null;
                    }
                    DateTime dateDernierInvest = Convert.ToDateTime(res);
                    return dateDernierInvest;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, "erreur recuperation date dernier invest");
                    throw;
                }
            }
        }

        public DateTime getDatePremierInvest()
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT date FROM InvestissementTotal ORDER BY date ASC LIMIT 1;";
                    var command = new SqliteCommand(query, connection);
                    var res = command.ExecuteScalar();
                    DateTime dateDernierInvest = Convert.ToDateTime(res);
                    return dateDernierInvest;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, "erreur recuperation date dernier invest");
                    throw;
                }
            }
        }

        public void ajouterInvestissementTotal(DateTime date, double quantiteInvestit)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO InvestissementTotal (date,quantiteEUR) VALUES (@date,@sommeQuantiteTotale);";
                    var command = new SqliteCommand(query, connection);
                    double sommeQuantiteTotal = this.getQuantiteTotalePrecedente(date) + quantiteInvestit;
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@sommeQuantiteTotale", sommeQuantiteTotal);
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur insertion InvestissementTotal SQLite : {ex.Message}");
                    throw;
                }
            }
        }

        public void modifierInvestissementTotal(DateTime date, double quantiteAjoute)
        {
            using (var connection = new SqliteConnection(_connexion))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE InvestissementTotal SET quantiteEUR = quantiteEUR + @quantiteAjoute WHERE date=@date;";
                    var command = new SqliteCommand(query, connection);
                    
                    command.Parameters.AddWithValue("@quantiteAjoute", quantiteAjoute);
                    command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex)
                {
                    Debug.WriteLine($"Erreur modification InvestissementTotal SQLite : {ex.Message}");
                    throw;
                }
            }
        }

    }
}
