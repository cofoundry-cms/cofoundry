using System.Reflection;

namespace Cofoundry.Web;

public class AssemblyDiscoveryRuleContext : IAssemblyDiscoveryRuleContext
{
    public AssemblyDiscoveryRuleContext(
        Assembly entryAssembly
        )
    {
        EntryAssembly = entryAssembly;
        EntryAssemblyName = entryAssembly.GetName();
    }

    public AssemblyName EntryAssemblyName { get; }
    public Assembly EntryAssembly { get; }
}
