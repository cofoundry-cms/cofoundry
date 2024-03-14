using Microsoft.Extensions.DependencyModel;

namespace Cofoundry.Web;

/// <summary>
/// Basic assembly discovery rule that includes Cofoundry assemblies, the executing application
/// assembly and assemblies with a name that contains the word "Plugin".
/// </summary>
public class CofoundryAssemblyDiscoveryRule : IAssemblyDiscoveryRule
{
    /// <inheritdoc/>
    public bool CanInclude(RuntimeLibrary libraryToCheck, IAssemblyDiscoveryRuleContext context)
    {
        var entryAssemblyName = context.EntryAssemblyName.Name;

        return libraryToCheck.Name.Contains("Cofoundry", StringComparison.OrdinalIgnoreCase)
            || (entryAssemblyName != null && libraryToCheck.Name.StartsWith(entryAssemblyName, StringComparison.OrdinalIgnoreCase))
            || libraryToCheck.Name.IndexOf("Plugin", StringComparison.OrdinalIgnoreCase) > 0;
    }
}
