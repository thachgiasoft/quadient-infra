using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace Infrastructure.Data.Configuration
{
    public class DefaultDataConfigurationSettings : IDataConfigurationSettings
    {
        private const string DatabaseNodeName = "DatabaseType";
        private const string TypeAttributeName = "type";
        private const string DefaultDatabaseNameNodeName = "DefaultDatabaseName";
        private const string LogDatabaseNameNodeName = "LogDatabaseName";
        private const string RegistryKeyPathNodeName = "ConnectionStringRegistryKeyPath";
        private const string ConfigurationSourceTypeNodeName = "ConfigurationSource";
        private const string DatabaseProviderNameNodeName = "DatabaseProviderName";
        private const string DbProviderMappingNodeName = "DbProviderMapping";
        private const string ValueAttributeName = "value";

        private readonly Type _defaultDatabaseType;
        public DefaultDataConfigurationSettings()
        {
            _defaultDatabaseType = typeof(SqlDatabase);
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new DefaultDataConfigurationSettings();
            var databaseTypeNode = section.SelectSingleNode(DatabaseNodeName);
            if (databaseTypeNode != null && databaseTypeNode.Attributes != null)
            {
                var typeNodeAttribute = databaseTypeNode.Attributes[TypeAttributeName];
                if (typeNodeAttribute == null || string.IsNullOrEmpty(typeNodeAttribute.Value))
                {
                    config.DatabaseType = _defaultDatabaseType;
                }
                else
                {
                    var type = Type.GetType(typeNodeAttribute.Value);
                    if (type == null)
                    {
                        config.DatabaseType = _defaultDatabaseType;
                    }
                    else if (type.IsSubclassOf(typeof(Database)))
                    {
                        config.DatabaseType = type;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("The type {0} is not derived from {1}", type.FullName, typeof(Database).FullName));
                    }
                }
            }
            var defaultDatabaseNameNode = section.SelectSingleNode(DefaultDatabaseNameNodeName);
            if (defaultDatabaseNameNode != null && defaultDatabaseNameNode.Attributes != null)
            {
                var defaultDatabaseNameNodeAttribute = defaultDatabaseNameNode.Attributes[ValueAttributeName];
                config.DefaultDatabaseName = defaultDatabaseNameNodeAttribute == null ? string.Empty : defaultDatabaseNameNodeAttribute.Value;
            }
            var logDatabaseNameNode = section.SelectSingleNode(LogDatabaseNameNodeName);
            if (logDatabaseNameNode != null && logDatabaseNameNode.Attributes != null)
            {
                var logDatabaseNameNodeAttribute = logDatabaseNameNode.Attributes[ValueAttributeName];
                config.LogDatabaseName = logDatabaseNameNodeAttribute == null ? string.Empty : logDatabaseNameNodeAttribute.Value;
            }
            var registryKeyPathNode = section.SelectSingleNode(RegistryKeyPathNodeName);
            if (registryKeyPathNode != null && registryKeyPathNode.Attributes != null)
            {
                var registryKeyPathNodeAttribute = registryKeyPathNode.Attributes[ValueAttributeName];
                config.ConnectionStringRegistryKeyPath = registryKeyPathNodeAttribute == null ? string.Empty : registryKeyPathNodeAttribute.Value;
            }
            var configurationSourceTypeNode = section.SelectSingleNode(ConfigurationSourceTypeNodeName);
            if (configurationSourceTypeNode != null && configurationSourceTypeNode.Attributes != null)
            {
                var typeNodeAttribute = configurationSourceTypeNode.Attributes[TypeAttributeName];
                if (typeNodeAttribute != null && !string.IsNullOrEmpty(typeNodeAttribute.Value))
                {
                    config.ConfigurationSourceType = Type.GetType(typeNodeAttribute.Value);
                }
            }
            var databaseProviderNameNode = section.SelectSingleNode(DatabaseProviderNameNodeName);
            if (databaseProviderNameNode != null && databaseProviderNameNode.Attributes != null)
            {
                var databaseProviderNameNodAttribute = databaseProviderNameNode.Attributes[ValueAttributeName];
                config.DatabaseProviderName = databaseProviderNameNodAttribute == null ? string.Empty : databaseProviderNameNodAttribute.Value;
            }

            config.ProviderMappings = new List<DbProviderMapping>();
            var providerMappingsNode = section.SelectSingleNode(DbProviderMappingNodeName);
            if (providerMappingsNode != null)
            {
                var addNodes = providerMappingsNode.SelectNodes("add");
                if (addNodes != null)
                {
                    foreach (XmlNode addNode in addNodes)
                    {
                        if (addNode.Attributes.Count >= 2 && addNode.Attributes["databaseType"] != null &&
                            addNode.Attributes["name"] != null)
                            config.ProviderMappings.Add(new DbProviderMapping(addNode.Attributes["name"].Value, addNode.Attributes["databaseType"].Value));
                    }
                }
            }
            return config;
        }

        public Type DatabaseType { get; private set; }
        public string DefaultDatabaseName { get; private set; }
        public string LogDatabaseName { get; private set; }
        public string ConnectionStringRegistryKeyPath { get; private set; }
        public Type ConfigurationSourceType { get; private set; }
        public string DatabaseProviderName { get; private set; }
        public IList<DbProviderMapping> ProviderMappings { get; private set; }
    }
}
