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
    /// Default implementation of IContainerRegister that uses the basic
    /// DI utilities provided by Microsoft.Extensions.DependencyInjection
    /// </summary>
    public class DefaultContainerRegister : IContainerRegister
    {
        private static readonly MethodInfo _registerFactoryMethod = typeof(DefaultContainerRegister).GetMethod(nameof(RegisterFactoryReflectionDelegate), BindingFlags.NonPublic | BindingFlags.Instance);
        private static InstanceScope DEFAULT_INSTANCE_SCOPE = InstanceScope.PerLifetimeScope;

        private readonly IDiscoveredTypesProvider _discoveredTypesProvider;
        private readonly IServiceCollection _serviceCollection;
        private readonly DefaultContainerBuilder _containerBuilder;
        private readonly ContainerConfigurationHelper _containerConfigurationHelper;

        public DefaultContainerRegister(
            IDiscoveredTypesProvider discoveredTypesProvider,
            IServiceCollection serviceCollection,
            DefaultContainerBuilder containerBuilder,
            IConfigurationRoot configurationRoot
            )
        {
            if (discoveredTypesProvider == null) throw new ArgumentNullException(nameof(discoveredTypesProvider));
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));
            if (configurationRoot == null) throw new ArgumentNullException(nameof(configurationRoot));

            _discoveredTypesProvider = discoveredTypesProvider;
            _serviceCollection = serviceCollection;
            _containerBuilder = containerBuilder;
            _containerConfigurationHelper = new ContainerConfigurationHelper(configurationRoot);
        }

        #region IContainerRegister implementation

        public IContainerConfigurationHelper Configuration { get => _containerConfigurationHelper; }

        public IContainerRegister RegisterInstance<TRegisterAs>(TRegisterAs instance, RegistrationOptions options = null)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var fn = new Action(() =>
            {
                _serviceCollection
                    .AddSingleton(typeof(TRegisterAs), instance)
                    ;
            });

            Register<TRegisterAs>(fn, options);

            return this;
        }

        public IContainerRegister RegisterInstance<TRegisterAs>(RegistrationOptions options = null)
        {
            return RegisterInstance<TRegisterAs, TRegisterAs>(options);
        }

        public IContainerRegister RegisterInstance<TRegisterAs, TConcrete>(RegistrationOptions options = null) where TConcrete : TRegisterAs
        {
            if (options == null)
            {
                options = new RegistrationOptions();
            }
            options.InstanceScope = InstanceScope.Singleton;
            return RegisterType<TRegisterAs, TConcrete>(options);
        }

        public IContainerRegister RegisterType<TConcrete>(RegistrationOptions options = null)
        {
            return RegisterType<TConcrete, TConcrete>(options);
        }

        public IContainerRegister RegisterType<TConcrete>(Type[] types, RegistrationOptions options = null)
        {
            var fn = new Action(() =>
            {
                var concreteType = typeof(TConcrete);

                AddService(concreteType, concreteType, options);

                foreach (var type in EnumerableHelper
                    .Enumerate(types)
                    .Where(t => t != concreteType)
                    )
                {
                    AddServiceWithFactory(type, x => x.GetService<TConcrete>(), options);
                }
            });

            Register<TConcrete>(fn, options);

            return this;
        }

        private static IServiceCollection Add(
            IServiceCollection collection,
            Type serviceType,
            Type implementationType,
            ServiceLifetime lifetime
            )
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
            collection.Add(descriptor);
            return collection;
        }

        public IContainerRegister RegisterType<TRegisterAs, TConcrete>(RegistrationOptions options = null) where TConcrete : TRegisterAs
        {
            var fn = new Action(() =>
            {
                AddService(typeof(TRegisterAs), typeof(TConcrete), options);
            });

            Register<TRegisterAs>(fn, options);

            return this;
        }

        public IContainerRegister RegisterTypeInCollection<TRegisterAs, TConcrete>() where TConcrete : TRegisterAs
        {
            AddService(typeof(TRegisterAs), typeof(TConcrete));

            return this;
        }

        public IContainerRegister RegisterAll<TToRegister>()
        {
            var typeToRegister = typeof(TToRegister);

            var concreteTypes = GetDiscoveredConcreteTypes()
                .Where(t => typeToRegister.GetTypeInfo().IsAssignableFrom(t) && t.AsType() != typeToRegister);

            foreach (var concreteTypeInfo in concreteTypes)
            {
                var concreteType = concreteTypeInfo.AsType();
                AddService(concreteType, concreteType);
                AddServiceWithFactory(typeToRegister, c => c.GetRequiredService(concreteType));
            }

            return this;
        }

        public IContainerRegister RegisterAllGenericImplementations(Type typeDef)
        {
            if (!typeDef.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException("TGeneric should be generic");
            }

            var handlerRegistrations =
                from implementation in GetDiscoveredConcreteTypes()
                let services =
                    from iface in implementation.GetInterfaces().Select(t => t.GetTypeInfo())
                    where iface.IsGenericType
                    where iface.GetGenericTypeDefinition() == typeDef
                    select iface
                from service in services
                select new { service, implementation };

            foreach (var handler in handlerRegistrations)
            {
                AddService(handler.service.AsType(), handler.implementation.AsType());
            }

            return this;
        }

        public IContainerRegister RegisterAllWithFactory(
            Type typeToRegisterImplementationsOf,
            Type genericFactoryType,
            RegistrationOptions options = null
            )
        {
            var typesToRegister = GetDiscoveredConcreteTypes()
                .Select(t => t.AsType())
                .Where(t => typeToRegisterImplementationsOf.IsAssignableFrom(t)
                    && t != typeToRegisterImplementationsOf
                );

            foreach (var typeToRegister in typesToRegister)
            {
                var factoryType = genericFactoryType.MakeGenericType(typeToRegister);
                var genericRegistrationMethod = _registerFactoryMethod.MakeGenericMethod(typeToRegister, factoryType);
                genericRegistrationMethod.Invoke(this, new object[] { options });
            }

            return this;
        }

        /// <summary>
        /// Private version of RegisterFactory used to prevent ambiguous
        /// matches when using reflection to get the MethodInfo
        /// </summary>
        private IContainerRegister RegisterFactoryReflectionDelegate<TConcrete, TFactory>(RegistrationOptions options = null) where TFactory : IInjectionFactory<TConcrete>
        {
            return RegisterFactory<TConcrete, TFactory>(options);
        }

        public IContainerRegister RegisterFactory<TToRegister, TFactory>(RegistrationOptions options = null) where TFactory : IInjectionFactory<TToRegister>
        {
            return RegisterFactory<TToRegister, TToRegister, TFactory>(options);
        }

        public IContainerRegister RegisterFactory<TRegisterAs, TConcrete, TFactory>(RegistrationOptions options = null)
            where TFactory : IInjectionFactory<TConcrete>
            where TRegisterAs : TConcrete
        {
            var fn = new Action(() =>
            {
                var factoryType = typeof(TFactory);
                // If the factory is a concrete type, we should make sure it is registered
                if (!factoryType.GetTypeInfo().IsInterface)
                {
                    AddService(factoryType, factoryType);
                }

                AddServiceWithFactory(typeof(TConcrete), c => c.GetRequiredService<TFactory>().Create(), options);
            });

            Register<TRegisterAs>(fn, options);

            return this;
        }

        public IContainerRegister RegisterGeneric(Type registerAs, Type typeToRegister)
        {
            AddService(registerAs, typeToRegister);

            return this;
        }

        #endregion

        #region helpers

        private void Register<TTo>(Action register, RegistrationOptions options = null)
        {
            if (options != null && options.ReplaceExisting)
            {
                _containerBuilder.QueueRegistration<TTo>(register);
            }
            else
            {
                register();
            }
        }

        private void AddService(
           Type serviceType,
           Type implementationType,
           RegistrationOptions options = null
           )
        {
            var lifetime = ConvertToServiceLifetime(options);

            var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
            _serviceCollection.Add(descriptor);
        }

        private void AddServiceWithFactory(
           Type serviceType,
           Func<IServiceProvider, object> implementationFactory,
           RegistrationOptions options = null
            )
        {
            var lifetime = ConvertToServiceLifetime(options);

            var descriptor = new ServiceDescriptor(serviceType, implementationFactory, lifetime);
            _serviceCollection.Add(descriptor);
        }

        public ServiceLifetime ConvertToServiceLifetime(RegistrationOptions options)
        {
            var scope = options?.InstanceScope ?? DEFAULT_INSTANCE_SCOPE;

            if (scope == null)
            {
                scope = DEFAULT_INSTANCE_SCOPE;
            }

            if (scope == InstanceScope.PerLifetimeScope)
            {
                return ServiceLifetime.Scoped;
            }

            if (scope == InstanceScope.Transient)
            {
                return ServiceLifetime.Transient;
            }

            if (scope == InstanceScope.Singleton)
            {
                return ServiceLifetime.Singleton;
            }

            throw new ArgumentException("InstanceScope '" + scope.GetType().FullName + "' not recognised");
        }

        private IEnumerable<TypeInfo> GetDiscoveredConcreteTypes()
        {
            return _discoveredTypesProvider
                .GetDiscoveredTypes()
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    &&!t.ContainsGenericParameters
                    );
        }

        #endregion
    }
}
