using Cofoundry.Core.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// An IDiscoveredTypesProvider that uses the ApplicationPartManager
    /// to retreive types.
    /// </summary>
    public class DiscoveredTypesProvider : IDiscoveredTypesProvider
    {
        private readonly ApplicationPartManager _applicationPartManager;

        public DiscoveredTypesProvider(
            ApplicationPartManager applicationPartManager
            )
        {
            _applicationPartManager = applicationPartManager;
        }

        public IEnumerable<Assembly> GetDiscoveredAssemblies()
        {
            return _applicationPartManager
                .ApplicationParts
                .OfType<AssemblyPart>()
                .Select(p => p.Assembly);
        }

        public IEnumerable<TypeInfo> GetDiscoveredTypes()
        {
            return _applicationPartManager
                .ApplicationParts
                .OfType<AssemblyPart>()
                .SelectMany(p => p.Types);
        }
    }
}
