using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// Static access to a configuration service, always prefer to use an injected
    /// instance of ConfigurationService unless you are sure the configration source will
    /// always be the web/app.config file.
    /// </summary>
    public static class ConfigurationHelper
    {
        private static readonly ConfigurationService _config = new ConfigurationService();

        public static string GetSettingOrDefault(string settingName, string defaultValue)
        {
            return _config.GetSettingOrDefault(settingName, defaultValue);
        }

        public static string GetSetting(string settingName)
        {
            return _config.GetSetting(settingName);
        }

        public static bool GetSettingAsBool(string settingName, bool? defValue = null)
        {
            var setting = _config.GetSettingOrDefault(settingName, Convert.ToString(defValue));
            bool b = false;
            if (!bool.TryParse(setting, out b))
            {
                throw new ApplicationException("The '" + settingName + "' setting could not be parsed as a boolean");
            }
            return b;
        }

        public static T GetSettingAs<T>(string settingName, T defValue = default(T)) where T : IConvertible
        {
            return _config.GetSettingAs<T>(settingName, defValue);
        }

        public static Nullable<T> GetSettingAsNullable<T>(string settingName, Nullable<T> defValue = null) where T : struct
        {
            return _config.GetSettingAsNullable<T>(settingName, defValue);
        }
    }
}
