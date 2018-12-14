using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace Infrastructure.Data
{
    public interface IDataConfigurationSettings : IConfigurationSectionHandler
    {
        Type DatabaseType { get; }
        string DefaultDatabaseName { get; }
        string LogDatabaseName { get; }
        string ConnectionStringRegistryKeyPath { get; }
        Type ConfigurationSourceType { get; }
        string DatabaseProviderName { get; }
        IList<DbProviderMapping> ProviderMappings { get; }
    }
}
