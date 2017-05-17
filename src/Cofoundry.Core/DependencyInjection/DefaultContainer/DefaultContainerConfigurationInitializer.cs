using Cofoundry.Core.Configuration;
using Cofoundry.Core.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Adds DI configuration for IConfigurationSettings implementations
    /// dicovered through type scanning.
    /// </summary>
    public class DefaultContainerConfigurationInitializer
    {
        private static readonly MethodInfo _registerOptionsWithServiceMethod = typeof(DefaultContainerConfigurationInitializer).GetMethod(nameof(RegisterOptionsWithService), BindingFlags.NonPublic | BindingFlags.Instance);

        #region constructor

        private readonly IServiceCollection _serviceCollection;
        private readonly IDiscoveredTypesProvider _discoveredTypesProvider;
        private readonly IConfigurationRoot _configurationRoot;

        public DefaultContainerConfigurationInitializer(
            IServiceCollection serviceCollection,
            IDiscoveredTypesProvider discoveredTypesProvider,
            IConfigurationRoot configurationRoot
            )
        {
            _serviceCollection = serviceCollection;
            _discoveredTypesProvider = discoveredTypesProvider;
            _configurationRoot = configurationRoot;
        }

        #endregion

        public void Initialize()
        {
            var settingsTypes = GetAllSettingsTypes();

            foreach (var settingType in settingsTypes)
            {
                var settingName = GetSettingsSectionName(settingType);
                var section = _configurationRoot.GetSection(settingName);

                var genericMethod = _registerOptionsWithServiceMethod.MakeGenericMethod(settingType);
                genericMethod.Invoke(this, new object[] { section });
            }
        }

        private void RegisterOptionsWithService<TOptions>(IConfigurationSection section) where TOptions : class
        {
            _serviceCollection.Configure<TOptions>(section);
        }

        private string GetSettingsSectionName(Type settingsType)
        {
            const string SETTINGS_SUFFIX = "Settings";

            string name = settingsType.Name;

            if (name.EndsWith(SETTINGS_SUFFIX))
            {
                name = name.Remove(name.Length - SETTINGS_SUFFIX.Length);
            }

            var namespaceAttribute = settingsType
                .GetCustomAttributes(true)
                .Where(a => a is NamespacedConfigurationSettingAttribute)
                .Cast<NamespacedConfigurationSettingAttribute>()
                .FirstOrDefault();

            if (namespaceAttribute != null)
            {
                name = namespaceAttribute.Namespace + ":" + name;
            }

            return name;
        }

        private IEnumerable<Type> GetAllSettingsTypes()
        {
            var dependencyRegistrationType = typeof(IConfigurationSettings);

            var settingsTypes = _discoveredTypesProvider
                .GetDiscoveredTypes()
                .Where(t => t.IsClass
                    && t.IsPublic
                    && !t.IsAbstract
                    && !t.ContainsGenericParameters
                    && dependencyRegistrationType.IsAssignableFrom(t)
                    );

            return settingsTypes;
        }
    }
}
