using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SandboxDependency
{
    public interface IAssemblyDiscoveryRuleContext
    {
        AssemblyName EntryAssemblyName { get; }
        Assembly EntryAssembly { get; }
    }
}
