using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SandboxDependency
{
    public interface IDiscoveredTypesProvider
    {
        IEnumerable<Assembly> GetDiscoveredAssemblies();

        IEnumerable<TypeInfo> GetDiscoveredTypes();
    }
}
