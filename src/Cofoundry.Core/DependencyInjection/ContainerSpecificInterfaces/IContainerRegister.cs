using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Bare bones DI container registration abstraction to be 
    /// used by modules to keep them DI Container agnostic and
    /// also allow overriding of other module defaults.
    /// </summary>
    public interface IContainerRegister
    {
        /// <summary>
        /// Basic access to application configuration settings.
        /// </summary>
        IContainerConfigurationHelper Configuration { get; }

        /// <summary>
        /// Registers a service instance using InstanceLifetime.Singleton.
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as.</typeparam>
        /// <param name="instance">The instance to register.</param>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterSingleton<TRegisterAs>(TRegisterAs instance, RegistrationOptions options = null);

        /// <summary>
        /// Registers a service as its concrete type only, using the default 
        /// InstanceLifetime (Transient).
        /// </summary>
        /// <typeparam name="TConcrete">Type to register.</typeparam>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister Register<TConcrete>(RegistrationOptions options = null);

        /// <summary>
        /// Registers the type against itself and multiple interfaces or base classes 
        /// specified in the types parameter. The type is registered using the default 
        /// InstanceLifetime (Transient).
        /// </summary>
        /// <typeparam name="TConcrete">Type to register.</typeparam>
        /// <param name="types">Types to register as.</param>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister Register<TConcrete>(ICollection<Type> types, RegistrationOptions options = null);

        /// <summary>
        /// Registers a service using the default InstanceLifetime (Transient).
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as</typeparam>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister Register<TRegisterAs, TConcrete>(RegistrationOptions options = null) where TConcrete : TRegisterAs;

        /// <summary>
        /// Registers a service as part of a collection of services, so that when an array of TRegisterAs
        /// is requested, all services registered through this method are returned.
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as</typeparam>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterInCollection<TRegisterAs, TConcrete>(RegistrationOptions options = null) where TConcrete : TRegisterAs;

        /// <summary>
        /// Registers all services that implement TToRegister as a registered collection of services.
        /// </summary>
        /// <typeparam name="TToRegister">Type to scan for implementations</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterAll<TToRegister>(RegistrationOptions options = null);

        /// <summary>
        /// Registers all services that implement typeToRegisterImplementationsOf as their
        /// concrete type and as a collection services. The types are constructed
        /// using a resolved instance of the genericFactoryType which should implement 
        /// IInjectionFactory
        /// </summary>
        /// <param name="typeToRegisterImplementationsOf">Type to scan for implementations</param>
        /// <param name="genericFactoryType">
        /// Open generic type of factory to use for construction. Should inherit
        /// from IInjectionFactory&lt;&gt;
        /// </param>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterAllWithFactory(Type typeToRegisterImplementationsOf, Type genericFactoryType, RegistrationOptions options = null);

        /// <summary>
        /// Registers all services that implement a generic interface. Each service
        /// is registered individually as a separate generic implementation of TGeneric
        /// </summary>
        /// <param name="typeDef">Generic interface to scan for implementations.</param>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterAllGenericImplementations(Type typeDef, RegistrationOptions options = null);

        /// <summary>
        /// Registers an open generic type
        /// </summary>
        /// <param name="registerAs">Type to register the service as e.g. typeof(IRepository&lt;&gt;)</param>
        /// <param name="typeToRegister">Concrete type to register e.g. typeof(Repository&lt;&gt;)</param>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterGeneric(Type registerAs, Type typeToRegister, RegistrationOptions options = null);

        /// <summary>
        /// Registers a type with a factory that is used to construct the type when it is resolved.
        /// </summary>
        /// <typeparam name="TToRegister">The type tp be registered</typeparam>
        /// <typeparam name="TFactory">The IInjectionFactory that should be used to construct the type.</typeparam>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterFactory<TToRegister, TFactory>(RegistrationOptions options = null) where TFactory : IInjectionFactory<TToRegister>;

        /// <summary>
        /// Registers a type with a factory that is used to construct the type when it is resolved.
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to be registered with the container.</typeparam>
        /// <typeparam name="TConcrete">The concrete trype tp be registered</typeparam>
        /// <typeparam name="TFactory">The IInjectionFactory that should be used to construct the type.</typeparam>
        /// <param name="options">Optional options argument.</param>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        IContainerRegister RegisterFactory<TRegisterAs, TConcrete, TFactory>(RegistrationOptions options = null)
            where TFactory : IInjectionFactory<TConcrete>
            where TConcrete : TRegisterAs;
    }
}
