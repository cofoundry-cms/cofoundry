using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// Abstraction for getting configuration settings.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets a setting value, returning the default value if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <param name="defaultValue">The default value to use if the setting does not exist</param>
        /// <returns>the setting value or the default value if the setting does not exist</returns>
        string GetSetting(string settingName);

        /// <summary>
        /// Gets a setting value, throwing an exception if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <returns>the setting value as a string</returns>
        T GetSettingAs<T>(string settingName, T defValue = default(T)) where T : IConvertible;

        /// <summary>
        /// Gets a setting value as the specified IConvertable type, returning the default value if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <param name="defaultValue">The default value to use if the setting does not exist</param>
        /// <returns>the setting value or the default value if the setting does not exist</returns>
        T? GetSettingAsNullable<T>(string settingName, T? defValue = null) where T : struct;

        /// <summary>
        /// Gets a setting value converted to the specified stuct type, returning the default value if the setting does not exist.
        /// </summary>
        /// <param name="settingName">Name of the setting to get</param>
        /// <param name="defaultValue">The default value to use if the setting does not exist</param>
        /// <returns>the setting value or the default value if the setting does not exist</returns>
        string GetSettingOrDefault(string settingName, string defaultValue);

        /// <summary>
        /// Gets a connection string with the specified name, throwing an exception
        /// if the connection string does not exist
        /// </summary>
        /// <param name="name">Name of the connection string setting to get</param>
        /// <returns></returns>
        string GetConnectionString(string name);

        /// <summary>
        /// Clears the settings cache and re-loads settings from storage.
        /// </summary>
        void ForceRefresh();
    }
}
