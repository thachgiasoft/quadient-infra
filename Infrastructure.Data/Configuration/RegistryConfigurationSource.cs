using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Infrastructure.Core.Cryptography;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.RegistryManager;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace Infrastructure.Data.Configuration
{
    /// <summary>
    /// Sistem kayit defteri yonetim sinifi.
    /// </summary>
    public class RegistryConfigurationSource : IConfigurationSource
    {
        private readonly IRegistryManager _registryManager;
        private readonly IDataConfigurationSettings _dataConfigurationSettings;
        private readonly ICoreCryptography _cryptography;

        public RegistryConfigurationSource()
        {
            _registryManager = EngineContext.Current.Resolve<IRegistryManager>();
            _dataConfigurationSettings = EngineContext.Current.Resolve<IDataConfigurationSettings>();
            _cryptography = EngineContext.Current.Resolve<ICoreCryptography>();
        }

        public void Add(string sectionName, ConfigurationSection configurationSection)
        {
            throw new NotImplementedException();
        }

        public void AddSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Kayit defterinden ilgili section i okur.
        /// </summary>
        /// <param name="sectionName">Section adi</param>
        /// <returns>Ilgili section bilgisini dondurur.</returns>
        public ConfigurationSection GetSection(string sectionName)
        {
            try
            {
                switch (sectionName)
                {
                    case "connectionStrings":
                        return GetConnectionStringsSection();
                    case "dataConfiguration":
                        return GetDatabaseSettings();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(String.Format(ex.Message));
            }
        }

        private ConnectionStringsSection GetConnectionStringsSection()
        {
            var result = new ConnectionStringsSection();

            //load configs from registry
            foreach (Connection connection in LoadConfigFromRegistry())
                result.ConnectionStrings.Add(new ConnectionStringSettings(connection.DatabaseName,
                                                                           connection.ConnectionString, connection.ProviderName));
            return result;
        }

        private IEnumerable<Connection> LoadConfigFromRegistry()
        {
            var connectionList = new List<Connection>();

            string path = _dataConfigurationSettings.ConnectionStringRegistryKeyPath;
            IDictionary<string, object> registryListItems = _registryManager.GetValues(path);

            foreach (var conList in registryListItems)
            {
                var xmlConstr = _cryptography.Decrypt(conList.Value.ToString());
                var document = new XmlDocument();
                document.LoadXml(xmlConstr);
                var conStrNode = document.SelectSingleNode("add");
                if (conStrNode != null && conStrNode.Attributes != null)
                {
                    var name = conStrNode.Attributes["name"].Value;
                    var connectionString = conStrNode.Attributes["connectionString"].Value;
                    var providerName = conStrNode.Attributes["providerName"].Value;
                    var configSetting = new Connection
                        {
                            DatabaseName = name,
                            ConnectionString = connectionString,
                            ProviderName = providerName
                        };
                    connectionList.Add(configSetting);
                }
            }

            return connectionList;
        }

        private ConfigurationSection GetDatabaseSettings()
        {
            var settings = new DatabaseSettings();
            //extra provider mappings i ekle
            foreach (var mapping in _dataConfigurationSettings.ProviderMappings)
            {
                settings.ProviderMappings.Add(mapping);
            }
            settings.DefaultDatabase = _dataConfigurationSettings.DefaultDatabaseName;
            return settings;
        }

        public void Remove(string sectionName)
        {
            throw new NotImplementedException();
        }

        public void RemoveSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ConfigurationSourceChangedEventArgs> SourceChanged;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
