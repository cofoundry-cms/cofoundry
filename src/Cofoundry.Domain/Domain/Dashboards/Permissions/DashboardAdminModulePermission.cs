using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DashboardAdminModulePermission : IPermission
    {
        public const string PermissionTypeCode = "COFDSH";

        public DashboardAdminModulePermission()
        {
            PermissionType = new PermissionType("COFDSH", "Dashboard", "View the dashboard in the admin panel");
        }

        public PermissionType PermissionType { get; private set; }
    }
}
