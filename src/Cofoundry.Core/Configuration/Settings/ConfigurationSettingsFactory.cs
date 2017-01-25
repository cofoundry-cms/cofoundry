using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Validation;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// An InjectionFactory for transforming application configuration files into settings objects.
    /// </summary>
    /// <typeparam name="TSettings">Type of settings object to instantiate</typeparam>
    public class ConfigurationSettingsFactory<TSettings> : IConfigurationSettingsFactory<TSettings> where TSettings : IConfigurationSettings, new()
    {
        #region constructor

        private readonly IConfigurationService _configurationService;
        private readonly IModelValidationService _modelValidationService;

        public ConfigurationSettingsFactory(
            IConfigurationService configurationService,
            IModelValidationService modelValidationService
            )
        {
            _configurationService = configurationService;
            _modelValidationService = modelValidationService;
        }

        #endregion

        #region public

        /// <summary>
        /// Creates an instance of a settings objects, extracting setting values from
        /// a configuration source.
        /// </summary>
        public TSettings Create()
        {
            var settings = new TSettings();
            var objType = typeof(TSettings);
            var properties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance); ;

            foreach (var property in properties)
            {
                var name = GetPropertyName(settings, property);
                var stringValue = _configurationService.GetSettingOrDefault(name, null);
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    var typedValue = ConvertValue(property, stringValue, name);
                    property.SetValue(settings, typedValue, null);
                }
            }

            var errors = _modelValidationService.GetErrors(settings);

            if (!EnumerableHelper.IsNullOrEmpty(errors))
            {
                throw new InvalidConfigurationException(objType.Name, errors);
            }

            return settings;
        }

        #endregion

        #region private helpers

        private string GetPropertyName(TSettings settings, PropertyInfo property)
        {
            const string SETTINGS_SUFFIX = "Settings";
            string name = typeof(TSettings).Name;

            if (name.EndsWith(SETTINGS_SUFFIX))
            {
                name = name.Remove(name.Length - SETTINGS_SUFFIX.Length);
            }

            name += ":" + property.Name;

            if (settings is INamespacedConfigurationSettings)
            {
                name = ((INamespacedConfigurationSettings)settings).Namespace + ":" + name;
            }

            return name;
        }

        private object ConvertValue(PropertyInfo property, string rawValue, string settingName)
        {
            var valueType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var typeConverter = GetFromStringConverter(valueType);
            object value;

            if (typeConverter != null)
            {
                value = typeConverter.ConvertFromString(null, CultureInfo.CurrentCulture, rawValue);
            }
            else
            {
                throw new ApplicationException("Unable to find a type converter for property '" + settingName + "' of type '" + property.GetType() + "'");
            }

            return value;
        }

        private TypeConverter GetFromStringConverter(Type type)
        {
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (typeConverter != null && !typeConverter.CanConvertFrom(typeof(String)))
            {
                typeConverter = null;
            }
            return typeConverter;
        }

        #endregion
    }
}
