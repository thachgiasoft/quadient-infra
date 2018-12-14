using Infrastructure.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Settings
{
    public class ConfigFileSettingProvider : ISettingProvider
    {
        private readonly IConfigurationManager _configurationManager;
        public ConfigFileSettingProvider(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }
        public void SetSetting(Setting setting)
        {
            throw new NotSupportedException();
        }

        public Setting GetSetting(string key)
        {
            return new Setting() { Key = key, Value = _configurationManager.AppSettings[key] };
        }

        public Setting GetSetting(int id)
        {
            throw new NotSupportedException();
        }

        public void DeleteSetting(string key)
        {
            throw new NotSupportedException();
        }

        public void DeleteSetting(int id)
        {
            throw new NotSupportedException();
        }

        public bool SettingExists(string key)
        {
            return _configurationManager.AppSettings[key] != null;
        }

        public IList<Setting> GetAllSettings()
        {
            var list = new List<Setting>();
            list.AddRange(_configurationManager.AppSettings.AllKeys
                              .Select(key => new Setting() { Key = key, Value = _configurationManager.AppSettings[key] }));
            return list;
        }
    }
}
