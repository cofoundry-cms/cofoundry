﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Core.DependencyInjection;

/// <summary>
/// Default implementation of IContainerBuilder that uses the basic
/// DI utilities provided by Microsoft.Extensions.DependencyInjection
/// </summary>
public class DefaultContainerBuilder : IContainerBuilder
{
    bool hasBuilt;

    private readonly Dictionary<Type, RegistrationOverride> RegistrationOverrides = [];
    private readonly IServiceCollection _serviceCollection;
    private readonly IDiscoveredTypesProvider _discoveredTypesProvider;
    private readonly IConfiguration _configurationRoot;

    public DefaultContainerBuilder(
        IServiceCollection serviceCollection,
        IDiscoveredTypesProvider discoveredTypesProvider,
        IConfiguration configurationRoot
        )
    {
        _serviceCollection = serviceCollection;
        _discoveredTypesProvider = discoveredTypesProvider;
        _configurationRoot = configurationRoot;
    }

    public void Build()
    {
        CheckIsBuilt();
        RegisterFramework();

        var containerRegister = new DefaultContainerRegister(
            _discoveredTypesProvider,
            _serviceCollection,
            this,
            _configurationRoot
            );

        var registrations = GetAllRegistrations();
        foreach (var registration in registrations)
        {
            registration.Register(containerRegister);
        }

        BuildOverrides();
    }

    internal void QueueRegistration<TTo>(Action registration, int priority)
    {
        var typeToRegister = typeof(TTo);
        if (RegistrationOverrides.TryGetValue(typeToRegister, out var value))
        {
            var existingOverride = value;

            // Don't allow the registrations with the same priority, but do 
            // replace lower priority registrations
            if (existingOverride.Priority == priority)
            {
                var errorMessage = $"Type {typeToRegister.FullName} is already registered as an override with the specified prioerity value ({priority}). Multiple overrides with the same priority values are not supported.";
                throw new InvalidTypeRegistrationException(typeToRegister, errorMessage);
            }
            else if (existingOverride.Priority < priority)
            {
                existingOverride.Registration = registration;
            }
        }
        else
        {
            RegistrationOverrides.Add(typeToRegister, new RegistrationOverride(registration, priority));
        }
    }

    private struct RegistrationOverride
    {
        public RegistrationOverride(Action registration, int priority)
        {
            Registration = registration;
            Priority = priority;
        }

        public Action Registration;
        public int Priority;
    }

    private void CheckIsBuilt()
    {
        if (hasBuilt)
        {
            throw new InvalidOperationException("The container has already been built.");
        }
        hasBuilt = true;
    }

    private void RegisterFramework()
    {
        // Register all configuration settings
        var settingsInitializer = new DefaultContainerConfigurationInitializer(
            _serviceCollection,
            _discoveredTypesProvider,
            _configurationRoot
            );

        settingsInitializer.Initialize();
    }

    private void BuildOverrides()
    {
        foreach (var registrationOverride in RegistrationOverrides)
        {
            registrationOverride.Value.Registration();
        }
    }

    private IEnumerable<IDependencyRegistration> GetAllRegistrations()
    {
        var dependencyRegistrationType = typeof(IDependencyRegistration);

        var registrationTypes = _discoveredTypesProvider
            .GetDiscoveredTypes()
            .Where(t => t.IsClass
                && t.IsPublic
                && !t.IsAbstract
                && !t.ContainsGenericParameters
                && dependencyRegistrationType.IsAssignableFrom(t.AsType())
                );

        foreach (var registrationType in registrationTypes)
        {
            if (registrationType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new InvalidOperationException(registrationType.Name + " does not have a public parameterless constructor. Types that implement IDependencyRegistration do not support constructor injection.");
            }

            var registration = Activator.CreateInstance(registrationType.AsType()) as IDependencyRegistration;

            if (registration == null)
            {
                throw new Exception($"Error instantiating dependency registration type {registrationType.FullName}");
            }

            yield return registration;
        }
    }
}
