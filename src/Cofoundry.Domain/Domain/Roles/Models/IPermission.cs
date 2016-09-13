using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A basic permission.
    /// </summary>
    public interface IPermission : IPermissionApplication
    {
        /// <summary>
        /// The action that is permitted by having this permission. 
        /// </summary>
        PermissionType PermissionType { get; }
    }
}
