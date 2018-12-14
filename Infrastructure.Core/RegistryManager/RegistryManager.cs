using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Win32;

namespace Infrastructure.Core.RegistryManager
{
    public class RegistryManager : IRegistryManager
    {
        /// <summary>
        /// Returns registry value for the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns>db conn string</returns>
        public object GetValue(string path, string name)
        {
            string value64 = string.Empty;
            RegistryKey localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            localKey = localKey.OpenSubKey(path);
            try
            {
                if (localKey != null)
                    value64 = localKey.GetValue(name).ToString();
            }
            catch (Exception)
            {

                EventLog.WriteEntry("Application", "Kayıt defteri okunurken hata oluştu.", EventLogEntryType.Error);
            }
            return value64;
        }

        /// <summary>
        ///Returns registry keys and values for the given path
        /// </summary>
        /// <param name="path">HKEY_LOCAL_MACHINE/SOFTWARE/Icisleri/Ebakanlik/ConnStr</param>
        /// <returns>db connection strings</returns>
        public Dictionary<string, object> GetValues(string path)
        {
            //convert path to RegistryKey
            var values = new Dictionary<string, object>();

            RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                                       RegistryView.Registry64);
            root = root.OpenSubKey(path);

            if (root != null)
            {
                foreach (var value in root.GetValueNames())
                {
                    try
                    {
                        values.Add(string.Format(value), (root.GetValue(value).ToString() ?? ""));
                    }
                    catch (Exception)
                    {

                        EventLog.WriteEntry("Application", "Kayıt defteri okunurken hata oluştu.", EventLogEntryType.Error);
                    }
                }
                return values;
            }
            return null;
        }
    }
}
