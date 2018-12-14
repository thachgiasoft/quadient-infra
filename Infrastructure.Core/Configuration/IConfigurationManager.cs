using System.Collections.Specialized;
using System.Configuration;

namespace Infrastructure.Core.Configuration
{
    public interface IConfigurationManager
    {
        NameValueCollection AppSettings { get; }
        //
        // Summary:
        //     Gets the System.Configuration.ConnectionStringsSection data for the current
        //     application's default configuration.
        //
        // Returns:
        //     Returns a System.Configuration.ConnectionStringSettingsCollection object
        //     that contains the contents of the System.Configuration.ConnectionStringsSection
        //     object for the current application's default configuration.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     Could not retrieve a System.Configuration.ConnectionStringSettingsCollection
        //     object.
        ConnectionStringSettingsCollection ConnectionStrings { get; }

        // Summary:
        //     Retrieves a specified configuration section for the current application's
        //     default configuration.
        //
        // Parameters:
        //   sectionName:
        //     The configuration section path and name.
        //
        // Returns:
        //     The specified System.Configuration.ConfigurationSection object, or null if
        //     the section does not exist.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        object GetSection(string sectionName);
        //
        // Summary:
        //     Opens the configuration file for the current application as a System.Configuration.Configuration
        //     object.
        //
        // Parameters:
        //   userLevel:
        //     The System.Configuration.ConfigurationUserLevel for which you are opening
        //     the configuration.
        //
        // Returns:
        //     A System.Configuration.Configuration object.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        System.Configuration.Configuration OpenExeConfiguration(ConfigurationUserLevel userLevel);
        //
        // Summary:
        //     Opens the specified client configuration file as a System.Configuration.Configuration
        //     object.
        //
        // Parameters:
        //   exePath:
        //     The path of the executable (exe) file.
        //
        // Returns:
        //     A System.Configuration.Configuration object.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        System.Configuration.Configuration OpenExeConfiguration(string exePath);
        //
        // Summary:
        //     Opens the machine configuration file on the current computer as a System.Configuration.Configuration
        //     object.
        //
        // Returns:
        //     A System.Configuration.Configuration object.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        System.Configuration.Configuration OpenMachineConfiguration();
        //
        // Summary:
        //     Opens the specified client configuration file as a System.Configuration.Configuration
        //     object that uses the specified file mapping and user level.
        //
        // Parameters:
        //   fileMap:
        //     An System.Configuration.ExeConfigurationFileMap object that references configuration
        //     file to use instead of the application default configuration file.
        //
        //   userLevel:
        //     The System.Configuration.ConfigurationUserLevel object for which you are
        //     opening the configuration.
        //
        // Returns:
        //     The configuration object.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        System.Configuration.Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel);
        //
        // Summary:
        //     Opens the specified client configuration file as a System.Configuration.Configuration
        //     object that uses the specified file mapping, user level, and preload option.
        //
        // Parameters:
        //   fileMap:
        //     An System.Configuration.ExeConfigurationFileMap object that references the
        //     configuration file to use instead of the default application configuration
        //     file.
        //
        //   userLevel:
        //     The System.Configuration.ConfigurationUserLevel object for which you are
        //     opening the configuration.
        //
        //   preLoad:
        //     true to preload all section groups and sections; otherwise, false.
        //
        // Returns:
        //     The configuration object.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        System.Configuration.Configuration OpenMappedExeConfiguration(ExeConfigurationFileMap fileMap, ConfigurationUserLevel userLevel, bool preLoad);
        //
        // Summary:
        //     Opens the machine configuration file as a System.Configuration.Configuration
        //     object that uses the specified file mapping.
        //
        // Parameters:
        //   fileMap:
        //     An System.Configuration.ExeConfigurationFileMap object that references configuration
        //     file to use instead of the application default configuration file.
        //
        // Returns:
        //     A System.Configuration.Configuration object.
        //
        // Exceptions:
        //   System.Configuration.ConfigurationErrorsException:
        //     A configuration file could not be loaded.
        System.Configuration.Configuration OpenMappedMachineConfiguration(ConfigurationFileMap fileMap);
        //
        // Summary:
        //     Refreshes the named section so the next time that it is retrieved it will
        //     be re-read from disk.
        //
        // Parameters:
        //   sectionName:
        //     The configuration section name or the configuration path and section name
        //     of the section to refresh.
        void RefreshSection(string sectionName);
    }
}
