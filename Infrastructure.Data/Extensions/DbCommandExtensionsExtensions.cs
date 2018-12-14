using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Extensions
{
    public static class DbCommandExtensions
    {
        /// <summary>
        /// Adds new parameter to DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public static void AddParameterWithValue(this DbCommand cmd, string parameterName, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(parameter);
        }
        /// <summary>
        /// Adds new parameter to DbCommand with DBType
        ///todo: DBType in default value sı bilinmediğinden kod tekrari yapildi duzeltilecek
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public static void AddParameterWithValue(this DbCommand cmd, string parameterName, object value, DbType dbType)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;
            parameter.DbType = dbType;
            cmd.Parameters.Add(parameter);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public static void AddWithValueSafe(this SqlParameterCollection target, string parameterName, object value)
        {
            target.AddWithValue(parameterName, value ?? DBNull.Value);
        }

        public static void AddWithValueSafe(this SqlParameterCollection target, string parameterName, SqlDbType dbType, object value)
        {
            target.Add(new SqlParameter()
                {
                    SqlDbType = dbType,
                    ParameterName = parameterName,
                    Value = value ?? DBNull.Value
                });
        }

        public static void AddWithValueSafe(this SqlParameterCollection target, string parameterName, SqlDbType dbType, object value, string udtTypeName)
        {
            target.Add(new SqlParameter()
            {
                SqlDbType = dbType,
                ParameterName = parameterName,
                Value = value ?? DBNull.Value,
                UdtTypeName = udtTypeName
            });
        }
    }
}
