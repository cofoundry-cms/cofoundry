using Cofoundry.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandboxDependency.DI
{
    public class DefaultContainerBuilder : IContainerBuilder
    {
        #region Privates

        private readonly Dictionary<Type, Action> RegistrationOverrides = new Dictionary<Type, Action>();
        private readonly IServiceCollection _serviceCollection;
        private readonly IDiscoveredTypesProvider _discoveredTypesProvider;
        bool hasBuilt = false;

        #endregion

        #region constructor

        public DefaultContainerBuilder(
            IServiceCollection serviceCollection,
            IDiscoveredTypesProvider discoveredTypesProvider
            )
        {
            _serviceCollection = serviceCollection;
            _discoveredTypesProvider = discoveredTypesProvider;
        }

        #endregion

        #region public methods

        public void Build()
        {
            CheckIsBuilt();

            var containerRegister = new DefaultContainerRegister(_discoveredTypesProvider, _serviceCollection, this);

            var registrations = GetAllRegistrations();
            foreach (var registration in registrations)
            {
                registration.Register(containerRegister);
            }

            BuildOverrides();
        }

        internal void QueueRegistration<TTo>(Action registration)
        {
            var typeToRegister = typeof(TTo);
            if (RegistrationOverrides.ContainsKey(typeToRegister))
            {
                throw new ArgumentException("Type already registered as an override. Multiple overrides currently not supported: " + typeToRegister);
            }

            RegistrationOverrides.Add(typeToRegister, registration);
        }

        #endregion

        #region helpers

        private void CheckIsBuilt()
        {
            if (hasBuilt)
            {
                throw new InvalidOperationException("The container has already been built.");
            }
            hasBuilt = true;
        }

        private void BuildOverrides()
        {
            foreach (var registrationOverride in RegistrationOverrides)
            {
                registrationOverride.Value();
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
                    && dependencyRegistrationType.IsAssignableFrom(t)
                    );

            foreach (var registrationType in registrationTypes)
            {
                if (registrationType.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new InvalidOperationException(registrationType.Name + " does not have a public parameterless constructor. Types that implement IDependencyRegistration do not support constructor injection.");
                }

                yield return (IDependencyRegistration)Activator.CreateInstance(registrationType);
            }
        }

        #endregion
    }
}
