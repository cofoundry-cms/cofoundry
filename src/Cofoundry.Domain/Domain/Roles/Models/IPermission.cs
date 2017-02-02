using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A basic permission that allows a user to perform an 
    /// action e.g. "view dashboard" or "view error log" but more
    /// commonly associated with an entity type using IEntityPermission
    /// </summary>
    public interface IPermission : IPermissionApplication
    {
        /// <summary>
        /// The action that is permitted by having this permission. 
        /// </summary>
        PermissionType PermissionType { get; }
    }
}
