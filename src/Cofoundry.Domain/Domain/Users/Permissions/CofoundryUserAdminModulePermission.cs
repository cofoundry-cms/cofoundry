using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CofoundryUserAdminModulePermission : IEntityPermission
    {
        public CofoundryUserAdminModulePermission()
        {
            EntityDefinition = new UserEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
