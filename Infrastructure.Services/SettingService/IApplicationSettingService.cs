using System;
using System.Collections.Generic;
using System.ServiceModel;
using Infrastructure.Core.Settings;

namespace Infrastructure.Services.SettingService
{
    [ServiceContract]
    public interface IApplicationSettingService
    {
        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="isEncrypted">Indicates Value is encryptd or decrypted</param>

        [OperationContract]
        void SetSetting(string key, string value, bool isEncrypted = false);

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        T GetSetting<T>(string key, T defaultValue = default(T))
            where T : IComparable, IConvertible, IComparable<T>, IEquatable<T>;
        /// <summary>
        /// Implemented for AppSettingManager. Get Setting By Id
        /// </summary>
        /// <param name="appsettingId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetSetting")]
        string GetSetting(string appSettingKey);

        [OperationContract(Name = "GetSettingByKey")]
        Setting GetSettingByKey(string key);

        /// <summary>
        /// Implemented for AppSettingManager. Get Setting By Id
        /// </summary>
        /// <param name="appsettingId"></param>
        /// <returns></returns>
        [OperationContract(Name = "GetSettingById")]
        Setting GetSetting(int appsettingId);

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Settings</returns>
        [OperationContract]
        IList<Setting> GetAllSettings();

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        [OperationContract]
        void DeleteSetting(Setting setting);

        /// <summary>
        /// Deletes a string by name
        /// </summary>
        /// <param name="key"></param>
        [OperationContract(Name = "DeleteSettingByKey")]
        void DeleteSetting(string key);

        /// <summary>
        /// Implemented for AppSettingManager. Deletes a setting by Id.
        /// </summary>
        /// <param name="appSettingId"></param>
        [OperationContract(Name = "DeleteSettingById")]
        void DeleteSetting(int appSettingId);

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true -setting exists; false - does not exist</returns>
        [OperationContract]
        bool SettingExists(string key);

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();
    }
}