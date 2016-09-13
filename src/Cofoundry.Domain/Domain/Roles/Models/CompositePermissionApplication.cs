using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
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

        public IPermission[] Permissions { get; set; }
    }
}
