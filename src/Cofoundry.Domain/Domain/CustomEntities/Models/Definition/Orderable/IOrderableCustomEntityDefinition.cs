using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Implement this interface to define a custom entity type that can have it's list ordering set. 
    /// The definition will automatically get picked up and added to the system.
    /// </summary>
    public interface IOrderableCustomEntityDefinition : ICustomEntityDefinition
    {
        /// <summary>
        /// Indicates the type of ordering permitted on this custom entity type.
        /// </summary>
        CustomEntityOrdering Ordering { get; }
    }
}
