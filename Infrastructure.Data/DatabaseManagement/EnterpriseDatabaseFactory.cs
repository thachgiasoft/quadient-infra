using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Infrastructure.Data.DatabaseManagement
{
    public class EnterpriseDatabaseFactory : IDatabaseFactory
    {
        private readonly DatabaseProviderFactory _factory;
        private readonly IDataConfigurationSettings _dataConfigurationSettings;
        public EnterpriseDatabaseFactory(IConfigurationSource configurationSource, IDataConfigurationSettings dataConfigurationSettings)
        {
            _factory = new DatabaseProviderFactory(configurationSource);
            DatabaseFactory.SetDatabaseProviderFactory(_factory);
            _dataConfigurationSettings = dataConfigurationSettings;

        }
        public T CreateDatabase<T>(string name) where T : Database
        {
            if (string.IsNullOrEmpty(name))
                name = _dataConfigurationSettings.DefaultDatabaseName;
            var db = DatabaseFactory.CreateDatabase(name) as T;
            if (db == null)
                throw new InvalidOperationException(string.Format("DatabaseFactory.CreateDatabase methodu ile yaratılan database T tipine dönüştürülemiyor veya DbProviderMapping hatali. T Tipi : {0}", typeof(T)));
            return db;
        }
    }
}
