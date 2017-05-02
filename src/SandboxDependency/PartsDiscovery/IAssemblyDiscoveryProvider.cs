using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SandboxDependency
{
    public interface IAssemblyDiscoveryProvider
    {
        IEnumerable<Assembly> DiscoverAssemblies(
            IMvcBuilder mvcBuilder,
            IEnumerable<IAssemblyDiscoveryRule> rules
            );
    }
}
