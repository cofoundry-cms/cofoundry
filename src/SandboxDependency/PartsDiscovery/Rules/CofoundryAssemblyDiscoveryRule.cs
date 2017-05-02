using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;

namespace SandboxDependency
{
    public class CofoundryAssemblyDiscoveryRule : IAssemblyDiscoveryRule
    {
        public bool CanInclude(RuntimeLibrary libraryToCheck, IAssemblyDiscoveryRuleContext context)
        {
            return libraryToCheck.Name.IndexOf("Cofoundry", StringComparison.OrdinalIgnoreCase) != -1
                || libraryToCheck.Name.StartsWith(context.EntryAssemblyName.Name, StringComparison.OrdinalIgnoreCase)
                || libraryToCheck.Name.IndexOf("Plugin", StringComparison.OrdinalIgnoreCase) > 0;
        }
    }
}
