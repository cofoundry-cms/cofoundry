using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a permission template for a custom entity. When loading permissions an instance of the template
    /// will be created for each custom entity.
    /// </summary>
    public interface ICustomEntityPermissionTemplate : IEntityPermission
    {
        /// <summary>
        /// Create an implementation of this template using the specified custom entity definition
        /// </summary>
        ICustomEntityPermissionTemplate CreateImplemention(ICustomEntityDefinition customEntityDefinition);
    }
}
