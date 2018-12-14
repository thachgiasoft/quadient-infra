using System;
using System.Data;

namespace Infrastructure.Data.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchCriteriaParameter : Attribute
    {
        #region Fields
        private string parameterName = string.Empty;
        private SqlDbType dbType = SqlDbType.Variant;
        #endregion


        public SearchCriteriaParameter(string _parameterName, SqlDbType _dbType)
        {
            parameterName = _parameterName;
            dbType = _dbType;
        }

        public SearchCriteriaParameter(string _parameterName)
        {
            parameterName = _parameterName;
        }

        public SearchCriteriaParameter(SqlDbType _dbType)
        {
            dbType = _dbType;
        }

        public SearchCriteriaParameter()
        {
        }

        public string ParameterName
        {
            get { return parameterName; }
        }

        public SqlDbType DbType
        {
            get { return dbType; }
        }
    }
}
