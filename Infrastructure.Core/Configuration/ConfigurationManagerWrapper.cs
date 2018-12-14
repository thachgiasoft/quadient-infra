using System.Collections.Specialized;
using System.Configuration;

namespace Infrastructure.Core.Configuration
{
    //IOC Friendly"

    /// <summary>
    /// Configurasyon dosyasina erisimi saglayan siniftir.
    /// </summary>
    public class ConfigurationManagerWrapper : IConfigurationManager
    {
        /// <summary>
        /// Konfigurasyon dosyasindaki AppSettings bolumunu getirir. Readonly olarak isaretlenmistir.
        /// </summary>
        public NameValueCollection AppSettings
        {
            get { return ConfigurationManager.AppSettings; }
        }
        /// <summary>
        /// Konfigurasyon dosyasindaki ConnectionStrings bolumunu getirir. Readonly olarak isaretlenmistir.
        /// </summary>
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return ConfigurationManager.ConnectionStrings; }
        }
        /// <summary>
        /// Konfigurasyon dosyasindaki sectionName ile belirtilen bolumunu getirir.
        /// </summary>
        /// <returns>sectionName ile belirtilen bolumu object turunde dondurur.</returns>
        public object GetSection(string sectionName)
        {
            return ConfigurationManager.GetSection(sectionName);
        }

        public System.Configuration.Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel)
        {
            return ConfigurationManager.OpenExeConfiguration(userLevel);
        }

        public System.Configuration.Configuration OpenExeConfiguration(string exePath)
        {
            return ConfigurationManager.OpenExeConfiguration(exePath);
        }

        public System.Configuration.Configuration OpenMachineConfiguration()
        {
            return ConfigurationManager.OpenMachineConfiguration();
        }

        public System.Configuration.Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel)
        {
            return ConfigurationManager.OpenMappedExeConfiguration(fileMap, userLevel);
        }

        public System.Configuration.Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel, bool preLoad)
        {
            return ConfigurationManager.OpenMappedExeConfiguration(fileMap, userLevel, preLoad);
        }

        public System.Configuration.Configuration OpenMappedMachineConfiguration(ConfigurationFileMap fileMap)
        {
            return ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
        }

        public void RefreshSection(string sectionName)
        {
            ConfigurationManager.RefreshSection(sectionName);
        }
    }
}
