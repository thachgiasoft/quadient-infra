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
    /// Varsayılan data değişikliği kaydedici
    /// </summary>
    public class EmptyAudit : IAudit
    {

        public void SaveDataChanges(DataTable tableOldState, DataTable tableNewState)
        {

        }

        public void SaveExecutedQuery(DbCommand command)
        {

        }
    }
}
