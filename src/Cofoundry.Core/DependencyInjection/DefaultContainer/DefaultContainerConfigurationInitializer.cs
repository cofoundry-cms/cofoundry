﻿using Cofoundry.Core.Configuration;
using Cofoundry.Core.Reflection.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Cofoundry.Core.DependencyInjection;

/// <summary>
/// Adds DI configuration for IConfigurationSettings implementations
/// dicovered through type scanning.
/// </summary>
public class DefaultContainerConfigurationInitializer
{
    private static readonly MethodInfo _registerOptionsWithServiceMethod = MethodReferenceHelper.GetPrivateInstanceMethod<DefaultContainerConfigurationInitializer>(nameof(RegisterOptionsWithService));

    private readonly IServiceCollection _serviceCollection;
    private readonly IDiscoveredTypesProvider _discoveredTypesProvider;
    private readonly IConfiguration _configuration;

    public DefaultContainerConfigurationInitializer(
        IServiceCollection serviceCollection,
        IDiscoveredTypesProvider discoveredTypesProvider,
        IConfiguration configuration
        )
    {
        _serviceCollection = serviceCollection;
        _discoveredTypesProvider = discoveredTypesProvider;
        _configuration = configuration;
    }

    public void Initialize()
    {
        var settingsTypes = GetAllSettingsTypes();

        foreach (var settingType in settingsTypes)
        {
            var settingName = GetSettingsSectionName(settingType);
            var section = _configuration.GetSection(settingName);

            var genericMethod = _registerOptionsWithServiceMethod.MakeGenericMethod(settingType.AsType());
            genericMethod.Invoke(this, new object[] { section });
        }
    }

    private void RegisterOptionsWithService<TOptions>(IConfigurationSection section) where TOptions : class
    {
        _serviceCollection.Configure<TOptions>(section);
    }

    private static string GetSettingsSectionName(TypeInfo settingsType)
    {
        const string SETTINGS_SUFFIX = "Settings";

        var name = settingsType.Name;

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

    private IEnumerable<TypeInfo> GetAllSettingsTypes()
    {
        var dependencyRegistrationType = typeof(IConfigurationSettings);

        var settingsTypes = _discoveredTypesProvider
            .GetDiscoveredTypes()
            .Where(t => t.IsClass
                && t.IsPublic
                && !t.IsAbstract
                && !t.ContainsGenericParameters
                && dependencyRegistrationType.IsAssignableFrom(t.AsType())
                );

        return settingsTypes;
    }
}