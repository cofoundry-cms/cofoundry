using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SandboxDependency
{
    public interface IAssemblyDiscoveryRule
    {
        bool CanInclude(RuntimeLibrary libraryToCheck, IAssemblyDiscoveryRuleContext context);
    }
}
