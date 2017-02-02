using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a permission template for an action partitioned by 
    /// User Area. When loading permissions an instance of the template
    /// will be created for each registered IUserAreaDefinition.
    /// </summary>
    public interface IUserAreaPermissionTemplate : IEntityPermission
    {
        /// <summary>
        /// Create an implementation of this template using the specified user area
        /// </summary>
        ICustomEntityPermissionTemplate CreateImplemention(IUserAreaDefinition userArea);
    }
}
