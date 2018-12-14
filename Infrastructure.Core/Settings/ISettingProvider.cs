using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Settings
{
    public interface ISettingProvider
    {
        /// <summary>
        /// Sets or update setting
        /// </summary>
        /// <param name="setting"></param>
        void SetSetting(Setting setting);
        /// <summary>
        /// Get Setting By Id
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Setting GetSetting(string key);
        /// <summary>
        /// Get Setting By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Setting GetSetting(int id);
        /// <summary>
        /// Deletes a string by name
        /// </summary>
        /// <param name="key"></param>
        void DeleteSetting(string key);
        /// <summary>
        ///   Deletes a setting by Id.
        /// </summary>
        /// <param name="id"></param>
        void DeleteSetting(int id);
        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true -setting exists; false - does not exist</returns>
        bool SettingExists(string key);
        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        IList<Setting> GetAllSettings();
    }
}
