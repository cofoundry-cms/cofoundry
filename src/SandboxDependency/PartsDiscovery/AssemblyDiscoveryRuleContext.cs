using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SandboxDependency
{
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
}
