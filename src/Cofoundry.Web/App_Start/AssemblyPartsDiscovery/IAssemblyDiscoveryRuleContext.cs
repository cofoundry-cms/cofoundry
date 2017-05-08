using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Contains contextual information about a running assembly discovery process.
    /// </summary>
    public interface IAssemblyDiscoveryRuleContext
    {
        /// <summary>
        /// The name of the application entry assembly.
        /// </summary>
        AssemblyName EntryAssemblyName { get; }

        /// <summary>
        /// A reference to the application entry assembly.
        /// </summary>
        Assembly EntryAssembly { get; }
    }
}
