using Conditions;
using System;
using System.Configuration;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// Abstraction for getting configuration settings from app.config or web.config.
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        /// <summary>
        /// Gets a setting value, returning the default value if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <param name="defaultValue">The default value to use if the setting does not exist</param>
        /// <returns>the setting value or the default value if the setting does not exist</returns>
        public string GetSettingOrDefault(string settingName, string defaultValue)
        {
            Condition
                .Requires(settingName, "settingName")
                .IsNotNullOrWhiteSpace();

            var setting = GetSettingByName(settingName);

            return setting ?? defaultValue;
        }

        /// <summary>
        /// Gets a setting value, throwing an exception if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <returns>the setting value as a string</returns>
        public string GetSetting(string settingName)
        {
            var setting = GetSettingOrDefault(settingName, null);
            Condition
                .Ensures(setting, "setting")
                .IsNotNull("The '" + settingName + "' setting could not be found.");

            return setting;
        }

        /// <summary>
        /// Gets a setting value as the specified IConvertable type, returning the default value if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <param name="defaultValue">The default value to use if the setting does not exist</param>
        /// <returns>the setting value or the default value if the setting does not exist</returns>
        public T GetSettingAs<T>(string settingName, T defValue = default(T)) where T : IConvertible
        {
            var setting = GetSettingOrDefault(settingName, Convert.ToString(defValue));
            if (string.IsNullOrEmpty(setting)) return defValue;
            T value = (T)Convert.ChangeType(setting, typeof(T));

            return value;
        }

        /// <summary>
        /// Gets a setting value converted to the specified stuct type, returning the default value if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <param name="defaultValue">The default value to use if the setting does not exist</param>
        /// <returns>the setting value or the default value if the setting does not exist</returns>
        public Nullable<T> GetSettingAsNullable<T>(string settingName, Nullable<T> defValue = null) where T : struct
        {
            var setting = GetSettingOrDefault(settingName, Convert.ToString(defValue));
            if (string.IsNullOrEmpty(setting)) return defValue;
            T value = (T)Convert.ChangeType(setting, typeof(T));

            return value;
        }

        /// <summary>
        /// Gets a connection string with the specified name, throwing an exception
        /// if the connection string does not exist
        /// </summary>
        /// <param name="name">Name of the connection string setting to get</param>
        /// <returns></returns>
        public string GetConnectionString(string name)
        {
            var cs = GetConnectionStringByName(name);
            Condition
                .Ensures(cs, "setting")
                .IsNotNull("The '" + name + "' connection string could not be found.");

            return cs;
        }

        /// <summary>
        /// Clears the settings cache and re-loads settings from storage.
        /// </summary>
        public virtual void ForceRefresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        #region overridable provider implementation

        protected virtual string GetConnectionStringByName(string name)
        {
            var cs = ConfigurationManager.ConnectionStrings[name];
            return cs != null ? cs.ConnectionString : null;
        }

        protected virtual string GetSettingByName(string settingName)
        {
            return ConfigurationManager.AppSettings[settingName];
        }

        #endregion

    }
}
