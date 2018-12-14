using System;
using System.Collections.Generic;
using System.Diagnostics;
using Infrastructure.Core.Caching;
using Infrastructure.Core.Cryptography;
using Infrastructure.Core.Helpers;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.Settings;

namespace Infrastructure.Services.SettingService
{
    public class ApplicationSettingService : IApplicationSettingService
    {
        /// <summary>
        /// Setting manager
        /// </summary>
        private readonly ICoreCryptography _coreCryptography;
        private readonly ISettingProvider _settingProvider;
        private readonly SettingNotification _settingNotification;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="coreCryptography">crytopraphy</param>
        /// <param name="settingProvider"></param>
        /// <param name="settingNotification"></param>
        public ApplicationSettingService(ICoreCryptography coreCryptography, ISettingProvider settingProvider, SettingNotification settingNotification)
        {
            _coreCryptography = coreCryptography;
            _settingProvider = settingProvider;
            _settingNotification = settingNotification;
            //SetAllSettingsToCache();
        }

        //private void SetAllSettingsToCache()
        //{
        //    foreach (var setting in _settingProvider.GetAllSettings())
        //    {
        //        _cacheManager.Set(setting.Key, setting);
        //    }
        //}

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="isEncrypted"></param>
        public virtual void SetSetting(string key, string value, bool isEncrypted = false)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim();
            Setting setting = null;
            //if (_cacheManager.IsSet(key))
            //{
            //    setting = _cacheManager.Get<Setting>(key);
            //    setting.Value = isEncrypted ? _coreCryptography.Encrypt(value) : value;
            //    setting.IsEncrypted = isEncrypted;
            //}
            if (setting == null)
                setting = new Setting
                    {
                        Key = key,
                        IsEncrypted = isEncrypted,
                        Value = value
                    };
            _settingProvider.SetSetting(setting);

            //_cacheManager.Set(key, setting);
            try
            {
                _settingNotification.NotifyForSettingChange(new SettingNotify(setting) { NotifyType = "SetSetting" });
            }
            catch (Exception exception)
            {

                EventLog.WriteEntry("Application", exception.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public virtual T GetSetting<T>(string key, T defaultValue = default(T)) where T : IComparable, IConvertible, IComparable<T>, IEquatable<T>
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            //var setting = _cacheManager.Get<Setting>(key);

            //if (setting != null)
            //    return CommonHelper.To<T>(setting.IsEncrypted ? _coreCryptography.Decrypt(setting.Value) : setting.Value);

            var setting = _settingProvider.GetSetting(key);
            if (setting != null)
            {
                //_cacheManager.Set(key, setting);
                return CommonHelper.To<T>(setting.Value);
                //return CommonHelper.To<T>(setting.IsEncrypted ? _coreCryptography.Decrypt(setting.Value) : setting.Value);
            }
            EventLog.WriteEntry("Application", string.Format("Application Setting Key Not Found! Key : {0}", key), EventLogEntryType.Error);
            return defaultValue;
        }

        public string GetSetting(string appSettingKey)
        {
            return GetSetting<string>(appSettingKey, string.Empty);
        }

        public Setting GetSettingByKey(string key)
        {
            var retSetting = _settingProvider.GetSetting(key);
            //if (retSetting != null && retSetting.IsEncrypted)
            //  retSetting.Value = _coreCryptography.Decrypt(retSetting.Value);
            return retSetting;
        }

        public Setting GetSetting(int appSettingId)
        {
            var retSetting = _settingProvider.GetSetting(appSettingId);
            if (retSetting != null && retSetting.IsEncrypted)
                retSetting.Value = _coreCryptography.Decrypt(retSetting.Value);
            return retSetting;
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            _settingProvider.DeleteSetting(setting.Key);
            //cache
            //_cacheManager.RemoveByPattern(setting.Key);
            try
            {
                _settingNotification.NotifyForSettingChange(new SettingNotify(setting) { NotifyType = "DeleteSetting" });
            }
            catch (Exception exception)
            {

                EventLog.WriteEntry("Application", exception.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="key"></param>
        public virtual void DeleteSetting(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            _settingProvider.DeleteSetting(key);
            //cache
            //_cacheManager.RemoveByPattern(key);
            try
            {
                _settingNotification.NotifyForSettingChange(new SettingNotify(new Setting() { Key = key }) { NotifyType = "DeleteSetting" });
            }
            catch (Exception exception)
            {

                EventLog.WriteEntry("Application", exception.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Deletes a setting by Id
        /// </summary>
        /// <param name="appSettingId"></param>
        public virtual void DeleteSetting(int appSettingId)
        {
            var setting = GetSetting(appSettingId);
            //if (setting != null)
            //{
            //    var settingCached = _cacheManager.Get<Setting>(setting.Key);
            //    //cache
            //    if (settingCached != null)
            //        _cacheManager.RemoveByPattern(settingCached.Key);
            //}
            _settingProvider.DeleteSetting(appSettingId);
            try
            {
                if (setting != null) _settingNotification.NotifyForSettingChange(new SettingNotify(setting) { NotifyType = "DeleteSetting" });
            }
            catch (Exception exception)
            {

                EventLog.WriteEntry("Application", exception.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IList<Setting> GetAllSettings()
        {
            return _settingProvider.GetAllSettings();
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <returns>true -setting exists; false - does not exist</returns>
        public virtual bool SettingExists(string key)
        {
            return _settingProvider.SettingExists(key);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            //SetAllSettingsToCache();
        }

    }
}
