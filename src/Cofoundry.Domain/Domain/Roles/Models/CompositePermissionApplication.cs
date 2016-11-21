using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This applies multiple permissions in an 'OR' arrangement in that the
    /// permission evaluation is true if the user has at least one of the
    /// specified permissions.
    /// </summary>
    public class CompositePermissionApplication : IPermissionApplication
    {
        private static readonly IPermission[] _emptyPermissions = new IPermission[0];

        public CompositePermissionApplication()
        {
            Permissions = _emptyPermissions;
        }

        public CompositePermissionApplication(params IPermission[] permissions)
        {
            Permissions = permissions;
        }

        /// <summary>
        /// The collection of permissions to check. A user would need only one
        /// of these permissions to pass the evaluation of this application.
        /// </summary>
        public IPermission[] Permissions { get; set; }
    }
}
