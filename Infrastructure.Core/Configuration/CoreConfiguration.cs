using System;
using System.Configuration;
using System.Xml;
using Infrastructure.Core.Infrastructure;

namespace Infrastructure.Core.Configuration
{
    public class CoreConfiguration : IConfigurationSectionHandler
    {
        private const string DefaultSaltValue = "d41d8cd98f00b204e9800998ecf8427e";
        private const string DefaultPassPhrase = "74be16979710d4c4e7c6647856088456";
        private const string ApplicationSettingServiceBaseEndpointNodeName = "ApplicationSettingServiceBaseEndpoint";
        private const string ValueAttributeName = "value";
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new CoreConfiguration();
            var dynamicDiscoveryNode = section.SelectSingleNode("DynamicDiscovery");
            if (dynamicDiscoveryNode != null && dynamicDiscoveryNode.Attributes != null)
            {
                var attribute = dynamicDiscoveryNode.Attributes["Enabled"];
                if (attribute != null)
                    config.DynamicDiscovery = Convert.ToBoolean(attribute.Value);
            }

            var engineNode = section.SelectSingleNode("Engine");
            if (engineNode != null && engineNode.Attributes != null)
            {
                var attribute = engineNode.Attributes["Type"];
                if (attribute != null)
                    config.EngineType = attribute.Value;
            }

            var cryptographyNode = section.SelectSingleNode("Cryptography");
            if (cryptographyNode != null && cryptographyNode.Attributes != null)
            {
                config.CryptographyConf = new CryptographyConfiguration();
                var attrSaltValue = cryptographyNode.Attributes["SaltValue"];
                if (attrSaltValue != null && !string.IsNullOrEmpty(attrSaltValue.Value))
                    config.CryptographyConf.SaltValue = attrSaltValue.Value;
                else
                    config.CryptographyConf.SaltValue = DefaultSaltValue;//Defult value

                var attrPassPhrase = cryptographyNode.Attributes["PassPhrase"];
                if (attrPassPhrase != null && !string.IsNullOrEmpty(attrPassPhrase.Value))
                    config.CryptographyConf.PassPhrase = attrPassPhrase.Value;
                else
                    config.CryptographyConf.PassPhrase = DefaultPassPhrase;//Defult value

                var attrPassIterations = cryptographyNode.Attributes["PasswordIterations"];
                if (attrPassIterations != null && !string.IsNullOrEmpty(attrPassIterations.Value))
                    config.CryptographyConf.PasswordIterations = int.Parse(attrPassIterations.Value);
                else
                    config.CryptographyConf.PasswordIterations = 1; //Defult value
            }
            var assemblySkipLoadingPatternNode = section.SelectSingleNode("AssemblySkipLoadingPattern");
            if (assemblySkipLoadingPatternNode != null && assemblySkipLoadingPatternNode.Attributes != null)
            {
                var attribute = assemblySkipLoadingPatternNode.Attributes["value"];
                if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                    config.AssemblySkipLoadingPattern = attribute.Value;
            }
            var distributedCacheNode = section.SelectSingleNode("DistributedCacheManager");
            if (distributedCacheNode != null && distributedCacheNode.Attributes != null)
            {
                var attribute = distributedCacheNode.Attributes["type"];
                if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                    config.DistributedCacheManager = attribute.Value;
            }
            var distributedCacheNameNode = section.SelectSingleNode("DistributedCacheName");
            if (distributedCacheNameNode != null && distributedCacheNameNode.Attributes != null)
            {
                var attribute = distributedCacheNameNode.Attributes["value"];
                if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                    config.DistributedCacheName = attribute.Value;
            }
            var applicationSettingServiceBaseEndpointNode = section.SelectSingleNode(ApplicationSettingServiceBaseEndpointNodeName);
            if (applicationSettingServiceBaseEndpointNode != null && applicationSettingServiceBaseEndpointNode.Attributes != null)
            {
                var applicationSettingServiceBaseEndpointAttribute = applicationSettingServiceBaseEndpointNode.Attributes[ValueAttributeName];
                config.ApplicationSettingServiceBaseEndpoint = applicationSettingServiceBaseEndpointAttribute == null ? string.Empty : applicationSettingServiceBaseEndpointAttribute.Value;
            }

            config.ApplicationType = ApplicationType.WebForm; //default
            var applicationTypeNode = section.SelectSingleNode("ApplicationType");
            if (applicationTypeNode != null && applicationTypeNode.Attributes != null)
            {
                var attribute = applicationTypeNode.Attributes["value"];
                ApplicationType type = ApplicationType.WebForm;
                if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                    Enum.TryParse<ApplicationType>(attribute.Value, out type);
                config.ApplicationType = type;
            }

            return config;
        }

        /// <summary>
        /// In addition to configured assemblies examine and load assemblies in the bin directory.
        /// </summary>
        public bool DynamicDiscovery { get; set; }

        /// <summary>
        /// A custom <see cref="IEngine"/> to manage the application instead of the default.
        /// </summary>
        public string EngineType { get; set; }

        /// <summary>
        /// Includes cryptography parameters
        /// </summary>
        public CryptographyConfiguration CryptographyConf { get; set; }

        /// <summary>
        /// Assembly names to skip
        /// </summary>
        public string AssemblySkipLoadingPattern { get; set; }
        /// <summary>
        /// Default Distributed Cache Manager
        /// </summary>
        public string DistributedCacheManager { get; set; }
        /// <summary>
        /// Default Distributed Cache Name
        /// </summary>
        public string DistributedCacheName { get; set; }
        /// <summary>
        /// base endpoint
        /// </summary>
        public string ApplicationSettingServiceBaseEndpoint { get; private set; }
        /// <summary>
        /// Get Application Type
        /// </summary>
        public ApplicationType ApplicationType { get; set; }
    }

    public enum ApplicationType
    {
        WebMVC = 0,
        WebForm = 1,
        WCF = 2,
        Windows = 3,
    }

    public class CryptographyConfiguration
    {
        public string SaltValue { get; set; }
        public string PassPhrase { get; set; }
        public int PasswordIterations { get; set; }
        public string InitVector { get; set; }
        public int KeySize { get; set; }
    }
}