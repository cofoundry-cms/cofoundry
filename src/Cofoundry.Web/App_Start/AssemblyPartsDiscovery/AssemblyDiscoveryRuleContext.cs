using System.Reflection;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IAssemblyDiscoveryRuleContext"/>.
/// </summary>
public class AssemblyDiscoveryRuleContext : IAssemblyDiscoveryRuleContext
{
    public AssemblyDiscoveryRuleContext(
        Assembly entryAssembly
        )
    {
        EntryAssembly = entryAssembly;
        EntryAssemblyName = entryAssembly.GetName();
    }

    /// <inheritdoc/>
    public AssemblyName EntryAssemblyName { get; }

    /// <inheritdoc/>
    public Assembly EntryAssembly { get; }
}
