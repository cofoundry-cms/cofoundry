using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Defines a rule that is used to determine if an assembly should be added
    /// to the ApplicationPartManager and therefore be included for feature and
    /// DI type discovery.
    /// </summary>
    public interface IAssemblyDiscoveryRule
    {
        /// <summary>
        /// Returns true if the specified runtime library should be added to
        /// the ApplicationPartManager; otherwise false.
        /// </summary>
        /// <param name="libraryToCheck">The loaded assembly to check.</param>
        /// <param name="context">Contextual information about the discovery process such as the entry assembly.</param>
        /// <returns>
        /// True if the specified runtime library should be added to
        /// the ApplicationPartManager; otherwise false.
        /// </returns>
        bool CanInclude(RuntimeLibrary libraryToCheck, IAssemblyDiscoveryRuleContext context);
    }
}
