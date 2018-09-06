using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.DependencyInjection
{
    public static class ContainerRegisterExtensions
    {
        private static RegistrationOptions SCOPED_OPTIONS = RegistrationOptions.Scoped();
        private static RegistrationOptions SINGLETON_OPTIONS = RegistrationOptions.SingletonScope();

        /// <summary>
        /// Registers a service as its concrete type only, using InstanceLifetime.Scoped.
        /// </summary>
        /// <typeparam name="TConcrete">Type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public static IContainerRegister RegisterScoped<TConcrete>(this IContainerRegister containerRegister)
        {
            return containerRegister.Register<TConcrete>(SCOPED_OPTIONS);
        }

        /// <summary>
        /// Registers a service using InstanceLifetime.Scoped.
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as.</typeparam>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public static IContainerRegister RegisterScoped<TRegisterAs, TConcrete>(this IContainerRegister containerRegister) 
            where TConcrete : TRegisterAs
        {
            return containerRegister.Register<TRegisterAs, TConcrete>(SCOPED_OPTIONS);
        }

        /// <summary>
        /// Registers a service instance using InstanceLifetime.Singleton.
        /// </summary>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public static IContainerRegister RegisterSingleton<TConcrete>(this IContainerRegister containerRegister)
        {
            return containerRegister.Register<TConcrete>(SINGLETON_OPTIONS);
        }

        /// <summary>
        /// Registers a service instance using InstanceLifetime.Singleton.
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as.</typeparam>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public static IContainerRegister RegisterSingleton<TRegisterAs, TConcrete>(this IContainerRegister containerRegister)
            where TConcrete : TRegisterAs
        {
            return containerRegister.Register<TRegisterAs, TConcrete>(SINGLETON_OPTIONS);
        }
    }
}
