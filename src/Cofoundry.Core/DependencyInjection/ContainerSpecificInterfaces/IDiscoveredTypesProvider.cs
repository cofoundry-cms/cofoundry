using System.Reflection;

namespace Cofoundry.Core.DependencyInjection;

/// <summary>
/// Provides access to assemblies and types that can be scanned to
/// register batches of dependencies e.g. types that implement a 
/// specific interface.
/// </summary>
public interface IDiscoveredTypesProvider
{
    /// <summary>
    /// Gets a subset of loaded assemblies that are candidates for 
    /// scanned dependency registrations.
    /// </summary>
    IEnumerable<Assembly> GetDiscoveredAssemblies();

    /// <summary>
    /// Gets a collection of all types from the discovered assemblies collection.
    /// </summary>
    IEnumerable<TypeInfo> GetDiscoveredTypes();
}
