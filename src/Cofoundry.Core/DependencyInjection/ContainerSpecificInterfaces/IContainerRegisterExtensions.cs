using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    public static class IContainerRegisterExtensions
    {
        /// <summary>
        /// Registers a DbContext using the default convention, which is to use InstanceScope.PerLifetimeScope
        /// and register it as itself only.
        /// </summary>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public static IContainerRegister RegisterDatabase<T>(this IContainerRegister container) where T : DbContext
        {
            var options = new RegistrationOptions();
            options.InstanceScope = InstanceScope.PerLifetimeScope;
            return container.RegisterType<T, T>(options);
        }

        /// <summary>
        /// Registers a service with InstanceScope.PerLifetimeScope
        /// </summary>
        /// <typeparam name="TRegisterAs">Type to register the service as</typeparam>
        /// <typeparam name="TConcrete">Concrete type to register.</typeparam>
        /// <returns>The IContainerRegister instance for method chaining.</returns>
        public static IContainerRegister RegisterPerRequestScope<TRegisterAs, TConcrete>(this IContainerRegister container) where TConcrete : TRegisterAs
        {
            var options = new RegistrationOptions();
            options.InstanceScope = InstanceScope.PerLifetimeScope;
            return container.RegisterType<TRegisterAs, TConcrete>(options);
        }
    }
}
