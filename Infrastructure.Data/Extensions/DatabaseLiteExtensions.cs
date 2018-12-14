using System.Data;
using System.Data.Common;

namespace Infrastructure.Data.Extensions
{
    public static class DatabaseLiteExtensions
    {
        /// <summary>
        /// İlgili dataset update islemini mevcut bir transaction varsa ona dahil eder.
        /// </summary>
        /// <param name="database">Database instance</param>
        /// <param name="dataset">Update edilecek dataset</param>
        /// <returns>Etkilenen kayit sayisini dondurur.</returns>
        public static int UpdateDatasetWithCurrentTransaction(this IDatabaseLite database, DataSet dataset)
        {
            if (database.TransactionActive)
            {
                return database.UpdateDataSet(dataset, database.CurrentTransaction);
            }

            return database.UpdateDataSet(dataset);
        }

        /// <summary>
        /// Ilgili cmd executenonquery islemini mevcut bir transaction varsa ona dahil eder. Yoksa tek basina execute eder.
        /// </summary>
        /// <param name="database">Database instance i</param>
        /// <param name="command">Execute edilecek komut</param>
        /// <returns>Etkilenen kayit sayisini dondurur.</returns>
        public static int ExecuteNonQueryWithCurrentTransaction(this IDatabaseLite database, DbCommand command)
        {
            if (database.TransactionActive)
            {
                return database.ExecuteNonQuery(command, database.CurrentTransaction);
            }

            return database.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Ilgili cmdexecute scalar islemini mevcut bir transaction varsa ona dahil eder. Yoksa tek basina execute eder.
        /// </summary>
        /// <param name="database">Database instance i</param>
        /// <param name="command">Execute edilecek komut</param>
        /// <returns>Execute sonucunu dondurur.</returns>
        public static object ExecuteScalarWithCurrentTransaction(this IDatabaseLite database, DbCommand command)
        {
            if (database.TransactionActive)
            {
                return database.ExecuteScalar(command, database.CurrentTransaction);
            }

            return database.ExecuteScalar(command);
        }
    }
}
