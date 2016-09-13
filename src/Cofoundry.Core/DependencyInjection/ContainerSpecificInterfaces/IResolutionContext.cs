using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Dependency resolution from a DI container for the current
    /// scope.
    /// </summary>
    public interface IResolutionContext
    {
        /// <summary>
        /// Returns the registered implementation of a service
        /// </summary>
        /// <typeparam name="TService">Type of service to resolve.</typeparam>
        /// <returns>TService instance.</returns>
        TService Resolve<TService>();

        /// <summary>
        /// Returns the registered implementation of a service
        /// </summary>
        /// <param name="t">Type of service to resolve.</param>
        /// <returns>Service instance.</returns>
        object Resolve(Type t);

        /// <summary>
        /// Returns all registered implementation of a service
        /// </summary>
        /// <typeparam name="TService">Type of service to resolve.</typeparam>
        /// <returns>All registered instances of TService.</returns>
        IEnumerable<TService> ResolveAll<TService>();

        /// <summary>
        /// Creates a child context inheriting the parent contexts settings
        /// but can be used with a different lifetime scope.
        /// </summary>
        /// <returns>Child IResolutionContext</returns>
        IChildResolutionContext CreateChildContext();

        /// <summary>
        /// Determines if the specified generic type is registered in the container
        /// </summary>
        /// <typeparam name="T">Type to check for registration</typeparam>
        /// <returns>True if the type is registered; otherwise false</returns>
        bool IsRegistered<T>();

        /// <summary>
        /// Determines if the specified type is registered in the container
        /// </summary>
        /// <param name="typeToCheck">Type to check for registration</typeparam>
        /// <returns>True if the type is registered; otherwise false</returns>
        bool IsRegistered(Type typeToCheck);
    }
}
