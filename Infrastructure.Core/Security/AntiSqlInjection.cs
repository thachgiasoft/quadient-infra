using System.Text.RegularExpressions;
using System.Web.Management;

namespace Infrastructure.Core.Security
{
    public class AntiSqlInjection
    {
        private const string DeleteAttack = @"(?i)[\s]*;[\s]*delete|(?i)[\s]*;[\s]*delete";
        private const string DropTableAttack = @"(?i)'[\s]*;[\s]*drop[\s]*table|(?i)[\s]*;[\s]*drop[\s]*table";
        private const string InsertAttack = @"(?i)'[\s]*;[\s]*insert|(?i)[\s]*;[\s]*insert";
        private const string ShutdownAttack = @"(?i)'[\s]*;[\s]*shutdown[\s]*with[\s]*nowait|(?i)[\s]*;[\s]*shutdown[\s]*with[\s]*nowait";
        private const string UpdateAttack = @"(?i)'[\s]*;[\s]*update|(?i)[\s]*;[\s]*update";
        private const string OrAttack = @"(?i)'[\s]*or|(?i)[\s]*[\s]or[\s]";
        /// <summary>
        /// Sql injection a karsi verilen sql cumlesini filtreler.
        /// ici dolduralacaktir developerlarin calismasina devam edebilmesi icin simdilik bos
        /// </summary>
        /// <param name="inputSql">Filtrelenecek Sql</param>
        /// <returns></returns>
        public static string PreventSqlInjection(string inputSql)
        {
            if (string.IsNullOrEmpty(inputSql)) return inputSql;
            var isMatchAnyThreat = false;
            //Sql Delete Attack
            var match = Regex.Match(inputSql, DeleteAttack, RegexOptions.IgnoreCase);
            isMatchAnyThreat = match.Success;
            //Sql Drop Table Attack
            if (!isMatchAnyThreat)
            {
                match = Regex.Match(inputSql, DropTableAttack, RegexOptions.IgnoreCase);
                isMatchAnyThreat = match.Success;
            }
            //Sql Insert Attack
            if (!isMatchAnyThreat)
            {
                match = Regex.Match(inputSql, InsertAttack, RegexOptions.IgnoreCase);
                isMatchAnyThreat = match.Success;
            }
            //Sql Server Shutdown Attack
            if (!isMatchAnyThreat)
            {
                match = Regex.Match(inputSql, ShutdownAttack, RegexOptions.IgnoreCase);
                isMatchAnyThreat = match.Success;
            }
            //Sql Update Attack
            if (!isMatchAnyThreat)
            {
                match = Regex.Match(inputSql, UpdateAttack, RegexOptions.IgnoreCase);
                isMatchAnyThreat = match.Success;
            }
            //Sql Or Attack
            if (!isMatchAnyThreat)
            {
                match = Regex.Match(inputSql, OrAttack, RegexOptions.IgnoreCase);
                isMatchAnyThreat = match.Success;
            }
            if (isMatchAnyThreat)
                throw new SqlExecutionException(string.Format("Sql Injection Tespit Edildi. Sql: {0}", inputSql));
            return inputSql;
        }
        /// <summary>
        /// Sql injection a karsi verilen objeyi filtreler.
        /// ici dolduralacaktir developerlarin calismasina devam edebilmesi icin simdilik bos
        /// </summary>
        /// <param name="inputValue">Filtrelenecek Sql</param>
        /// <returns></returns>
        public static object PreventSqlInjection(object inputValue)
        {
            // ReSharper disable OperatorIsCanBeUsed
            return inputValue.GetType() == typeof(string) ? PreventSqlInjection(inputValue.ToString()) : inputValue;
            // ReSharper restore OperatorIsCanBeUsed
        }
    }
}
