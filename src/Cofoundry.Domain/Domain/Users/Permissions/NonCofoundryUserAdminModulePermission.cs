using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class NonCofoundryUserAdminModulePermission : IEntityPermission
    {
        public NonCofoundryUserAdminModulePermission()
        {
            EntityDefinition = new NonCofoundryUserEntityDefinition();
            PermissionType = CommonPermissionTypes.AdminModule("Non Cofoundry Users");
        }

        public IEntityDefinition EntityDefinition { get; private set; }
        public PermissionType PermissionType { get; private set; }
    }
}
