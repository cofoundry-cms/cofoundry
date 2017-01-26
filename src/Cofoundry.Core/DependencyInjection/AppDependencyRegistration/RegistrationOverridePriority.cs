using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Enum of integer values that indicate priority levels to use when overriding 
    /// component registrations. Any integer can be used, but these indicate recommended 
    /// values.
    /// </summary>
    public enum RegistrationOverridePriority
    {
        /// <summary>
        /// Will override the default implementation and nothing more. Typically used
        /// inside the framework to override a default/empty implementation lower down 
        /// in the framework.
        /// </summary>
        Low = -50,

        /// <summary>
        /// Default and the option to typically use in a plugin. Will override the existing 
        /// implementation and any low level (typically default/placeholder) implementation.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// A higher level priority that should rarely be used (and never in the framework or plugins), but may be 
        /// needed in a client application to override a plugin.
        /// </summary>
        High = 50
    }
}
