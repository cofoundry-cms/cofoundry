using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CurrentUserAdminModulePermission : IEntityPermission
    {
        public CurrentUserAdminModulePermission()
        {
            EntityDefinition = new CurrentUserEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Current User Account");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
