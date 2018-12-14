using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Auditing
{
    /// <summary>
    /// Veri degisikliklerini kaydeden siniflarin arayuzu
    /// </summary>
    public interface IAudit
    {
        /// <summary>
        /// Typed datasetler icin veri degisikliklerini loglayan metot.
        /// </summary>
        /// <param name="tableOldState"></param>
        /// <param name="tableNewState"></param>
        void SaveDataChanges(DataTable tableOldState, DataTable tableNewState);

        /// <summary>
        /// ExecuteNonQuery ve ExecuteScalar metotlari icin veri degisikliklerini loglayan metot.
        /// </summary>
        /// <param name="command">Sql komutu</param>
        void SaveExecutedQuery(DbCommand command);
    }
}
